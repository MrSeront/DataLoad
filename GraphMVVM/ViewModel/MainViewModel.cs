using System;
using System.Threading.Tasks;
using GraphMVVM.Model;
using GraphMVVM.DirectoryTree;
using GraphMVVM.DataGraph;

using System.Windows.Controls;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows;
using System.Threading;
using GraphMVVM.PowerSource;
using System.IO.Ports;
using OxyPlot;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Splat;
using DynamicData.Binding;
using DynamicData;
using System.Windows.Threading;

namespace GraphMVVM.ViewModel
{
    public class MainViewModel : ReactiveObject
    {

        [Reactive] public DataGridCellInfo CellInfo { get; set; }
        [Reactive] public int ProgressValue { get; set; }
        [Reactive] public string ProgressText { get; set; }
        [Reactive] public ObservableCollection<GridTable> FirstOpenData { get; set; } = new ObservableCollection<GridTable>();
        [Reactive] public string[] Ports { get; set; }
        [Reactive] public byte PortIndex { get; set; }
        [Reactive] public ObservableCollection<BaseDataModel> SecondPlotData { get; set; } = new ObservableCollection<BaseDataModel>();
        [Reactive] public ObservableCollection<BaseDataModel> ProgressData { get; set; } = new ObservableCollection<BaseDataModel>();
        [Reactive] public ObservableCollection<DirectoryModel> DirectoryTreeData { get; set; } = new ObservableCollection<DirectoryModel>();
        [Reactive] public PlotModel FirstOxyPlotModel { get; set; }
        [Reactive] public PlotModel SecondOxyPlotModel { get; set; }
        [Reactive] public ObservableCollection<GridTable> GridTableData { get; set; } = new ObservableCollection<GridTable>();
        [Reactive] public string Status { get; set; }

        [Reactive] public bool IsMilliSecondsSleep { get; set; }


        //private DataGridCellInfo cellInfo;
        //public DataGridCellInfo CellInfo
        //{
        //    get { return cellInfo; }
        //    set
        //    {
        //        cellInfo = value;
        //        OnPropertyChanged();

        //    }
        //}

        //private int selectedIndexDataGrid;
        //public int SelectedIndexDataGrid
        //{
        //    get { return selectedIndexDataGrid; }
        //    set
        //    {
        //        selectedIndexDataGrid = value;
        //        OnPropertyChanged();

        //    }
        //}
        #region private переменные

        // private ObservableCollection<GridTable> firstOpenData = new ObservableCollection<GridTable>();
        // private ObservableCollection<BaseDataModel> secondPlotData = new ObservableCollection<BaseDataModel>();
        // private ObservableCollection<DirectoryModel> directoryTreeData = new ObservableCollection<DirectoryModel>();
        // private ObservableCollection<GridTable> gridTableData = new ObservableCollection<GridTable>();
        // private string[] ports;
        // private byte portIndex;
        //// private int progressValue;

        bool DeleteMethodFlag = false;
        // private string[] oscill = new string[2];
        // private string progressText;
        // private bool dataflag = false;


        // private PlotModel firstOxyPlotModel = new PlotModel();
        // private PlotModel secondOxyPlotModel = new PlotModel();
        //public PlotModel FirstOxyPlotModel { get; private set; }
        //public PlotModel SecondOxyPlotModel { get; private set; }
        #endregion

        #region public переменные
        public bool AutoSendCheck { get; set; }
        //public string PathToFile = "";
        SerialPort sp = new SerialPort();
        PowerSourceMainModel<BaseDataModel> PowerSource = new PowerSourceMainModel<BaseDataModel>();

        public ObservableCollection<BaseDataModel> SelectedData = new ObservableCollection<BaseDataModel>();
        public ObservableCollection<GridTable> SelectedDataSegment = new ObservableCollection<GridTable>();

        public LineSeries LineSeriesFirst = new LineSeries();
        public LineSeries LineSeriesSecond = new LineSeries();

        public LineSeries LineSeriesSelectedLine = new LineSeries();
        public LineSeries LineSeriesSelectedPoint = new LineSeries();

        public LineSeries LineSeriesProgressFirst = new LineSeries();
        public LineSeries LineSeriesProgressSecond = new LineSeries();

        public LinearAxis xAxisFirst = new LinearAxis();
        public LinearAxis yAxisFirst = new LinearAxis();

        public LinearAxis xAxisSecond = new LinearAxis();
        public LinearAxis yAxisSecond = new LinearAxis();

        DataPoint pStart;
        DataPoint pEnd;
        DataPoint pMove;

        int counter;

        bool ClickFlag = false;
        #endregion

        #region комманды

      

        private ICommand selectedTreeViewItemChangedCommand; // команда при изменении выделения в TreeView
        public ICommand SelectedTreeViewItemChangedCommand
        {
            get
            {
                if (selectedTreeViewItemChangedCommand == null)
                    selectedTreeViewItemChangedCommand = new RelayCommand(args => SelectedTreeViewItemChanged(args));
                return selectedTreeViewItemChangedCommand;
            }
        }

       

        public ReactiveCommand<Unit, Unit> AddPointCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeletePointCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveDataCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> WeldingCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ACommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CopySegmentCommand { get; private set; }

        readonly ObservableAsPropertyHelper<bool> isWelding;
        public bool IsWelding
        {
            get { return isWelding.Value; }
        }
        readonly ObservableAsPropertyHelper<bool> isA;
        public bool IsA
        {
            get { return isA.Value; }
        }

        [Reactive] public bool IsDataLoad { get; set; } = false;
        [Reactive] public string PathToFile { get; set; }
        #endregion

        public MainViewModel()
        {

            var canExecute = this.WhenAnyValue(x => x.PathToFile, (PathToFile) => PathToFile!="" && PathToFile != null);
            var canExecute1 = this.WhenAnyValue(x => x.IsWelding, (isWelding) => isWelding != true);

            SaveDataCommand = ReactiveCommand.Create(() => DataGraphViewModel.SaveData(GridTableData));
            WeldingCommand = ReactiveCommand.CreateFromObservable(Welding,canExecute);
            WeldingCommand.IsExecuting.ToProperty(this, x => x.IsWelding, out isWelding);

            ACommand = ReactiveCommand.CreateFromObservable(A, canExecute1);
            ACommand.IsExecuting.ToProperty(this, x => x.IsA, out isA);



            SettingOxyPlot();

            GetDirectoryTreeDataAsync();

            Ports = SerialPort.GetPortNames();



        }

       

        private IObservable<Unit> Welding()
        {
            Status = "Отправка данных в СОМ-port";
            return Observable.Start(() =>
            {
                GetProgressAsync();
                    PowerSource.CreateAndSendCsvFile(PathToFile, IsMilliSecondsSleep);
                    Status = "Отправка данных завершена";
            }
            );
        }

        private IObservable<Unit> A()
        {
            Status = "Отправка комманды \"A\"";
            return Observable.Start(() =>
            {

                PowerSource.Welding();
                Status = "Отправка комманды завершена";
            }
            );
        }

        private void SettingOxyPlot()
        {
            SecondOxyPlotModel = new PlotModel();
            FirstOxyPlotModel = new PlotModel();
          

            LineSeriesSelectedLine.DataFieldX = "Time";
            LineSeriesSelectedLine.DataFieldY = "Current";
            LineSeriesSelectedLine.Color = OxyColors.Red;
            LineSeriesSelectedLine.StrokeThickness = 4;

            LineSeriesSelectedPoint.DataFieldX = "Time";
            LineSeriesSelectedPoint.DataFieldY = "Current";
            LineSeriesSelectedPoint.MarkerSize = 6;
            LineSeriesSelectedPoint.MarkerType = MarkerType.Circle;
            LineSeriesSelectedPoint.MarkerFill = OxyColors.Red;
            LineSeriesSelectedPoint.StrokeThickness = 0;

            LineSeriesFirst.ItemsSource = SecondPlotData;
            LineSeriesFirst.DataFieldX = "Time";
            LineSeriesFirst.DataFieldY = "Current";
            LineSeriesFirst.StrokeThickness = 4;
            LineSeriesFirst.CanTrackerInterpolatePoints = false;

            LineSeriesSecond.ItemsSource = LineSeriesFirst.ItemsSource;
            LineSeriesSecond.DataFieldX = LineSeriesFirst.DataFieldX;
            LineSeriesSecond.DataFieldY = LineSeriesFirst.DataFieldY;
            LineSeriesSecond.StrokeThickness = LineSeriesFirst.StrokeThickness;
            LineSeriesSecond.CanTrackerInterpolatePoints = LineSeriesFirst.CanTrackerInterpolatePoints;

            LineSeriesProgressFirst.DataFieldX = "Time";
            LineSeriesProgressFirst.DataFieldY = "Current";
            LineSeriesProgressFirst.Color = OxyColors.Blue;
            LineSeriesProgressFirst.StrokeThickness = 4;

            LineSeriesProgressSecond.DataFieldX = "Time";
            LineSeriesProgressSecond.DataFieldY = "Current";
            LineSeriesProgressSecond.Color = OxyColors.Blue;
            LineSeriesProgressSecond.StrokeThickness = 4;

            //LineSeriesFirst.Points.Add(new DataPoint(0, 0));
            //LineSeriesFirst.Points.Add(new DataPoint(10, 18));
            //LineSeriesFirst.Points.Add(new DataPoint(20, 12));
            //LineSeriesFirst.Points.Add(new DataPoint(30, 8));
            //LineSeriesFirst.Points.Add(new DataPoint(40, 15));

            xAxisFirst.Title = "Время";
            xAxisFirst.Position = AxisPosition.Bottom;
            xAxisFirst.MajorGridlineStyle = LineStyle.Solid;
            xAxisFirst.MajorGridlineThickness = 1;
            xAxisFirst.MajorGridlineColor = OxyColors.LightGray;

            xAxisSecond.Title = xAxisFirst.Title;
            xAxisSecond.Position = xAxisFirst.Position;
            xAxisSecond.MajorGridlineStyle = xAxisFirst.MajorGridlineStyle;
            xAxisSecond.MajorGridlineThickness = xAxisFirst.MajorGridlineThickness;
            xAxisSecond.MajorGridlineColor = xAxisFirst.MajorGridlineColor;

            yAxisFirst.Title = "ток, А";
            yAxisFirst.Position = AxisPosition.Left;

            yAxisFirst.ExtraGridlines = new double[] { 0 };
            yAxisFirst.ExtraGridlineThickness = 2;
            yAxisFirst.ExtraGridlineColor = OxyColors.Black;

            yAxisFirst.MajorGridlineStyle = LineStyle.Solid;
            yAxisFirst.MajorGridlineThickness = 1;
            yAxisFirst.MajorGridlineColor = OxyColors.LightGray;


            yAxisSecond.Title = yAxisFirst.Title;
            yAxisSecond.Position = yAxisFirst.Position;

            yAxisSecond.ExtraGridlines = yAxisFirst.ExtraGridlines;
            yAxisSecond.ExtraGridlineThickness = yAxisFirst.ExtraGridlineThickness;
            yAxisSecond.ExtraGridlineColor = yAxisFirst.ExtraGridlineColor;

            yAxisSecond.MajorGridlineStyle = yAxisFirst.MajorGridlineStyle;
            yAxisSecond.MajorGridlineThickness = yAxisFirst.MajorGridlineThickness;
            yAxisSecond.MajorGridlineColor = yAxisFirst.MajorGridlineColor;

            //FirstOxyPlotModel.Axes.Add(xAxisFirst);
            //FirstOxyPlotModel.Axes.Add(yAxisFirst);

            //FirstOxyPlotModel.Series.Add(LineSeriesFirst);

            FirstOxyPlotModel.Axes.Add(xAxisFirst);
            FirstOxyPlotModel.Axes.Add(yAxisFirst);

            FirstOxyPlotModel.Series.Add(LineSeriesFirst);
            FirstOxyPlotModel.Series.Add(LineSeriesProgressFirst);

            SecondOxyPlotModel.Axes.Add(xAxisSecond);
            SecondOxyPlotModel.Axes.Add(yAxisSecond);

            SecondOxyPlotModel.Series.Add(LineSeriesSecond);
            SecondOxyPlotModel.Series.Add(LineSeriesSelectedLine);
            SecondOxyPlotModel.Series.Add(LineSeriesProgressSecond);
            SecondOxyPlotModel.Series.Add(LineSeriesSelectedPoint);

          

        }



        #region TreeView methods

        private void SelectedTreeViewItemChanged(object args)
        {
            if (args is FileModel)
            {
                SecondPlotData.Clear();

                FileModel data = args as FileModel;
                PathToFile = data.Path;

              
                GetDataForGraph();

                AddGridTableData(FirstOpenData);

                SettingsPort(Ports[PortIndex]);

                PowerSource.ChangePort(sp);

                PowerSource.ClearData();

                CopyDataOnClickTreeViewAsync(GridTableData, SecondPlotData);
                ProgressData.Clear();
               // GetProgressAsync();
                //}
            }
            if (args is DirectoryModel)
            {
                DirectoryTreeData.Clear();
                DirectoryModel data = args as DirectoryModel;
                GetDirectoryTreeDataAsync(data.Path);
            }
        }

        public void GetDataForGraph()
        {
            if ((PathToFile.Substring(PathToFile.LastIndexOf(".") + 1)) == "imp")
            {
                FirstOpenData.Clear();
                FirstOpenData = DataGraphViewModel.GetDataForGraph(PathToFile);
            }
            if ((PathToFile.Substring(PathToFile.LastIndexOf(".") + 1)) == "csv" || (PathToFile.Substring(PathToFile.LastIndexOf(".") + 1)) == "txt")
            {
                FirstOpenData.Clear();
                FirstOpenData = DataGraphViewModel.GetDataForGraphCsv(PathToFile);
            }
        }

        private void SettingsPort(string portName)
        {
            if (!sp.IsOpen)
            {
                sp.PortName = portName;
                sp.BaudRate = 921600;
                sp.DataBits = 8;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
                sp.ReadTimeout = 5000;
                sp.WriteTimeout = 5000;
                sp.Handshake = Handshake.None;
            }
        }

        ObservableCollection<GridTable> OriginalTables = new ObservableCollection<GridTable>(); // Коллекция элементов до изменения полей

        private void AddGridTableData(ObservableCollection<GridTable> gridTable)
        {
            GridTableData.Clear();
            OriginalTables.Clear();

            GridTable.Counter = 0;

            for (int i = 0; i < gridTable.Count; i++)
            {
                GridTableData.Add(new GridTable
                {
                    Time = gridTable[i].Time,
                    Current = gridTable[i].Current,
                    Id = i + 1,
                    OscillatorOn = gridTable[i].OscillatorOn,
                    PairingAlgoritm = gridTable[i].PairingAlgoritm,

                });

                OriginalTables.Add(new GridTable
                {
                    Time = gridTable[i].Time,
                    Current = gridTable[i].Current,
                    Id = i + 1,
                    OscillatorOn = gridTable[i].OscillatorOn,
                    PairingAlgoritm = gridTable[i].PairingAlgoritm,

                });
            }
           ;
        }

        public async void GetDirectoryTreeDataAsync()
        {
            await Task.Run(() => GetDirectoryTreeData());
        }

        public void GetDirectoryTreeData()
        {
            DirectoryTreeData = DirectoryTreeViewModel.InitializeDirectoryTree();
        }

        public async void GetDirectoryTreeDataAsync(string path)
        {
            await Task.Run(() => GetDirectoryTreeData(path));
        }

        public void GetDirectoryTreeData(string path)
        {
            DirectoryTreeData = DirectoryTreeViewModel.GetDirectoryTreeData(path);
        }

        public async void CopyDataOnClickTreeViewAsync(ObservableCollection<GridTable> gridTables, ObservableCollection<BaseDataModel> baseDataModels)
        {
            await Task.Run(() => CopyDataOnClickTreeView(gridTables, out baseDataModels));
            if (AutoSendCheck)
            {
                Status = "Sending data to a power source";
                await Task.Run(() => PowerSource.SendData(baseDataModels));
                Status = "Вata sending completed";
            }
        }

        public void CopyDataOnClickTreeView(ObservableCollection<GridTable> gridTables, out ObservableCollection<BaseDataModel> baseDataModels)
        {
            baseDataModels = DataGraphViewModel.CopyData(gridTables);
            SecondPlotData = baseDataModels;
            LineSeriesFirst.ItemsSource = baseDataModels;
            LineSeriesSecond.ItemsSource = baseDataModels;

            FirstOxyPlotModel.InvalidatePlot(true);
            SecondOxyPlotModel.InvalidatePlot(true);

        }
        #endregion


     
        public void SendDataToPowerSource(ObservableCollection<BaseDataModel> baseDataModels)
        {
          
                PowerSource.SendData(baseDataModels);
        }
        

        public async void GetProgressAsync()
        {
            await Task.Run(() => GetProgress());
        }


        public void GetProgress()
        {

            if (ClickFlag == false)
            {
                
                do
                {
                    ProgressData = PowerSource.GetProgressData();
                    ProgressValue = PowerSource.GetProgress();

                    Thread.Sleep(200);
                    ProgressText = Convert.ToString(ProgressValue) + " %";
                    LineSeriesProgressFirst.ItemsSource = ProgressData;
                    LineSeriesProgressSecond.ItemsSource = ProgressData;
                    SecondOxyPlotModel.InvalidatePlot(true);
                    FirstOxyPlotModel.InvalidatePlot(true);
                }
                while (ProgressValue < 100);
                

            }
        }
    
    }
}

