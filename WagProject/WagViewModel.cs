using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;

namespace WagProject
{
    internal sealed class WagViewModel : BaseViewModel
    {
        public WagViewModel()
        {
            TailViewModel = new TailViewModel();
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




    }
}
