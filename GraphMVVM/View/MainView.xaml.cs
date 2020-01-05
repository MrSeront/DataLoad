using GraphMVVM.ViewModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GraphMVVM.View
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window, IViewFor<MainViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
        .Register(nameof(ViewModel), typeof(MainViewModel), typeof(MainView));
       
        //MainViewModel MainViewModel = new MainViewModel();

        public MainView()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
            Closed += (sender, args) => Application.Current.Shutdown();
        }


        public MainViewModel ViewModel
        {
            get => (MainViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel)value;
        }

    }


}

