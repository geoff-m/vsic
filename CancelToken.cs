using System.Diagnostics;

namespace Visual_SICXE
{
    public class CancelToken
    {
        private readonly object locker = new object();
        private bool c = false;
        public bool Cancelled
        {
            get
            {
                lock(locker)
                {
                    return c;
                }
            }
            private set
            {
                lock(locker)
                {
                    c = value;
                }
            }
        }

        public void Cancel()
        {
            Debug.Assert(!Cancelled, "Already cancelled!");
            Cancelled = true;
        }

        public void Reset()
        {
            Debug.WriteLineIf(Cancelled, "Reset before being cancelled!");
            Cancelled = false;
        }
    }
}
