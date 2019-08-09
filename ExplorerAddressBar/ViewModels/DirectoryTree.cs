using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace ExplorerAddressBar.ViewModels
{
    class DirectoryTree
    {
        public string BasePath { get; }

        public IList<DirectoryNode> Nodes { get; }


        public DirectoryTree(string path)
        {
            BasePath = GetBasePath(path);
        }

        private static string GetBasePath(string path)
        {
            // nullならDesktopにする
            //if (string.IsNullOrWhiteSpace(path))
            //    return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            // ディレクトリ存在しなければデスクトップで上書き
            if (!Directory.Exists(path))
                return null;
                //return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            // 文字末の \ は除去
            return path.TrimEnd(Path.DirectorySeparatorChar);
        }

    }
}
