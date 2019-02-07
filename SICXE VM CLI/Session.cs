using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SICXE;
using System.Diagnostics;
using System.Numerics;
using System.Timers;

namespace SICXE_VM_CLI
{
    class Session
    {
        public Session()
        {
            timer = new System.Timers.Timer(SPEED_CHECK_INTERVAL_MILLISECONDS);
            timer.Elapsed += UpdateSpeedStats;
            logger = new ConsoleLogger();
        }

        ILogSink logger;
        Machine m;
        BigInteger instructionsExecuted;
        Machine.RunResult result;
        private void EndRun(object sender, EventArgs e) // todo: fix: this method is being invoked twice.
        {
            if (m.IsRunning)
                return;

            result = m.LastRunResult;
            Console.WriteLine(GetMessageFor(result));
            BigInteger newInstructionsExecuted = m.InstructionsExecuted;
            BigInteger diff = newInstructionsExecuted - instructionsExecuted;
            Console.WriteLine($"{diff.ToString("N0")} instructions executed.");
            instructionsExecuted += newInstructionsExecuted;
            Program.RestorePrompt();
        }

        public void WaitForStop()
        {
            runThread?.Join();
        }

        public void New()
        {
            if (m != null)
            {
                m.RunStateChanged -= EndRun;
                m.Logger = null;
            }
            m = new Machine();
            m.RegisterL = (Word)0x2b;
            m.Logger = logger;
            m.RunStateChanged += EndRun;
            instructionsExecuted = 0;
        }

        public void New(int memory)
        {
            m = new Machine(memory);
        }

        public void LoadOBJ(string path)
        {
            m.LoadObj(path);
        }

        private Thread runThread;
        readonly object runLocker = new object();
        private CancelToken ct;
        public void BeginRun(ulong steps, CancelToken ct)
        {
            if (IsRunning)
                return;
            lock (runLocker)
            {
                if (runThread != null)
                {
                    if (runThread.IsAlive)
                        runThread.Join();
                }
                this.ct = ct;
                runThread = new Thread(() => {
                    timer.Start();
                    Machine.RunResult res = Machine.RunResult.None;
                    for (; steps > 0 && !ct.Cancelled; --steps)
                    {
                        res = m.Step();
                        if (res != Machine.RunResult.None)
                            break;
                    }
                    timer.Stop();
                    EndRun(this, null);
                });
                runThread.Start();
            }
        }

        public void BeginRun(CancelToken ct)
        {
            if (IsRunning)
                return;
            lock (runLocker)
            {
                if (runThread != null)
                {
                    if (runThread.IsAlive)
                        runThread.Join();
                }
                this.ct = ct;
                runThread = new Thread(() => {
                    timer.Start();
                    m.Run(ct);
                    timer.Stop();
                });
                runThread.Start();
            }
        }

        public void Step()
        {
            if (IsRunning)
                return;
            var result = m.Step();
            string message = GetMessageFor(result);
            if (message.Length > 0)
                Console.WriteLine(message);
        }

        private string GetMessageFor(Machine.RunResult r)
        {
            switch (r)
            {
                case Machine.RunResult.None:
                    return "";
                case Machine.RunResult.CancellationSignalled:
                    return "Run aborted. ";
                case Machine.RunResult.IllegalInstruction:
                    return $"Illegal instruction at {m.ProgramCounter}!";
                case Machine.RunResult.HitBreakpoint:
                    return "Hit breakpoint!";
                case Machine.RunResult.HardwareFault:
                    return "A hardware fault occurred!";
                case Machine.RunResult.AddressOutOfBounds:
                    return "An instruction referenced an out-of-bounds address!";
                case Machine.RunResult.EndOfMemory:
                    return "The program counter has reached the end of memory.";
            }
            Debug.Fail("Unknown run result!");
            return "";
        }

        System.Timers.Timer timer;
        BigInteger timeStartInstructions = BigInteger.Zero;
        int blankOut = 0;
        const double SPEED_CHECK_INTERVAL_SECONDS = 1.5;
        const int SPEED_CHECK_INTERVAL_MILLISECONDS = (int)(1000 * SPEED_CHECK_INTERVAL_SECONDS);
        bool anyData = false;
        private void UpdateSpeedStats(object sender, ElapsedEventArgs args)
        {
            BigInteger now = m.InstructionsExecuted;
            if (anyData)
            {
                ConsoleHelper.PushCursorPosition();

                BigInteger diff = now - timeStartInstructions;
                ulong rate = (ulong)((double)diff / SPEED_CHECK_INTERVAL_SECONDS);

                string toPrint = rate.ToString("N0");
                int len = toPrint.Length;
                if (len > blankOut)
                    blankOut = len;
                ConsoleHelper.WriteRightAligned($"{toPrint.PadLeft(blankOut, ' ')} OP/S");

                ConsoleHelper.PopCursorPosition();
            }
            else
                anyData = true;
            timeStartInstructions = now;
        }

        public bool IsRunning
        {
            get
            {
                //Debug.WriteLine("IsRunning waiting for lock...");
                lock (runLocker)
                {
                    //Debug.WriteLine("IsRunning got lock.");
                    return runThread != null && runThread.IsAlive;
                }
            }
        }

        public void ControlC(object sender, ConsoleCancelEventArgs args)
        {
            if (!IsRunning)
                return;
            //Debug.WriteLine("ControlC waiting for lock...");
            lock (runLocker)
            {
                //Debug.WriteLine("ControlC got lock.");
                if (ct != null)
                {
                    if (args!= null)
                        args.Cancel = true;
                    ct.Cancel();
                }
            }
        }

        public int MemorySize => m.MemorySize;

        public void PrintMemory(int start, int stop)
        {
            int byteCount = stop - start;
            var data = new byte[byteCount];
            m.DMARead(data, data.Length, start);

            var line = new StringBuilder();

            const int addressChars = 9; // "0x123456 "

            // bytesPerLine * 2 + bytesPerLine - 1 + addressChars = WindowWidth
            // bytesPerLine * (2 + 1) = WindowWidth + 1 - addressChars
            int bytesPerLine = (Console.WindowWidth - addressChars + 1) / 3;
            
            for (int b = 0; b < byteCount; ++b)
            {
                line.Append($"0x{b + start:X6} ");
                for (int lineByte = 0; lineByte < bytesPerLine && b + lineByte < byteCount; ++lineByte)
                {
                    int effectiveByte = b + lineByte;
                    line.Append($"{data[effectiveByte]:X2} ");
                }
                line.Remove(line.Length - 1, 1);
                Console.WriteLine(line.ToString());
                line.Clear();
                b += bytesPerLine - 1;
            }

        }

        public void PrintRegisters()
        {
            Console.WriteLine($"PC: {m.ProgramCounter}");
            Console.WriteLine($"CC: {m.ConditionCode}");
            Console.WriteLine($"A:  {m.RegisterA}  B:  {m.RegisterB}");
            Console.WriteLine($"X:  {m.RegisterX}  L:  {m.RegisterL}");
            Console.WriteLine($"S:  {m.RegisterS}  T:  {m.RegisterT}");
        }

        public void SetRegister(Register r, Word value)
        {
            m.SetRegister(r, value);
        }

    }
}
