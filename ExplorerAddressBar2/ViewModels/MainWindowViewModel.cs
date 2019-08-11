using ExplorerAddressBar2.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerAddressBar2.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private const string InitialPath = @"C:\data\";

        // 選択中ディレクトリPATH
        public ReactiveProperty<string> BasePath { get; } = new ReactiveProperty<string>(initialValue: InitialPath);

        // 選択中ディレクトリの子ファイル
        public ReadOnlyReactiveProperty<string> ChildFiles { get; }

        // 選択ディレクトリのノードチェーン
        public ReadOnlyReactiveProperty<IList<DirectoryNode>> DirectoryNodeChain { get; }

        public MainWindowViewModel()
        {
            // 選択中ディレクトリの更新
            var directoryTree = BasePath
                .Select(path => new DirectoryTree(path))
                .ToReadOnlyReactivePropertySlim();

            // 末端ディレクトリ内のファイル一覧
            ChildFiles = directoryTree
                .Select(x => x.Nodes.LastOrDefault()?.ChildFileNames)
                .Where(x => x != null)
                .Select(x => string.Join(Environment.NewLine, x))
                .ToReadOnlyReactiveProperty();

            // 選択ディレクトリのノードチェーン
            DirectoryNodeChain = directoryTree.Select(x => x.Nodes)
                .ToReadOnlyReactiveProperty();

        }

    }
}
