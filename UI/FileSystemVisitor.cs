using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UI
{
    public class FileSystemVisitor : IEnumerable<FileSystemInfo>, INotifyCollectionChanged
    {
        private IEnumerable<FileSystemInfo> _fileSystemInfos;

        public FileSystemVisitor(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            _fileSystemInfos = directoryInfo.GetFileSystemInfos();
        }

        public FileSystemVisitor(string path, Func<IEnumerable<FileSystemInfo>, string, IEnumerable<FileSystemInfo>> filterAction, string columnName)
        {
            var directoryInfo = new DirectoryInfo(path);

            _fileSystemInfos = directoryInfo.GetFileSystemInfos();
            _fileSystemInfos = filterAction.Invoke(_fileSystemInfos, columnName);
        }

        public Func<IEnumerable<FileSystemInfo>, string, IEnumerable<FileSystemInfo>> SortFunction { get; set; }

        public Action StartSort { get; set; }

        public Action FinishSort { get; set; }

        public Action FileSystemComponentFound { get; set; }

        public Action FileSystemComponentNotFound { get; set; }

        public int Length => _fileSystemInfos.Count();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
        }

        public void Sort(string columnName)
        {
            StartSort?.Invoke();
            _fileSystemInfos = SortFunction?.Invoke(_fileSystemInfos, columnName);
            FinishSort?.Invoke();

            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public FileSystemInfo SearchFileSystemComponentByName(string name)
        {
            var fileSystemComponent = _fileSystemInfos.FirstOrDefault(fileSystemComponent => fileSystemComponent.Name == name);

            if (fileSystemComponent == null)
            {
                FileSystemComponentNotFound?.Invoke();
            }
            else
            {
                FileSystemComponentFound?.Invoke();
            }

            return fileSystemComponent;
        }

        public IEnumerator<FileSystemInfo> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var fileSystemInfo in _fileSystemInfos)
            {
                yield return fileSystemInfo;
            }
        }
    }
}
