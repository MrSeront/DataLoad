using GraphMVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphMVVM.View;

namespace GraphMVVM.DirectoryTree
{
    
   public class DirectoryModel:Item

    {

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
        

        private ObservableCollection<Item> children;
        public ObservableCollection<Item> Children
        {
            get { return children; }
            set
            {
                children = value;
                OnPropertyChanged("Children");
               
            }
        }

        public DirectoryModel()
        {
            
            Children = new ObservableCollection<Item>();
            this.PropertyChanged += (s, a) => {

            };
        }
    }

}
