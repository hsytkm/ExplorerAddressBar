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
        private readonly ReactiveProperty<DirectoryNode> _directoryNode = new ReactiveProperty<DirectoryNode>();

        // Viewで選択されたディレクトリ
        public ReactiveProperty<DirectoryNode> SelectedDirectory { get; } = new ReactiveProperty<DirectoryNode>();

        // 対象ディレクトリ名
        public ReadOnlyReactiveProperty<string> DirectoryName { get; }

        // 子ディレクトリを持つか(末端ディレクトリ)フラグ
        public ReadOnlyReactiveProperty<bool> HasChildDirectory { get; }

        // 対象ディレクトリ内のディレクトリ
        public ReactiveCollection<DirectoryNode> ChildDirectoryNodes { get; }

        public DirectoryPathNodeViewModel(IContainerExtension container)
        {
            var modelMaster = container.Resolve<ModelMaster>();

            // 対象ディレクトリ名
            DirectoryName = _directoryNode
                .Where(x => x != null)
                .Select(x => x.Name)
                .ToReadOnlyReactiveProperty();

            // 子ディレクトリを持つか(末端ディレクトリ)フラグ
            HasChildDirectory = _directoryNode
                .Where(x => x != null)
                .Select(x => x.GetChildDirectoryNodes().Any())
                .ToReadOnlyReactiveProperty();

            // 対象ディレクトリ内のディレクトリ
            ChildDirectoryNodes = _directoryNode
                .Where(x => x != null)
                .Select(x => x.GetChildDirectoryNodes())
                .SelectMany(x => x)
                .ToReactiveCollection();

            // Viewからのディレクトリ選択
            SelectedDirectory
                .Where(x => x != null)
                .Do(x => modelMaster.TargetDirectoryPath = x.FullPath)  // 選択結果を通知
                .Subscribe(x => SelectedDirectory.Value = null);        // 通知したら空に戻す(値保持しない)

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
                _directoryNode.Value = directoryNode;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[DirectoryNodeKey] is DirectoryNode directoryNode)
                return directoryNode != null && _directoryNode.Value.FullPath == directoryNode.FullPath;
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion

    }
}
