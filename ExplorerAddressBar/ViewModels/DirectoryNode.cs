using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerAddressBar.ViewModels
{
    class DirectoryNode
    {
        public string NodeFullPath { get; }

        public IList<string> ChildrenDirectoriesPath { get; }

        public IList<string> ChildrenFilesPath { get; }

    }
}
