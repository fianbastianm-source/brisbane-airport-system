using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Base abstraction for all the UI “screens”. Where each screen renders itself and
    /// returns the next screen to show, enabling a simple screen state machine.
    /// </summary>
    internal abstract class Screen
    {
        public abstract Screen? Show(App app);
    }
}