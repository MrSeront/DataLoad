using GraphMVVM.ViewModel;
using GraphMVVM.DataGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphMVVM.DataGraph
{
    
    public class BaseDataModel : VM, IData
    {
        private int time;
        private int current;
        private bool code;
        private int pairingAlgoritm;


        public int PairingAlgoritm
        {

            get { return pairingAlgoritm; }
            set
            {
                pairingAlgoritm = value;
                OnPropertyChanged();
            }
        }
        public int Time
        {
            get { return time; }
            set
            {
                if (value>=0)
                {
                    time = value;
                    OnPropertyChanged();
                }
                
            }
        }
        public int Current
        {
            get { return current; }
            set
            {
                current = value;
                OnPropertyChanged();
            }
        }
        public bool OscillatorOn
        {
            get { return code; }
            set { code = value; OnPropertyChanged(); }

        }
       
    }
}

