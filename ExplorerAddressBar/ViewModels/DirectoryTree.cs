using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;

namespace ExplorerAddressBar.ViewModels
{
    class DirectoryTree
    {
        public IList<DirectoryNode> Nodes { get; }

        public DirectoryTree(string basePath)
        {
            basePath = GetBasePath(basePath);

            Nodes = GetDirectoryPathTree(basePath).Select(x => new DirectoryNode(x)).ToList();
        }

        // フルパスからDirectoryを順に返す(ルートから順番)
        private static IEnumerable<string> GetDirectoryPathTree(string basePath)
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

        // 基準パス
        private static string GetBasePath(string basePath)
        {
            // nullならDesktopにする
            //if (string.IsNullOrWhiteSpace(basePath))
            //    return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            // ディレクトリ存在しなければデスクトップで上書き
            if (!Directory.Exists(basePath))
                return null;
            //return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            // 文字列末尾の \ は除去
            return basePath;    //.TrimEnd(Path.DirectorySeparatorChar);
        }

    }
}
