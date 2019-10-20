﻿using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;

namespace WagStandalone
{
    internal sealed class WagViewModel : BaseViewModel
    {
        public WagViewModel()
        {
            TailViewModel = new TailViewModel();
            ButtonBackground = Colors.White;
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

        public Color ButtonBackground { get; set; }

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
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            DialogResult result;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileName = ofd.FileName;
            }
        }





    }
}
