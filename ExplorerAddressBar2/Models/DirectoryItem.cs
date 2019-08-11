using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExplorerAddressBar2.Models
{
    public class DirectoryItem
    {
        public string FullPath { get; }
        public string Name { get; }

        public DirectoryItem(string fullPath)
        {
            FullPath = fullPath;
            Name = GetDirectoryName(fullPath);
        }

        // ディレクトリのフルパスから末尾のディレクトリ名を取得する
        public static string GetDirectoryName(string path)
            => path?.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();

        // フルパスからDirectoryPATHを順に返す(ルートから順番)
        public static IEnumerable<string> GetDirectoriesPath(string basePath)
        {
            if (string.IsNullOrEmpty(basePath)) yield break;

            var path = "";
            foreach (var dirName in basePath
                .Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
            {
                // "C:" になっちゃうので必ず最後に\付ける
                path = Path.Combine(path, dirName) + Path.DirectorySeparatorChar;
                yield return path;
            }
        }

        // フルパスからDirectory名を順に返す(ルートから順番)
        public static IEnumerable<string> GetDirectoriesName(string basePath)
        {
            foreach (var dir in GetDirectoriesPath(basePath))
                yield return GetDirectoryName(dir);
        }

    }
}
