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
        public string BasePath { get; }

        public IList<DirectoryItem> ChildDirectoryNames { get; private set; }

        public IList<string> ChildFileNames { get; private set; }

        public DirectoryNode(string path)
        {
            BasePath = path;
            Update();
        }

        // ディレクトリを更新
        public void Update()
        {
            var path = BasePath;
            ChildDirectoryNames = GetChildDirectories(path).ToList();
            ChildFileNames = GetChildFiles(path).ToList();
        }

        // 引数pathディレクトリ内の子ディレクトリを返す
        private static IEnumerable<DirectoryItem> GetChildDirectories(string basePath)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(basePath)) return Enumerable.Empty<DirectoryItem>();

            return Directory.GetDirectories(basePath, "*", SearchOption.TopDirectoryOnly)
                .Select(p => new DirectoryItem(p));
        }

        // 引数pathディレクトリ内の子ファイルを返す
        private static IEnumerable<string> GetChildFiles(string basePath)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(basePath)) return Enumerable.Empty<string>();

            return Directory.GetFiles(basePath, "*", SearchOption.TopDirectoryOnly)
                .Select(p => Path.GetFileName(p));
        }

    }
}
