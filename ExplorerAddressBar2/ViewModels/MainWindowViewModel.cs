using ExplorerAddressBar2.Models;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ExplorerAddressBar2.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private readonly ModelMaster modelMaster;

        // 選択中ディレクトリPATH
        public ReactiveProperty<string> BasePath { get; } = new ReactiveProperty<string>();

        // 選択中ディレクトリの子ファイル
        public ReadOnlyReactiveProperty<string> ChildFiles { get; }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            modelMaster = container.Resolve<ModelMaster>();

            // ModelとのTwoWayバインディング
            BasePath = modelMaster.ToReactivePropertyAsSynchronized(x => x.TargetDirectoryPath);

            // 選択中ディレクトリの更新
            var directoryTree = BasePath
                .Select(path => new DirectoryTree(path))
                .ToReadOnlyReactivePropertySlim();

            // 末端ディレクトリ内のファイル一覧
            ChildFiles = directoryTree
                //.Select(x => x.Nodes.LastOrDefault()?.ChildFileNames)
                .Select(x => x.Nodes.LastOrDefault()?.GetChildFileNames())
                .Where(x => x != null)
                .Select(x => string.Join(Environment.NewLine, x))
                .ToReadOnlyReactiveProperty();

        }

    }
}
