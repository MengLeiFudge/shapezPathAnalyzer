using System;
using System.Windows;
using Microsoft.Win32;

namespace shapezAnalyzer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly MainWindowViewModel ViewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            BaseGrid.DataContext = ViewModel;
            ViewModel.AutoLoadDb();
            TextBoxExpression.Focus();
        }

        private void Button_ChangeDB_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "数据库文件(*.csv)|*.csv",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (dialog.ShowDialog() == false)
            {
                return;
            }
            var dbPath = dialog.FileName;
            ViewModel.ChangeDb(dbPath);
        }
    }
}