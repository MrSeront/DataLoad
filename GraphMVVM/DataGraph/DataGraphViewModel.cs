using GraphMVVM.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace GraphMVVM.DataGraph
{
    class DataGraphViewModel
    {
        static public XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<GridTable>));
       
        private static object locker=true;

        static public ObservableCollection<GridTable> GetDataForGraph(string pathToFile)
        {
            ObservableCollection<GridTable> products;
            using (StreamReader file = File.OpenText(pathToFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                return products = (ObservableCollection<GridTable>)serializer.Deserialize(file, typeof(ObservableCollection<GridTable>));
            }


            // ObservableCollection<GridTable> Data = new ObservableCollection<GridTable>();
            // //string json;
            // //using (StreamReader sr = new StreamReader(pathToFile))
            // //{
            // //     json = sr.ReadToEnd();
            // //}
            //// ObservableCollection<GridTable> products = JsonConvert.DeserializeObject<ObservableCollection<GridTable>>(File.ReadAllText(pathToFile));
            // //ObservableCollection<GridTable> products = JsonConvert.DeserializeObject<ObservableCollection<GridTable>>(json);


            //foreach (var t in products)

            //{
            //    Data.Add(new GridTable
            //    {
            //        Time = t.Time,
            //        Current = t.Current,
            //        Code = t.Code,
            //        PairingAlgoritm = t.PairingAlgoritm
            //    });
            //}


            //using (FileStream fs = new FileStream(pathToFile, FileMode.OpenOrCreate))
            //{

            //    ObservableCollection<GridTable> NewData = (ObservableCollection<GridTable>)formatter.Deserialize(fs);

            //    foreach (var t in products)

            //    {
            //        Data.Add(new GridTable
            //        {
            //            Time = t.Time,
            //            Current = t.Current,
            //            Code = t.Code,
            //            PairingAlgoritm = t.PairingAlgoritm
            //        });
            //    }
            //}
            // return Data;
        }

        static public ObservableCollection<GridTable> GetDataForGraphCsv(string pathToFile) // Метод для получения коллекции данных из файла CSV \ xml по пути pathToFile
        {
            ObservableCollection<GridTable> Data = new ObservableCollection<GridTable>();

          
                string[] lines = File.ReadAllLines(pathToFile);

                Data.Add(new GridTable { Time = 0, Current = 0, OscillatorOn = true });
                foreach (var l in lines)
                {
                    string[] cell = l.Split(';');
                    cell[0] = cell[0].Substring(1);
                    if (cell[2] == "")
                    {
                        Data.Add(new GridTable { Time = Convert.ToInt32(cell[0]) - 1, Current = 0, OscillatorOn = false });
                    }
                    else
                    {
                        if ((Convert.ToByte(Convert.ToInt32(cell[2]) & 0x02) == 0x02))
                        {
                            if ((Convert.ToByte(Convert.ToInt32(cell[2]) & 0x80) == 0x80))
                            {
                                Data.Add(new GridTable { Time = Convert.ToInt32(cell[0]) - 1, Current = Convert.ToInt32(cell[1]) * -1, OscillatorOn = true });
                            }
                            else
                            {
                                Data.Add(new GridTable { Time = Convert.ToInt32(cell[0]) - 1, Current = Convert.ToInt32(cell[1]) * -1, OscillatorOn = false });
                            }
                        }
                        else
                        {
                            if ((Convert.ToByte(Convert.ToInt32(cell[2]) & 0x80) == 0x80))
                            {
                                Data.Add(new GridTable { Time = Convert.ToInt32(cell[0]) - 1, Current = Convert.ToInt32(cell[1]), OscillatorOn = true });
                            }
                            else
                            {
                                Data.Add(new GridTable { Time = Convert.ToInt32(cell[0]) - 1, Current = Convert.ToInt32(cell[1]), OscillatorOn = false });
                            }
                        }
                    }
                }
                if (Data[Data.Count - 1].Current != 0) Data.Add(new GridTable { Time = Data.Count - 1, Current = 0, OscillatorOn = true });
           
            return Data;
        }

        /// <summary>
        /// Преобразование коллекции из узловых точек в полную  коллекцию(узловые точки с точками сопряжения)
        /// </summary>
        /// <param name="tables"> коллекция узловых точек </param>
        /// <returns>полная коллекция</returns>

        static public ObservableCollection<BaseDataModel> CopyData(ObservableCollection<GridTable> tables, PropertyChangedEventArgs e)
        {
            lock (locker)
            {
                ObservableCollection<BaseDataModel> Item = new ObservableCollection<BaseDataModel>();
                try
                {
                    if (e.PropertyName != "Id")
                    {
                        Item = CopyData(tables);
                    }
                    return Item;
                }
                catch { return Item; };
            }
        }
        static public ObservableCollection<BaseDataModel> CopyData(ObservableCollection<GridTable> tables)
        {
            Console.WriteLine("Start data copy " + Thread.CurrentThread.ManagedThreadId);
            ObservableCollection<BaseDataModel> Item = new ObservableCollection<BaseDataModel>();
            ObservableCollection<GridTable> BuffGridTable = new ObservableCollection<GridTable>();
            ObservableCollection<GridTable> BuffGridTable2 = new ObservableCollection<GridTable>();

            int k = 0;

            foreach (var i in tables)
            {
                BuffGridTable.Add(i);
            }
            
            BuffGridTable = OrderThoseGroups(BuffGridTable);
           
            GridTable.Counter = 0;
            foreach (var i in BuffGridTable)
            {
                BuffGridTable2.Add(new GridTable { Time = i.Time, Current = i.Current, Id = k + 1, OscillatorOn = i.OscillatorOn, PairingAlgoritm = i.PairingAlgoritm, ComboBoxData = i.ComboBoxData });
                k++;
            }

            int MaxTime = 0;

            for (int i = 0; i < BuffGridTable2.Count; i++) // нахождение максимального значения по времени 
            {
                if (BuffGridTable2[i].Time >= MaxTime)
                {
                    MaxTime = BuffGridTable2[i].Time;
                }
            }

            for (int i = 0; i < MaxTime + 1; i++) // добавление "MaxTime" элементов в коллекцию
            {
                Item.Add(new BaseDataModel { });
            }

            Item[0].Time = 0;
            Item[0].Current = 0;

            for (int i = 0; i < BuffGridTable2.Count - 1; i++)
            {
                int k1 = 0;
                ObservableCollection<BaseDataModel> PairnigPoints = new ObservableCollection<BaseDataModel>();
                if(Math.Sign(BuffGridTable2[i].Current)!= Math.Sign(BuffGridTable2[i+1].Current))
                {
                    if (BuffGridTable2[i].OscillatorOn==true)
                    {
                        PairnigPoints = PairingData(BuffGridTable2[i].Time, BuffGridTable2[i].Current, BuffGridTable2[i + 1].Time, BuffGridTable2[i + 1].Current, BuffGridTable2[i].PairingAlgoritm);
                        Item[BuffGridTable2[i].Time].Time = BuffGridTable2[i].Time;
                        Item[BuffGridTable2[i].Time].Current = BuffGridTable2[i].Current;

                        for (int j = BuffGridTable2[i].Time; j < BuffGridTable2[i + 1].Time; j++)
                        {
                            try
                            {
                                if (BuffGridTable2[i].Current == 0)
                                {
                                    
                                        if (k1==0)
                                        {
                                            Item[j + 1].Time = PairnigPoints[k1].Time;
                                            Item[j + 1].Current = PairnigPoints[k1].Current;
                                            Item[j + 1].OscillatorOn = true;
                                            k1++;
                                        }
                                        else
                                        {
                                            Item[j + 1].Time = PairnigPoints[k1].Time;
                                            Item[j + 1].Current = PairnigPoints[k1].Current;
                                            k1++;
                                        }

                                }
                                else
                                {
                                    if (k1 - 1 < 0)
                                    {
                                        Item[j + 1].Time = PairnigPoints[k1].Time;
                                        Item[j + 1].Current = PairnigPoints[k1].Current;
                                        k1++;

                                    }
                                    else
                                    {
                                        if ((Math.Sign(PairnigPoints[k1].Current) != Math.Sign(PairnigPoints[k1 - 1].Current)) && PairnigPoints[k1].Current!=0)
                                        {
                                            Item[j + 1].Time = PairnigPoints[k1].Time;
                                            Item[j + 1].Current = PairnigPoints[k1].Current;
                                            Item[j + 1].OscillatorOn = true;
                                            k1++;
                                        }
                                        else
                                        {
                                            Item[j + 1].Time = PairnigPoints[k1].Time;
                                            Item[j + 1].Current = PairnigPoints[k1].Current;
                                            k1++;
                                        }
                                    }
                                }
                            }
                            catch { };
                        }

                    }
                    else
                    {
                        PairnigPoints = PairingData(BuffGridTable2[i].Time, BuffGridTable2[i].Current, BuffGridTable2[i + 1].Time, BuffGridTable2[i + 1].Current, BuffGridTable2[i].PairingAlgoritm);
                        Item[BuffGridTable2[i].Time].Time = BuffGridTable2[i].Time;
                        Item[BuffGridTable2[i].Time].Current = BuffGridTable2[i].Current;

                        for (int j = BuffGridTable2[i].Time; j < BuffGridTable2[i + 1].Time; j++)
                        {
                            Item[j + 1].Time = PairnigPoints[k1].Time;
                            Item[j + 1].Current = PairnigPoints[k1].Current;
                            k1++;
                        }
                    }
                }
                else
                {
                    PairnigPoints = PairingData(BuffGridTable2[i].Time, BuffGridTable2[i].Current, BuffGridTable2[i + 1].Time, BuffGridTable2[i + 1].Current, BuffGridTable2[i].PairingAlgoritm);
                    Item[BuffGridTable2[i].Time].Time = BuffGridTable2[i].Time;
                    Item[BuffGridTable2[i].Time].Current = BuffGridTable2[i].Current;

                    for (int j = BuffGridTable2[i].Time; j < BuffGridTable2[i + 1].Time; j++)
                    {
                        Item[j + 1].Time = PairnigPoints[k1].Time;
                        Item[j + 1].Current = PairnigPoints[k1].Current;
                        k1++;
                    }
                }
            }

            Console.WriteLine("finish data copy " + Thread.CurrentThread.ManagedThreadId);

            Item[Item.Count - 1].Time = BuffGridTable2[BuffGridTable2.Count - 1].Time;
            Item[Item.Count - 1].Current = BuffGridTable2[BuffGridTable2.Count - 1].Current;
            
            int SelectedDataCount = Item.Count;
            for (int l = 0; l < SelectedDataCount; l++)
            {
                if (Item[l].Time == 0)
                {
                    if (tables[0].Time != 0)
                    {
                        Item.RemoveAt(l);
                        l--;
                        SelectedDataCount--;
                    }
                }
            }
            return Item;
        }
            
        

        public static void SaveData(ObservableCollection<GridTable> gridTables) // сохранение данных в файл xml 
        {
            var savedata = new ObservableCollection<BaseDataModel>();

            for (int i = 0; i < gridTables.Count; i++)
            {
                savedata.Add(new BaseDataModel { Time = gridTables[i].Time, Current = gridTables[i].Current, OscillatorOn = gridTables[i].OscillatorOn, PairingAlgoritm = gridTables[i].PairingAlgoritm, });
            }         

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Файл импульса (*.imp)|*.imp" + "|Все файлы (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter sw = File.CreateText(saveFileDialog.FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                
                    {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, savedata);
                    }
                
                //    JsonSerializer serializer = new JsonSerializer();

                //    serializer.NullValueHandling = NullValueHandling.Ignore;

                //using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                //using (JsonWriter writer = new JsonTextWriter(sw))
                //{
                //        writer.Formatting = Formatting.Indented;
                //    serializer.Serialize(writer, savedata);
                //    // {"ExpiryDate":new Date(1230375600000),"Price":0}
                //}
            }
            //string output = JsonConvert.SerializeObject(savedata, Formatting.Indented);
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Файл импульса (*.xml)|*.xml" + "|Все файлы (*.*)|*.*";
            //if (saveFileDialog.ShowDialog() == true)
            //{
            //    // File.WriteAllLines(saveFileDialog.FileName, csv);
            //    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
            //    {
            //        formatter.Serialize(fs, gridTables);

            //    }
            //}
        }

        public static ObservableCollection<GridTable> OrderThoseGroups(ObservableCollection<GridTable> orderThoseGroups)
        {
            ObservableCollection<GridTable> temp;
            temp = new ObservableCollection<GridTable>(orderThoseGroups.OrderBy(p => p.Time));
            orderThoseGroups.Clear();
            foreach (var j in temp) orderThoseGroups.Add(j);
            return orderThoseGroups;

        }

        static public ObservableCollection<BaseDataModel> PairingData(int x1, int y1, int x2, int y2, int algoritm)
        {
            ObservableCollection<BaseDataModel> Item = new ObservableCollection<BaseDataModel>();

            if (x1 != x2 && x2>x1)
            {
                for (int i = x1; i < x2; i++)
                {
                    Item.Add(new BaseDataModel { });
                }

                int n = x2 - x1;
                int[] x = new int[n];
                int j = 0;
                int[] y;

                switch (algoritm)
                {
                    case 0:

                        for (int i = 0; i < x.Length; i++)
                        {
                            x[i] = 1 + i+x1;
                        }

                        y = InterpolationAlgorithms.Linear(x1, y1, x2, y2, x);

                        for (int i = 0; i < x2 - x1; i++)
                        {
                            Item[i].Time = x[j];
                            Item[i].Current = y[j];
                            j++;
                        }

                        Item[Item.Count - 1].Time = x2;
                        Item[Item.Count - 1].Current = y2;

                        break;

                    case 1:

                        for (int i = 0; i < x.Length; i++)
                        {
                            x[i] = i + 1;
                        }

                        y = InterpolationAlgorithms.Sinus(x1, y1, x2, y2, x);

                        for (int i = 0; i < x2 - x1; i++)
                        {
                            Item[i].Time = x[j] + x1;
                            Item[i].Current = y[j];
                            j++;
                        }

                        Item[Item.Count - 1].Time = x2;
                        Item[Item.Count - 1].Current = y2;

                        break;

                    case 2:

                        for (int i = 0; i < x.Length; i++)
                        {
                            x[i] = i + 1;
                        }

                        y = InterpolationAlgorithms.Sinus1(x1, y1, x2, y2, x);

                        for (int i = 0; i < x2 - x1; i++)
                        {
                            Item[i].Time = x[j] + x1;
                            Item[i].Current = y[j];
                            j++;
                        }

                        Item[Item.Count - 1].Time = x2;
                        Item[Item.Count - 1].Current = y2;

                        break;
                }
            }
            return Item;
        }

        public static ObservableCollection<GridTable> SortData(ObservableCollection<GridTable> gridTables)
        {
            int j = 0;
            ObservableCollection<GridTable> BuffGridTables1 = new ObservableCollection<GridTable>();
            foreach (var i in gridTables)
            {
                BuffGridTables1.Add(i);
            }

            BuffGridTables1 = OrderThoseGroups(BuffGridTables1);

            foreach (var i in BuffGridTables1)
            {
                gridTables.Add(new GridTable { Time = i.Time, Current = i.Current, Id = j + 1, OscillatorOn = i.OscillatorOn, PairingAlgoritm = i.PairingAlgoritm });
                j++;
              
            }

            return gridTables;

        }
    }
}
