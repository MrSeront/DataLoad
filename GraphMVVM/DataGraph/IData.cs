using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMVVM.DataGraph
{
    interface IData
    {
        int Time { get; set; }
        int Current { get; set; }
        bool OscillatorOn { get; set; }
    }
}
