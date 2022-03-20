using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using UI.ViewModels;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _activeSortColumn;
        private bool _isSortedDomains;

        private MainViewModel MainViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _activeSortColumn = string.Empty;
            _isSortedDomains = false;

            var filterAction = new Func<IEnumerable<FileSystemInfo>, string, IEnumerable<FileSystemInfo>>(
                (fileSystemInfos, name) => fileSystemInfos.OrderBy(fileSystemInfo => fileSystemInfo.GetType().GetProperty("Name").GetValue(fileSystemInfo)));

            MainViewModel = new MainViewModel(AppContext.BaseDirectory, filterAction, columnName: "Name");

            SetEvents();

            DataContext = MainViewModel;
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((MainViewModel.SelectedItem.Attributes == FileAttributes.Directory) ||
                (MainViewModel.SelectedItem.Attributes == (FileAttributes.Directory | FileAttributes.Hidden)))
            {
                MainViewModel.CurrentDirectory = MainViewModel.SelectedItem.FullName;
                MainViewModel.FileSystemVisitor = new FileSystemVisitor(MainViewModel.SelectedItem.FullName);

                SetEvents();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var previosDirectory = new DirectoryInfo(MainViewModel.CurrentDirectory).Parent.FullName;

            MainViewModel.FileSystemVisitor = new FileSystemVisitor(previosDirectory);
            MainViewModel.CurrentDirectory = previosDirectory;

            SetEvents();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.SelectedItem = MainViewModel.FileSystemVisitor.SearchFileSystemComponentByName(MainViewModel.SearchText);
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var header = (GridViewColumnHeader)sender;
            var columnName = header.Column.Header.ToString();

            Func<IEnumerable<FileSystemInfo>, string, IEnumerable<FileSystemInfo>> filterAction;

            if (columnName != _activeSortColumn)
            {
                filterAction = new Func<IEnumerable<FileSystemInfo>, string, IEnumerable<FileSystemInfo>>(
                (fileSystemInfos, name) => fileSystemInfos.OrderBy(fileSystemInfo => fileSystemInfo.GetType().GetProperty(columnName).GetValue(fileSystemInfo)));

                MainViewModel.FileSystemVisitor.SortFunction = filterAction;

                MainViewModel.FileSystemVisitor.Sort(columnName);

                _isSortedDomains = true;
                _activeSortColumn = columnName;

            }
            else
            {
                if (_isSortedDomains)
                {
                    filterAction = new Func<IEnumerable<FileSystemInfo>, string, IEnumerable<FileSystemInfo>>(
                        (fileSystemInfos, name) => fileSystemInfos.OrderByDescending(fileSystemInfo => fileSystemInfo.GetType().GetProperty(columnName).GetValue(fileSystemInfo)));
                }
                else
                {
                    filterAction = new Func<IEnumerable<FileSystemInfo>, string, IEnumerable<FileSystemInfo>>(
                        (fileSystemInfos, name) => fileSystemInfos.OrderBy(fileSystemInfo => fileSystemInfo.GetType().GetProperty(columnName).GetValue(fileSystemInfo)));
                }

                MainViewModel.FileSystemVisitor.SortFunction = filterAction;

                MainViewModel.FileSystemVisitor.Sort(columnName);

                _isSortedDomains = !_isSortedDomains;
            }
        }

        private void SetEvents()
        {
            MainViewModel.FileSystemVisitor.StartSort = new Action(() => MainViewModel.Logs = MainViewModel.Logs + "> Start Sort\n");
            MainViewModel.FileSystemVisitor.FinishSort = new Action(() => MainViewModel.Logs = MainViewModel.Logs + "> Finish Sort\n");
            MainViewModel.FileSystemVisitor.FileSystemComponentFound = new Action(() => MainViewModel.Logs = MainViewModel.Logs + "> File System Component Founded\n");
            MainViewModel.FileSystemVisitor.FileSystemComponentNotFound = new Action(() => MainViewModel.Logs = MainViewModel.Logs + "> File System Component Not Founded\n");
        }
    }
}
