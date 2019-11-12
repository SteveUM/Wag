using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;

namespace WagProject
{
    internal sealed class _WagViewModel : _BaseViewModel
    {
        public _WagViewModel()
        {
            TailViewModel = new _TailViewModel();
            IsLogging = false;
        }


        private _TailViewModel tailViewModel;

        public _TailViewModel TailViewModel
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

        internal void SelectLogFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult result;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileName = ofd.FileName;
            }
        }

        internal void ToggleLogging()
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



    }
}
