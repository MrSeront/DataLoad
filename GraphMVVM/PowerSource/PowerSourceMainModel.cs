using GraphMVVM.DataGraph;
using GraphMVVM.Model;
using GraphMVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphMVVM.PowerSource
{
    class PowerSourceMainModel<T> : VM where T : IData, new()
    {

        private ObservableCollection<T> PrepareData = new ObservableCollection<T>();
        private ObservableCollection<T> DataInPowerSource = new ObservableCollection<T>();
        private ObservableCollection<T> BuffData = new ObservableCollection<T>();
        private ObservableCollection<T> ProgressData = new ObservableCollection<T>();
        private T Empty = new T();
        private object locker = true;
        private bool DataClearBool;
        private bool threadStop = false;
        private bool sendStop = true;
        private int ProgressValue { get; set; }
        private string Status { get; set; }
        private SerialPort SerialPort { get; set; } = new SerialPort();

        public void ChangePort(SerialPort serialPort)
        {
            if (serialPort != null)
                SerialPort = serialPort;
        }

        private void OpenPort()
        {
            try
            {
                if (!SerialPort.IsOpen)
                {
                    SerialPort.Open();
                }
                else
                {
                    Console.WriteLine("Порт уже открыт");
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine("ERROR: невозможно открыть порт:" + ee.ToString());
                return;
            }
            //  Console.WriteLine("Порт " + SerialPort.PortName + " открыт");
        }

        private void ClosePort()
        {
            try
            {
                if (SerialPort.IsOpen)
                {
                    SerialPort.Close();
                }
                else
                {
                    Console.WriteLine("Порт уже закрыт");
                }

            }
            catch (Exception ee)
            {
                Console.WriteLine("ERROR: невозможно закрыть порт:" + ee.ToString());
                return;
            }
            // Console.WriteLine("Порт " + SerialPort.PortName + " закрыт");
        }

        public void SendData(ObservableCollection<T> datas)
        {
            lock (locker)
            {

                Console.WriteLine("подготовка данных к отправке " + Thread.CurrentThread.ManagedThreadId);

                if (DataClearBool)
                {
                    DataInPowerSource.Clear();
                    DataClearBool = false;

                }
                PrepareData.Clear();
                BuffData.Clear();

                for (int i = 0; i < datas.Count; i++)
                {
                    BuffData.Add(datas[i]);
                }

                if (DataInPowerSource.Count > 1)
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        if (i >= DataInPowerSource.Count)
                        {
                            DataInPowerSource.Add(Empty);
                        }

                        if ((DataInPowerSource[i].Time != datas[i].Time) || (DataInPowerSource[i].Current != datas[i].Current) || (DataInPowerSource[i].OscillatorOn != datas[i].OscillatorOn))
                        {
                            PrepareData.Add(datas[i]);
                        }


                    }
                    if (datas.Count < DataInPowerSource.Count)
                    {
                        for (int i = datas.Count; i < DataInPowerSource.Count; i++)
                        {
                            PrepareData.Add(new T { Time = DataInPowerSource[i].Time, Current = 0, OscillatorOn = false });
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        PrepareData.Add(datas[i]);
                    }
                }
                Console.WriteLine("Данные подготовленны к отправке " + Thread.CurrentThread.ManagedThreadId);


                // CreateAndSendCsvFile(PrepareData);
                SendFile();

                DataInPowerSource.Clear();

                foreach (T i in BuffData)
                {
                    DataInPowerSource.Add(i);
                }
            }
        }


        public void CreateAndSendCsvFile(string path, bool threadSleep)
        {
            ProgressValue = 0;
            if (path != "")
            {

                //List<int> datafromport = new List<int>();

                string[] csv = File.ReadAllLines(path);

                // 0.14  SerialPort.WriteLine(l); while (datafromport != '\r');
                //0.15  char[] ar = l.ToCharArray();
                //0.25 
                //byte[] sen = new byte[ar.Length];
                //for (int i = 0; i < ar.Length; i++)
                //{
                //    sen[i] = Convert.ToByte(ar[i]);
                //}
                //SerialPort.Write(sen, 0, sen.Length);

                OpenPort();
                int j = 0;
                byte[] send = new byte[1];
                int row = 0;
                int k = 0;
                //int readdata;
                //int count;
                int[] buff = new int[2];
                for (int i = 0; i < csv.Length; i++)// по строчно читает файл
                {
                    string l = csv[i];

                    buff[0] = 0;
                    buff[1] = 0;

                    char[] ar = l.ToCharArray(); // преобазует строку в массив чаров
                   
                    send[0] = 0x0D;
                   
                    //byte[] sen = new byte[ar.Length + 1];
                    //for (int i = 0; i < ar.Length; i++)
                    //{
                    //    sen[i] = Convert.ToByte(ar[i]); // заполняет массив байтами
                    //}

                    //sen[sen.Length - 1] = 0x0D;
                    //Console.WriteLine("Начало отправки данных");
                    //for (int i = 0; i < sen.Length; i++)
                    //{
                    //    count = 0;
                    //    SerialPort.Write(sen, i, 1);
                        
                    //    do
                    //    {
                    //        readdata = SerialPort.ReadByte();
                    //        count +=1;
                    //        Console.Write(Convert.ToChar(readdata)+"(" + count +")");
                    //    } while (readdata!=sen[i]);
                       
                    //}
                    // Console.WriteLine("Отправка данных закончена");
                    SerialPort.Write(ar, 0, ar.Length);
                    SerialPort.Write(send, 0, 1);

                    // byte[] datafromport = new byte
                    //Console.WriteLine("Считываение ответа");

                    do
                    {
                        buff[j] = SerialPort.ReadByte();
                       // Console.Write(Convert.ToChar(buff[j]));
                        j = (j + 1) & 1;

                    }
                    while (buff[(j - 1) & 1] != 0x0A || buff[j] != 0x0D);
                   // Console.WriteLine("Переход к следуюзим данным");
                    if (threadSleep)
                    {
                        //Console.WriteLine("ThreadSleep");
                        Thread.Sleep(1);
                    }
                    

                    //SerialPort.Write(l); // отправляет строку вида W...;...;...


                    //for (int i = 0; i < ar.Length; i++)
                    //{

                    //        SerialPort.Write(Convert.ToString(ar[i]));

                    //}
                    //Thread.Sleep(500);
                    row++;
                    ProgressValue = row * 100 / csv.Length;
                    k++;

                }
            }
            else
            {

            }
            
        }







        private void SendFile()           // отправка файла в COM-порт
        {
            DateTime dateTimeStart = DateTime.Now;
            OpenPort();
            string data2, data3;
            string readrow;
            int data1;
            byte it;                    // Счетчик отмены
            byte[] send = new byte[1];
            int row = 0;
            byte highbyte;
            byte lowbyte;
            char datachar;
            int wc, hbc, lbc, crc, cdc;
            //sendStop = false;

            //if (flcb == false)           // проверка быстрой загрузки
            // {
            //int k = 0;
            //foreach (var l in csv)
            //{

            //    readrow = "";

            //    char[] ar = l.ToCharArray();

            //    for (int i = 0; i < ar.Length; i++)
            //    {

            //        it = 0;
            //        SerialPort.Write(Convert.ToString(ar[i]));
            //        do
            //        {

            //            data1 = SerialPort.ReadByte();
            //            it++;
            //            //Thread.Sleep(1);
            //        }
            //        while (Convert.ToChar(data1) != Convert.ToChar(ar[i]));//&& it != 100
            //       //readrow += Convert.ToChar(data1);
            //    }
            //    Console.WriteLine(readrow);
            //    send[0] = Convert.ToByte('\r');
            //    SerialPort.Write(send, 0, 1);
            //    Thread.Sleep(10);

            //    data2 = SerialPort.ReadLine();

            //    data3 = SerialPort.ReadLine();
            //    Console.WriteLine(data3);
            //    if (PrepareData[0].Time == 0)
            //    {
            //        ProgressData.Add(new T { Time = PrepareData[k].Time, Current = PrepareData[k].Current });
            //    }
            //    else if (ProgressData.Count - 1 < PrepareData[k].Time)
            //    {
            //        ProgressData.Add(new T { Time = PrepareData[k].Time, Current = PrepareData[k].Current });
            //    }
            //    else
            //    {
            //        ProgressData[PrepareData[k].Time].Current = PrepareData[k].Current;
            //    }
            //    row++;
            //    ProgressValue = row * 100 / PrepareData.Count;
            //    k++;
            //}
            // }
            //else
            //{

            byte[] w = new byte[1];
            byte[] hb = new byte[1];
            byte[] lb = new byte[1];
            byte[] cr = new byte[1];
            byte[] cd = new byte[1];
            w[0] = 0;
            hb[0] = 0;
            lb[0] = 0;
            cr[0] = 0;
            cd[0] = 0;

            for (int i = 0; i < PrepareData.Count; i++)
            {
                if (threadStop)
                {
                    Console.WriteLine("Отправка остановлена " + Thread.CurrentThread.ManagedThreadId);
                    ClosePort();
                    sendStop = true;
                    Thread.Sleep(100);
                    sendStop = false;

                    break;
                };

                highbyte = Convert.ToByte((PrepareData[i].Time & 0xFF00) >> 8);
                lowbyte = Convert.ToByte(PrepareData[i].Time & 0x00FF);
                w[0] = 119;
                hb[0] = highbyte;
                lb[0] = lowbyte;
                cr[0] = Convert.ToByte(Math.Abs(PrepareData[i].Current));

                if (PrepareData[i].Current < 0)
                {
                    if (PrepareData[i].OscillatorOn == true)
                    {
                        cd[0] = 131;
                    }
                    else
                    {
                        cd[0] = 3;
                    }
                }
                else
                {
                    if (PrepareData[i].OscillatorOn == true)
                    {
                        cd[0] = 69;
                    }
                    else
                    {
                        cd[0] = 5;
                    }
                }
                wc = Sendfastcom(w);
                hbc = Sendfastcom(hb);
                lbc = Sendfastcom(lb);
                crc = Sendfastcom(cr);
                cdc = Sendfastcom(cd);
                data1 = SerialPort.ReadByte();
                Console.WriteLine(Convert.ToChar(wc) + " " + Convert.ToString(PrepareData[i].Time) + " " + crc + " " + cdc + " " + Convert.ToChar(data1));

                if (PrepareData[0].Time == 0)
                {
                    ProgressData.Add(new T { Time = PrepareData[i].Time, Current = PrepareData[i].Current });
                }
                else if (ProgressData.Count - 1 < PrepareData[i].Time)
                {
                    ProgressData.Add(new T { Time = PrepareData[i].Time, Current = PrepareData[i].Current });
                }
                else
                {
                    ProgressData[PrepareData[i].Time].Current = PrepareData[i].Current;
                }
                row++;
                ProgressValue = row * 100 / PrepareData.Count;

                //PgbText.Text = Convert.ToString(row * 100 / csv.Length) + "%";
            }

            ClosePort();
            DateTime dateTimeFinish = DateTime.Now;
            var worktime = dateTimeFinish - dateTimeStart;
            Console.WriteLine("Время загрузки " + worktime);
            sendStop = true;
            Thread.Sleep(250);
            ProgressValue = 0;

        }
        private int Sendfastcom(byte[] w)
        {
            int data1 = 0;
            SerialPort.Write(w, 0, 1);
            do
            {
                data1 = SerialPort.ReadByte();
                Thread.Sleep(1);
            }
            while (data1 != w[0]);
            return data1;
        }

        //private byte[] Sendfastcom(byte[] w)
        //{
        //    byte[] data1 = new byte[5];
        //    for (int i = 0; i < w.Length; i++)
        //    {
        //        SerialPort.Write(w, i, 1);
        //        do
        //        {
        //            data1[i] = (byte)SerialPort.ReadByte();
        //            Thread.Sleep(1);
        //        }
        //        while (data1[i] != w[0]);



        //    }


        //    return data1;
        //}

        public ObservableCollection<T> GetProgressData()
        {
            return ProgressData;
        }
        public int GetProgress()
        {
            return ProgressValue;
        }
        public void ClearData()
        {
            //Console.WriteLine("Начало отчистки " + Thread.CurrentThread.ManagedThreadId);

            bool dataClearEnd = false;

            threadStop = true;

            do
            {
                try
                {
                    if (sendStop == true)
                    {


                        threadStop = false;

                        OpenPort();

                        int readedDataFromPort = 0;

                        byte[] send = new byte[] { 67, 13 }; // 67 == 'C', 13 == '\r'

                        //SerialPort.Write(send, 0, 2);

                        //do
                        //{
                        //    readedDataFromPort = SerialPort.ReadByte();
                        //    Thread.Sleep(1);
                        //}
                        //while (readedDataFromPort != 79); // 79 == 'O'

                        ClosePort();

                        //Console.WriteLine("Память отчищена " + Thread.CurrentThread.ManagedThreadId);
                        DataClearBool = true;
                        dataClearEnd = true;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Ошибка очистки памяти СОМ-порта");
                }
            }
            while (!dataClearEnd);

            // Console.WriteLine("Конец отчистки " + Thread.CurrentThread.ManagedThreadId);

        }
        public void Welding() // Метод отсылает комманду "А" ("Сварка") в ком порт и дожидается ответа ОК
        {

            OpenPort();
                      
            byte[] send = new byte[] { 65, 13 }; // 65 == 'A', 13 == '\n'

            SerialPort.Write(send, 0, 2);
                   
            
            ClosePort();
        }

    }
}
