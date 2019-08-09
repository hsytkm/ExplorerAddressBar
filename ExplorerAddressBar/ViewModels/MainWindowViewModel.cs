using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace ExplorerAddressBar.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private const string InitialPath = @"C:\data\";

        #region 選択中ディレクトリ

        // 選択中ディレクトリPATH
        public ReactiveProperty<string> BasePath { get; } = new ReactiveProperty<string>(initialValue: InitialPath);

        // 選択中ディレクトリが子ディレクトリ持つかフラグ
        public ReactiveProperty<bool> HasChildDirectory { get; } = new ReactiveProperty<bool>();

        // 選択中ディレクトリの子ファイル
        public ReadOnlyReactiveProperty<string> ChildFiles { get; }

        #endregion

        #region 子ディレクトリの選択

        // 選択候補のディレクトリ達
        public ObservableCollection<DirectoryItem> ChildDirectories { get; } = new ObservableCollection<DirectoryItem>();

        // Viewで選択されたディレクトリ
        public ReactiveProperty<DirectoryItem> SelectedDirectory { get; } = new ReactiveProperty<DirectoryItem>();

        #endregion

        // 選択ディレクトリのノードチェーン
        public IList<DirectoryNode> DirectoryNodeChain
        {
            get => _DirectoryNodeChain;
            private set => SetProperty(ref _DirectoryNodeChain, value);
        }
        private IList<DirectoryNode> _DirectoryNodeChain;

        public MainWindowViewModel()
        {
            // 選択中ディレクトリの更新
            var directoryTree = BasePath
                .Select(path => new DirectoryTree(path))
                .ToReadOnlyReactivePropertySlim();

            // 子ディレクトリの存在チェック(もうちょいマシな実装ない？)
            var collectionChanged = ChildDirectories.CollectionChangedAsObservable();
            collectionChanged.Where(e => e.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(_ => HasChildDirectory.Value = false);
            collectionChanged.Where(e => e.Action == NotifyCollectionChangedAction.Add)
                .Subscribe(_ => HasChildDirectory.Value = true);

            // 末端ディレクトリの選択候補
            directoryTree
                .Select(x => x.Nodes.LastOrDefault()?.ChildDirectoryNames)
                .Where(x => x != null)
                .Subscribe(paths =>
                {
                    ChildDirectories.Clear();
                    foreach(var path in paths) ChildDirectories.Add(path);
                });

            // Viewで選択されたディレクトリ
            SelectedDirectory.Where(x => x != null)
                .Subscribe(x => BasePath.Value = x.FullPath);

            // 末端ディレクトリ内のファイル一覧
            ChildFiles = directoryTree
                .Select(x => x.Nodes.LastOrDefault()?.ChildFileNames)
                .Where(x => x != null)
                .Select(x => string.Join(Environment.NewLine, x))
                .ToReadOnlyReactiveProperty();

            // 選択ディレクトリのノードチェーン
            directoryTree.Select(x => x.Nodes)
                .Where(x => x != null)
                .Subscribe(nodes => DirectoryNodeChain = nodes);

        }

    }
}
