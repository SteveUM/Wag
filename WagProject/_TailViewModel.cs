using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Windows.Data;

namespace WagProject
{

    internal class LogEntries : ObservableCollection<_LogEntry>
    {

    }


    internal sealed class _TailViewModel : _BaseViewModel
    {
        const int FILE_READ_DELAY = 100;

        public _TailViewModel()
        {
            Running = false;
            LastErrorMessage = "";
            LogData = new ObservableCollection<object>();
            LogViewSource = CollectionViewSource.GetDefaultView(LogData);
            FilterText = "";

            LogViewSource.Filter = n => ((_LogEntry)n).Entry.Contains(FilterText);
        }

        private string filterText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                LogViewSource.Filter = n => ((_LogEntry)n).Entry.Contains(FilterText);
            }
        }

        public string FileName { get; set; }

        private bool running;

        public bool Running
        {
            get { return running; }
            set
            {
                running = value;
                NotifyPropertyChanged();
            }
        }

        private string lastErrorMessage;

        public string LastErrorMessage
        {
            get { return lastErrorMessage; }
            set { lastErrorMessage = value; NotifyPropertyChanged(); }
        }

        public void StartTail()
        {
            if (!File.Exists(FileName))
            {
                LastErrorMessage = $"File: {FileName} does not exist";
            }
            else
            {
                LastErrorMessage = $"Tailing file {Path.GetFileName(FileName)}";
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
            LastErrorMessage = "";
            Running = false;
        }

        private ICollectionView logViewSource;

        public ICollectionView LogViewSource
        {
            get { return logViewSource; }
            set { logViewSource = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<object> logData;

        public ObservableCollection<object> LogData
        {
            get
            {
                return logData;
            }
            set
            {
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


            //https://www.codeproject.com/Articles/7568/Tail-NET
            using (StreamReader reader = new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string line = string.Empty;

                // read the existing file content
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(); // Switch to main thread
                        LogData.Add(new _LogEntry() { Entry = line });
                        //NotifyPropertyChanged("LogData");
                    }
                    NotifyPropertyChanged("LogData");
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
                            LogData.Add(new _LogEntry() { Entry = line });
                            //NotifyPropertyChanged("LogData");
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
