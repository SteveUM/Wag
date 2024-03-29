﻿namespace WagProject
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using System.Windows.Input;
    using System.Windows.Media;
    using WagLib;

    /// <summary>
    /// Interaction logic for WagControl.
    /// </summary>
    public partial class WagControl : UserControl
    {
        //internal WagViewModel ViewModel { get; set; }
        internal WagViewModel ViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WagControl"/> class.
        /// </summary>
        public WagControl()
        {  
            this.InitializeComponent();
            ViewModel = new WagViewModel();
            ViewModel.TailViewModel.LogData.CollectionChanged += LogData_CollectionChanged;
            DataContext = ViewModel;
        }

        private void LogData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var border = VisualTreeHelper.GetChild(logFileDataTable, 0) as Decorator;
            if(border != null)
            {
                var scroll = border.Child as ScrollViewer;
                if(scroll != null)
                {
                    scroll.ScrollToEnd();
                }
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "Wag");
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
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                filterTextChanged(sender, null);
            }
        }
    }
}