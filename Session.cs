using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace sicsim
{
    /// <summary>
    /// Represents the entire state of a session in vsic, including whatever machine is present.
    /// </summary>
    class Session
    {
        Machine machine;
        SortedSet<Breakpoint> breakpoints;

        /// <summary>
        /// Creates a new, empty session.
        /// </summary>
        public Session()
        {
            machine = new Machine();
            breakpoints = new SortedSet<Breakpoint>(new Breakpoint.Comparer());
        }



    }
}
