using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private FileSystemVisitor _fileSystemVisitor;
        private FileSystemInfo _selectedItem;

        private string _logs;


        public MainViewModel(string path)
        {
            FileSystemVisitor = new FileSystemVisitor(path);
            CurrentDirectory = path;
        }

        public MainViewModel(string path, Func<IEnumerable<FileSystemInfo>, string, IEnumerable<FileSystemInfo>> filterAction, string columnName)
        {
            FileSystemVisitor = new FileSystemVisitor(path, filterAction, columnName);
            CurrentDirectory = path;
        }

        public string CurrentDirectory { get; set; }

        public string SearchText { get; set; }

        public string Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged("Logs");
            }
        }

        public FileSystemVisitor FileSystemVisitor
        {
            get => _fileSystemVisitor;
            set
            {
                _fileSystemVisitor = value;
                OnPropertyChanged("FileSystemVisitor");
            }
        }

        public FileSystemInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void SortFileSystemVisitor(string columnName)
        {
            FileSystemVisitor.Sort(columnName);
            OnPropertyChanged("FileSystemVisitor");
        }
    }
}
