using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace WagStandalone
{

    internal class LogDataCollection : BaseViewModel
    {
        string logEntry;

        public string LogEntry
        {
            get {
                return logEntry;
            }
            set {
                logEntry = value;
                NotifyPropertyChanged();
            }
        }
    }


    internal sealed class TailViewModel : BaseViewModel
    {
        public TailViewModel()
        {
            Running = false;
            LastErrorMessage = "";
            //LogData = new ObservableCollection<LogDataCollection>();    //BuildTable();
        }

        public string FileName { get; set; }

        public bool Running { get; private set; }

        public string LastErrorMessage { get; set; }

        public void StartTail()
        {
            if (!File.Exists(FileName))
            {
                LastErrorMessage = "File does not exist";
            }
            else
            {
                if (LogData == null)
                {
                    LogData = new ObservableCollection<LogDataCollection>();
                }
                LogData?.Clear();

                Thread t = new Thread(() =>
                {
                    Running = true;
                    TailFile();
                });
                t.Start();
            }
        }

        public void StopTail()
        {
            Running = false;
        }

        //private DataTable logData;

        //public DataTable LogData
        //{
        //    get { return logData; }
        //    set {
        //        logData = value;
        //       // NotifyPropertyChanged();
        //    }
        //}
        ObservableCollection<LogDataCollection> logData;

        public ObservableCollection<LogDataCollection> LogData
        {
            get {
                return logData;
            }
            set {
                logData = value;
                NotifyPropertyChanged();
            }
        }


        private DataTable BuildTable()
        {
            DataTable dt = new DataTable("log");
            DataColumn column = new DataColumn("data");
            column.DataType = Type.GetType("System.String");
            column.Caption = "Data";
            column.DefaultValue = "";
            dt.Columns.Add(column);

            dt.Rows.Add(dt.NewRow()["data"] = "test row...");

            return dt;
        }

        private void TailFile()
        {

            //while (Running)
            //{
            //    Thread.Sleep(2000);
            //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        LogData.Add(new LogDataCollection() { LogEntry = "asdfasdfasdfasdf..." });
            //        NotifyPropertyChanged("LogData");
            //    });

            //    //LogData.Rows.Add(LogData.NewRow()["data"] = "asefasdf");
            //    //LogData = LogData;
            //    //NotifyPropertyChanged("LogData");

            //}
            //return;


            //https://www.codeproject.com/Articles/7568/Tail-NET
            using (StreamReader reader = new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                //start at the end of the file
                long lastMaxOffset = reader.BaseStream.Length;

                while (Running)
                {
                    Thread.Sleep(100);


                    //if the file size has not changed, idle
                    if (reader.BaseStream.Length == lastMaxOffset)
                        continue;

                    //seek to the last max offset
                    reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                    //read out of the file until the EOF
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            LogData.Add(new LogDataCollection() { LogEntry =line });
                            NotifyPropertyChanged("LogData");
                        });
                    }

                    //update the last max offset
                    lastMaxOffset = reader.BaseStream.Position;
                }
            }
        }


    }
}
