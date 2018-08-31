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
        }

        ILogSink logger = new ConsoleLogger();
        Machine m;
        BigInteger instructionsExecuted;
        Machine.RunResult result;
        private void EndRun(object sender, EventArgs e)
        {
            result = m.LastRunResult;
            switch (result)
            {
                case Machine.RunResult.None:
                    break;
                case Machine.RunResult.CancellationSignalled:
                    Console.Write("Run aborted. ");
                    break;
                case Machine.RunResult.IllegalInstruction:
                    Console.Write($"Illegal instruction at {m.ProgramCounter}! ");
                    break;
            }
            BigInteger newInstructionsExecuted = m.InstructionsExecuted;
            BigInteger diff = newInstructionsExecuted - instructionsExecuted;
            Console.WriteLine($"{diff.ToString("N0")} instructions executed.");
            instructionsExecuted += newInstructionsExecuted;
            Program.RestorePrompt();
        }

        public void WaitForStop()
        {
            runThread.Join();
        }

        public void New()
        {
            if (m != null)
            {
                m.RunStateChanged -= EndRun;
                m.Logger = null;
            }
            m = new Machine();
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
                    args.Cancel = true;
                    ct.Cancel();
                }
            }
        }

        public void PrintMemory(int start, int stop)
        {


            int wordsPerLine = (Console.WindowWidth - 2 - Word.Size * 2) / (Word.Size * 2 + 1);

            // round start address to nearest start of line.
            start = wordsPerLine * (int)Math.Floor(start / (double)wordsPerLine);

            // round stop address to nearest end of line.
            stop = wordsPerLine * (int)Math.Ceiling(stop / (double)wordsPerLine);

            int byteCount = stop - start;
            var data = new byte[byteCount];
            m.DMARead(data, data.Length, start);
            //m.Memory.Seek(start, System.IO.SeekOrigin.Begin);
            //m.Memory.Read(data, 0, data.Length);

            // line format:
            // start = word01
            // stop = word08
            // 0x123456 word01 word02 word03 word04 word05
            // 0x123465 word06 word07 word08 word09 word10

            int lineCount = (int)Math.Ceiling(byteCount / (double)Word.Size / wordsPerLine);

            var line = new StringBuilder();
            string format = "X" + Word.Size * 2;

        }

        public void PrintRegisters()
        {
            Console.WriteLine($"PC: {m.ProgramCounter}");
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
