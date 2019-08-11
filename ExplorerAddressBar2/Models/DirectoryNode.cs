using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExplorerAddressBar2.Models
{
    class DirectoryNode : DirectoryItem
    {
        // 子ディレクトリのリスト
        public IList<DirectoryItem> ChildDirectoryNames { get; private set; }

        // 子ファイルのリスト
        public IList<string> ChildFileNames { get; private set; }

        public DirectoryNode(string path) : base(path)
        {
            Update();
        }

        // ディレクトリを更新
        public void Update()
        {
            var path = FullPath;
            if (string.IsNullOrEmpty(path)) throw new NullReferenceException(nameof(path));

            ChildDirectoryNames = GetChildDirectoryItems(path).ToList();
            ChildFileNames = GetChildFileNames(path).ToList();
        }

        // 引数pathディレクトリ内の子ディレクトリを返す
        public static IEnumerable<DirectoryItem> GetChildDirectoryItems(string basePath)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(basePath)) yield break;

            IEnumerable<DirectoryItem> items;
            try
            {
                items = Directory.GetDirectories(basePath, "*", SearchOption.TopDirectoryOnly)
                    .Select(p => new DirectoryItem(p));
            }
            catch (UnauthorizedAccessException)
            {
                yield break;   // アクセス権のないフォルダにアクセスした場合は無視
            }

            foreach (var item in items) yield return item;
        }

        // 引数pathディレクトリ内の子ディレクトリを返す
        public static IEnumerable<string> GetChildDirectoriesName(string basePath)
        {
            if (string.IsNullOrEmpty(basePath)) yield break;
            foreach (var dir in GetChildDirectoryItems(basePath))
            {
                yield return GetDirectoryName(dir.FullPath);
            }
        }

        // 引数pathディレクトリ内の子ファイルを返す
        private static IEnumerable<string> GetChildFileNames(string basePath)
        {
            if (string.IsNullOrEmpty(basePath)) throw new NullReferenceException(nameof(basePath));
            var empty = Enumerable.Empty<string>();

            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(basePath)) return empty;

            try
            {
                return Directory.GetFiles(basePath, "*", SearchOption.TopDirectoryOnly)
                    .Select(p => Path.GetFileName(p));
            }
            catch (UnauthorizedAccessException)
            {
                return empty;   // アクセス権のないフォルダにアクセスした場合は無視
            }
        }

    }
}
