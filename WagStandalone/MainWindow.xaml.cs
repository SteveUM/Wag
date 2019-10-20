using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WagStandalone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new WagViewModel();
            DataContext = ViewModel;
        }

        internal WagViewModel ViewModel { get; set; }

        private void ToggleLogging(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.TailViewModel.Running)
            {
                ViewModel.TailViewModel.StartTail();
            }
            else
            {
                ViewModel.TailViewModel.StopTail();
            }
        }

        private void SelectFileClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectLogFile();
        }

    }
}
