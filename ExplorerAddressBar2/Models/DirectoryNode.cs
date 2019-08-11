﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExplorerAddressBar2.Models
{
    class DirectoryNode
    {
        public string Name { get; }

        public string FullPath { get; }

        public IList<DirectoryItem> ChildDirectoryNames { get; private set; }

        public IList<string> ChildFileNames { get; private set; }

        public DirectoryNode(string path)
        {
            Name = GetDirectoryName(path);
            FullPath = path;
            Update();
        }

        // ディレクトリを更新
        public void Update()
        {
            var path = FullPath;
            if (string.IsNullOrEmpty(path)) throw new NullReferenceException(nameof(path));

            ChildDirectoryNames = GetChildDirectories(path).ToList();
            ChildFileNames = GetChildFiles(path).ToList();
        }

        // 引数pathディレクトリ内の子ディレクトリを返す
        private static IEnumerable<DirectoryItem> GetChildDirectories(string basePath)
        {
            var empty = Enumerable.Empty<DirectoryItem>();

            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(basePath)) return empty;

            try
            {
                return Directory.GetDirectories(basePath, "*", SearchOption.TopDirectoryOnly)
                    .Select(p => new DirectoryItem(p, GetDirectoryName(p)));
            }
            catch (UnauthorizedAccessException)
            {
                return empty;   // アクセス権のないフォルダにアクセスした場合は無視
            }
        }

        // 引数pathディレクトリ内の子ファイルを返す
        private static IEnumerable<string> GetChildFiles(string basePath)
        {
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

        // ディレクトリのフルパスから末尾のディレクトリ名を取得する
        public static string GetDirectoryName(string path)
            => path.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();

    }
}
