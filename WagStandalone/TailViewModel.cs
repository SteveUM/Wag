using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace WagLib
{

    internal sealed class TailViewModel : BaseViewModel
    {
        const int FILE_READ_DELAY = 100;

        public TailViewModel()
        {
            Running = false;
            LastErrorMessage = "";
            LogData = new ObservableCollection<LogEntry>();
            LogViewSource = CollectionViewSource.GetDefaultView(LogData);
            FilterText = "";

            LogViewSource.Filter = n => ((LogEntry)n).Entry.Contains(FilterText);
        }

        private string filterText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                LogViewSource.Filter = n => ((LogEntry)n).Entry.Contains(FilterText);
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
                    LogData = new ObservableCollection<LogEntry>();
                }
                LogData?.Clear();

                Thread t = new Thread(() =>
                {
                    Running = true;
                    TailFileAsync();
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
            set
            {
                logViewSource = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<LogEntry> logData;

        public ObservableCollection<LogEntry> LogData
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

        public bool LoadComplete { get; set; }

        private async void TailFileAsync()
        {
            //https://www.codeproject.com/Articles/7568/Tail-NET
            using (StreamReader reader = new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string line = string.Empty;

#if STANDALONE
                while ((line = reader.ReadLine()) != null)
                {
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        LogData.Add(new LogEntry() { Entry = line });
                    }));
                }
                NotifyPropertyChanged("LogData");
                NotifyPropertyChanged("LogViewSource");
#else

                // read the existing file content
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(); // Switch to main thread
                        LogData.Add(new LogEntry() { Entry = line });
                        //NotifyPropertyChanged("LogData");
                    }
                    NotifyPropertyChanged("LogData");
                });
#endif

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
#if STANDALONE
                    while ((line = reader.ReadLine()) != null)
                    {
                        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            LogData.Add(new LogEntry() { Entry = line });
                        }));
                    }
                    NotifyPropertyChanged("LogViewSource");
#else
                    while ((line = reader.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        ThreadHelper.JoinableTaskFactory.Run(async delegate
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(); // Switch to main thread
                            LogData.Add(new LogEntry() { Entry = line });
                            //NotifyPropertyChanged("LogData");
                        });
                    }
#endif
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
