using ExplorerAddressBar2.Models;
using ExplorerAddressBar2.Views;
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
    class DirectoryPathNodeViewModel : BindableBase, INavigationAware
    {
        private const string DirectoryNodeKey = nameof(DirectoryNodeKey);

        // 対象ディレクトリPATH
        public ReactiveProperty<DirectoryNode> TargetDirectoryNode { get; } = new ReactiveProperty<DirectoryNode>();

        // Viewディレクトリ選択コンボボックス
        public ReactiveProperty<DirectoryNode> SelectedDirectory { get; } = new ReactiveProperty<DirectoryNode>();

        // 子ディレクトリを持つか(末端ディレクトリ)フラグ
        public ReadOnlyReactiveProperty<bool> HasChildDirectory { get; }

        // 対象ディレクトリ内のディレクトリ
        public ReactiveCollection<DirectoryNode> ChildDirectoryNodes { get; }

        // Viewディレクトリ選択ボタン
        public ReactiveCommand SelectDirectoryCommand { get; } = new ReactiveCommand();

        public DirectoryPathNodeViewModel(IContainerExtension container)
        {
            var modelMaster = container.Resolve<ModelMaster>();

            // 子ディレクトリを持つか(末端ディレクトリ)フラグ
            HasChildDirectory = TargetDirectoryNode
                .Where(x => x != null)
                .Select(x => x.HasChildDirectory())
                .ToReadOnlyReactiveProperty();

            // 対象ディレクトリ内のディレクトリ
            ChildDirectoryNodes = TargetDirectoryNode
                .Where(x => x != null)
                .Select(x => x.GetChildDirectoryNodes())
                .SelectMany(x => x)
                .ToReactiveCollection();

            // Viewからのディレクトリ選択コンボボックス
            SelectedDirectory
                .Where(x => x != null)
                .Do(x => modelMaster.TargetDirectoryPath = x.FullPath)  // 選択結果を通知
                .Subscribe(x => SelectedDirectory.Value = null);        // 通知したら空に戻す(値保持しない)

            // ディレクトリ選択ボタン
            SelectDirectoryCommand
                .Where(x => x != null)
                .Cast<DirectoryNode>()
                .Subscribe(x => modelMaster.TargetDirectoryPath = x.FullPath);

        }

        #region INavigationAware

        public static NavigationParameters GetNavigationParameters(DirectoryNode directoryNode) =>
            new NavigationParameters
                {
                    { DirectoryNodeKey, directoryNode },
                };

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[DirectoryNodeKey] is DirectoryNode directoryNode)
                TargetDirectoryNode.Value = directoryNode;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[DirectoryNodeKey] is DirectoryNode directoryNode)
                return directoryNode != null && TargetDirectoryNode.Value.FullPath == directoryNode.FullPath;
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion

    }
}
