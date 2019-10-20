using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Threading;

namespace WagProject
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
            }
        }
    }


    internal sealed class TailViewModel : BaseViewModel
    {
        const int FILE_READ_DELAY = 100;

        public TailViewModel()
        {
            Running = false;
            LastErrorMessage = "";
            LogData = new ObservableCollection<object>();
        }

        public string FileName { get; set; }


        private bool running;

        public bool Running
        {
            get { return running; }
            set {
                running = value;
                NotifyPropertyChanged();
            }
        }

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
                    LogData = new ObservableCollection<object>();
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

        private ObservableCollection<object> logData;

        public ObservableCollection<object> LogData
        {
            get {
                return logData;
            }
            set {
                logData = value;
                NotifyPropertyChanged();
            }
        }

        private void TailFile()
        {
            //test
            //while (Running)
            //{
            //    Thread.Sleep(2000);
            //    LogData.Rows.Add(LogData.NewRow()["data"] = "asefasdf");
            //    //LogData = LogData;
            //    //NotifyPropertyChanged("LogData");
            //}
            //return;

            

            //FileInfo fi = new FileInfo(FileName);
            //if(fi.Extension.ToLower() == "csv")
            //{
                

            //}


            //https://www.codeproject.com/Articles/7568/Tail-NET
            using (StreamReader reader = new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string line = string.Empty;

                // read the existing file content
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(); // Switch to main thread
                        LogData.Add(new LogDataCollection() { LogEntry = line });
                        NotifyPropertyChanged("LogData");
                    }
                });


                long EOFPosition = reader.BaseStream.Length;

                // tail the file
                while (Running)
                {
                    Thread.Sleep(FILE_READ_DELAY);

                    if (reader.BaseStream.Length == EOFPosition)
                    {
                        continue;
                    }

                    reader.BaseStream.Seek(EOFPosition, SeekOrigin.Begin);

                    while ((line = reader.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        ThreadHelper.JoinableTaskFactory.Run(async delegate
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(); // Switch to main thread
                            LogData.Add(new LogDataCollection() { LogEntry = line });
                            NotifyPropertyChanged("LogData");
                        });
                    }

                    EOFPosition = reader.BaseStream.Position;
                }
            }
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
            {
                expandoDict[propertyName] = propertyValue;
            }
            else
            {
                expandoDict.Add(propertyName, propertyValue);
            }
        }
    }
}
