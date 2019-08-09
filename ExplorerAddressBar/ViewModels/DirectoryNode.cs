using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ExplorerAddressBar.ViewModels
{
    class DirectoryItem
    {
        public string FullPath { get; }
        public string Name { get; }

        public DirectoryItem(string path)
        {
            FullPath = path;
            Name = path.Split(Path.DirectorySeparatorChar).Last();
        }
    }

    class DirectoryNode
    {
        public string FullPath { get; }

        public ObservableCollection<DirectoryItem> ChildDirectoryNames { get; private set; }

        public IList<string> ChildFileNames { get; private set; }

        public DirectoryNode(string path)
        {
            FullPath = path;
            Update();
        }

        // ディレクトリを更新
        public void Update()
        {
            var path = FullPath;
            ChildDirectoryNames = new ObservableCollection<DirectoryItem>(GetChildDirectories(path));
            ChildFileNames = GetChildFiles(path).ToList();
        }

        // 引数pathディレクトリ内の子ディレクトリを返す
        private static IEnumerable<DirectoryItem> GetChildDirectories(string basePath)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(basePath)) return Enumerable.Empty<DirectoryItem>();

            return Directory.GetDirectories(basePath, "*", SearchOption.TopDirectoryOnly)
                .Select(p => p.Replace(basePath, ""))
                //.Select(p => p.TrimStart(Path.DirectorySeparatorChar))  // ディレクトリ先頭の\を削除
                .Select(p => Path.Combine(basePath, p))
                .Select(p => new DirectoryItem(p));
        }

        // 引数pathディレクトリ内の子ファイルを返す
        private static IEnumerable<string> GetChildFiles(string path)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(path)) return Enumerable.Empty<string>();

            return Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly)
                .Select(p => Path.GetFileName(p));
        }

    }
}
