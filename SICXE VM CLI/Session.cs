using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SICXE;
using System.Diagnostics;
using System.Numerics;

namespace SICXE_VM_CLI
{
    class Session
    {
        ILogSink logger = new ConsoleLogger();
        Machine m;
        BigInteger instructionsExecuted;
        Machine.RunResult result;
        private void RunFinished(object sender, EventArgs e)
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
                m.RunStateChanged -= RunFinished;
                m.Logger = null;
            }
            m = new Machine();
            m.Logger = logger;
            m.RunStateChanged += RunFinished;
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

        Thread runThread;
        readonly object runLocker = new Object();
        CancelToken ct;
        public void BeginRun(CancelToken ct)
        {
            if (IsRunning)
                return;
            Debug.WriteLine("BeginRun waiting for lock...");
            lock (runLocker)
            {
                Debug.WriteLine("BeginRun got lock.");
                if (runThread != null)
                {
                    if (runThread.IsAlive)
                        runThread.Join();
                }
                this.ct = ct;
                runThread = new Thread(() => m.Run(ct));
                runThread.Start();
            }
            Debug.WriteLine("BeginRun release lock.");
        }

        public bool IsRunning
        {
            get
            {
                Debug.WriteLine("IsRunning waiting for lock...");
                lock (runLocker)
                {
                    Debug.WriteLine("IsRunning got lock.");
                    return runThread != null && runThread.IsAlive;
                }
            }
        }

        public void ControlC(object sender, ConsoleCancelEventArgs args)
        {
            if (!IsRunning)
                return;
            Debug.WriteLine("ControlC waiting for lock...");
            lock (runLocker)
            {
                Debug.WriteLine("ControlC got lock.");
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
