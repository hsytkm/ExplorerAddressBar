using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ExplorerAddressBar2.Models
{
    class DirectoryTree
    {
        public IList<DirectoryNode> Nodes { get; }

        public DirectoryTree(string basePath)
        {
            basePath = GetBasePath(basePath);

            Nodes = DirectoryItem.GetDirectoriesPath(basePath).Select(x => new DirectoryNode(x)).ToList();
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
