﻿using System.Collections;
using System.Windows.Forms;

namespace WagLib
{
    public sealed class WagViewModel : BaseViewModel
    {
        public WagViewModel()
        {
            TailViewModel = new TailViewModel();
            IsLogging = false;
        }

        private TailViewModel tailViewModel;

        public TailViewModel TailViewModel
        {
            get { return tailViewModel; }
            set {
                tailViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public bool EnableLogging
        {
            get { return !TailViewModel.Running; }
            set { NotifyPropertyChanged(); }
        }

        public bool DisableLogging
        {
            get { return TailViewModel.Running; }
            set { NotifyPropertyChanged(); }
        }

        private bool isLogging;
        public bool IsLogging
        {
            get {
                return isLogging;
            }
            set {
                if (isLogging == value)
                {
                    return;
                }

                isLogging = value;
                NotifyPropertyChanged();
            }
        }

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value;
                TailViewModel.FileName = value;
                NotifyPropertyChanged(); }
        }

        public void SelectLogFile()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            DialogResult result;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileName = ofd.FileName;
            }
        }

        public void ToggleLogging()
        {
            if (!TailViewModel.Running)
            {
                TailViewModel.StartTail();
                IsLogging = true;
            }
            else
            {
                TailViewModel.StopTail();
                IsLogging = false;
            }
        }

        private bool highlight;

        public bool Highlight
        {
            get { return highlight; }
            set { highlight = value; NotifyPropertyChanged(); }
        }

        private string filterText;

        public string FilterText
        {
            get { return filterText; }
            set { filterText = value; NotifyPropertyChanged(); }
        }

        private Queue filterHistory = new Queue(10);

        public Queue FilterHistory
        {
            get { return filterHistory; }
            set { filterHistory = value; }
        }

    }
}
