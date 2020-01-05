using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphMVVM.ViewModel;

namespace GraphMVVM.DirectoryTree
{
    public class Item : VM
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
