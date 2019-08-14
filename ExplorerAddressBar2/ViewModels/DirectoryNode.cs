using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExplorerAddressBar2.ViewModels
{
    public class DirectoryNode
    {
        // ディレクトリ略名の最大文字列(5文字なら "dire..." となる)
        private static readonly int AbbreviationNameLengthMax = 10;

        public string FullPath { get; } // ディレクトリのフルパス
        public string FullName { get; } // ディレクトリ名
        public string AbbrName { get; } // Abbreviation=略語

        public DirectoryNode(string fullPath)
        {
            FullPath = EmendFullPath(fullPath);
            FullName = GetDirectoryName(fullPath);
            AbbrName = GetAbbreviationDirectoryName(FullName);
        }

        // 子ディレクトリがあるかフラグ
        public bool HasChildDirectory() => GetChildDirectoryNodes(FullPath).Any();

        // 引数pathディレクトリ内の子ファイルを返す
        public IEnumerable<DirectoryNode> GetChildDirectoryNodes() => GetChildDirectoryNodes(FullPath);

        // 引数pathディレクトリ内の子ファイルを返す
        public IEnumerable<string> GetChildFileNames() => GetChildFileNames(FullPath);

        // FullPathを整形
        private static string EmendFullPath(string srcPath)
        {
            if (srcPath.Last() == Path.DirectorySeparatorChar)
            {
                // "C:\" は \ を除去しない
                if (srcPath.Skip(srcPath.Count() - 2).First() != Path.VolumeSeparatorChar)
                    return srcPath.TrimEnd(Path.DirectorySeparatorChar);
            }
            return srcPath;
        }

        // フルパスから末端ディレクトリのView用の略名を取得する
        private static string GetAbbreviationDirectoryName(string dirName)
        {
            var lengthMax = AbbreviationNameLengthMax;
            if (dirName.Length > lengthMax - 1)
                dirName = dirName.Substring(0, lengthMax - 1) + "...";
            return dirName;
        }

        // ディレクトリのフルパスから末端ディレクトリ名を取得する
        private static string GetDirectoryName(string fullPath) =>
            fullPath?.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();

        // 引数pathディレクトリ内の子ディレクトリを返す
        private static IEnumerable<DirectoryNode> GetChildDirectoryNodes(string basePath)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(basePath)) yield break;

            IEnumerable<DirectoryNode> items;
            try
            {
                items = Directory.GetDirectories(basePath, "*", SearchOption.TopDirectoryOnly)
                    .Select(p => new DirectoryNode(p));
            }
            catch (UnauthorizedAccessException)
            {
                yield break;   // アクセス権のないフォルダにアクセスした場合は無視
            }
            foreach (var item in items) yield return item;
        }

        // 引数pathディレクトリ内の子ファイルを返す
        public static IEnumerable<string> GetChildFileNames(string basePath)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(basePath)) yield break;

            IEnumerable<string> items;
            try
            {
                items = Directory.GetFiles(basePath, "*", SearchOption.TopDirectoryOnly)
                    .Select(p => Path.GetFileName(p));
            }
            catch (UnauthorizedAccessException)
            {
                yield break;   // アクセス権のないフォルダにアクセスした場合は無視
            }
            foreach (var item in items) yield return item;
        }

        // 引数ディレクトリからDirectoryNodeクラスをルートから順番に返す
        public static IEnumerable<DirectoryNode> GetDirectoryNodes(string basePath) =>
            GetDirectoriesPath(basePath).Select(path => new DirectoryNode(path));

        // 引数ディレクトリからDirectoryPATHをルートから順番に返す
        private static IEnumerable<string> GetDirectoriesPath(string basePath)
        {
            if (string.IsNullOrEmpty(basePath)) yield break;
            if (!Directory.Exists(basePath)) yield break;

            var path = "";
            var dirNames = basePath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var dirName in dirNames)
            {
                // "C:" になっちゃうので必ず最後に\付ける
                path = Path.Combine(path, dirName) + Path.DirectorySeparatorChar;
                yield return path;
            }
        }

    }
}
