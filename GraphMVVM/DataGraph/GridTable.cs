using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using GraphMVVM.DataGraph;

namespace GraphMVVM.Model
{
   
    public class GridTable:BaseDataModel
    {
        
        public static int Counter { get; set; }

       
        private bool isSelected;
      
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        //public bool IsSelected { get; set; }
        
        private string[] comboBoxData;
       
        public string[] ComboBoxData
        { 
            get { return comboBoxData; }
            set { comboBoxData = value; OnPropertyChanged(); }
        }
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value;}
        }

        private string[] pairing = new string[3];

        public GridTable()
        {
            pairing[0] = "Линия";
            pairing[1] = "Синус выпуклый";
            pairing[2] = "Синус вогнутый";

            Counter++;

            Id = Counter;

            ComboBoxData = pairing;
        }
    }


    //public enum Algoritm
    //{
    //    [Description("Линейный")]
    //    Linear = 0,
    //    [Description("Синус. вог")]
    //    SinusA = 1,
    //    [Description("Синус. вып")]
    //    SinusB = 2,
    //}


}
