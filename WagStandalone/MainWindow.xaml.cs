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

namespace WagLib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal WagViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new WagViewModel();
            ViewModel.TailViewModel.LogData.CollectionChanged += LogData_CollectionChanged;
            DataContext = ViewModel;
        }

        private void LogData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var border = VisualTreeHelper.GetChild(logFileDataTable, 0) as Decorator;
            if (border != null)
            {
                var scroll = border.Child as ScrollViewer;
                if (scroll != null)
                {
                    scroll.ScrollToEnd();
                }
            }
        }

        private void ToggleLogging(object sender, RoutedEventArgs e)
        {
            ViewModel.ToggleLogging();
        }

        private void SelectFileClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectLogFile();
        }

        private void CollectionViewSource_Filter(object sender, System.Windows.Data.FilterEventArgs e)
        {

        }

        private void filterTextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.TailViewModel.FilterText = ViewModel.Highlight ? string.Empty : xfilterTextBox.Text;
            LogEntry.SearchString = ViewModel.Highlight ? xfilterTextBox.Text : string.Empty;
        }

        private void ClearFilterClick(object sender, RoutedEventArgs e)
        {
            xfilterTextBox.Text = string.Empty;
            ViewModel.TailViewModel.FilterText = string.Empty;
            LogEntry.SearchString = string.Empty;
        }

        private void XfilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            filterTextChanged(sender, null);
        }

        private void XfilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Tab)
            {
                filterTextChanged(sender, null);
            }
        }
    }
}
