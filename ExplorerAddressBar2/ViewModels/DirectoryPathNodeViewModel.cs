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
using System.Text;
using System.Threading.Tasks;

namespace ExplorerAddressBar2.ViewModels
{
    class DirectoryPathNodeViewModel : BindableBase, INavigationAware
    {
        private const string DirectoryPathKey = nameof(DirectoryPathKey);

        // 対象ディレクトリPATH
        private readonly ReactiveProperty<string> _directoryPath = new ReactiveProperty<string>();

        // Viewで選択されたディレクトリ
        public ReactiveProperty<DirectoryItem> SelectedDirectory { get; } =
            new ReactiveProperty<DirectoryItem>();

        // 子ディレクトリを持つか(末端ディレクトリ)フラグ
        public ReadOnlyReactiveProperty<bool> HasChildDirectory { get; }
        
        // 対象ディレクトリ名
        public ReadOnlyReactiveProperty<string> DirectoryName { get; }

        // 対象ディレクトリ内のディレクトリ
        public ReadOnlyReactiveProperty<IList<DirectoryItem>> ChildDirectoryItems { get; }

        public DirectoryPathNodeViewModel(IContainerExtension container)
        {
            var modelMaster = container.Resolve<ModelMaster>();

            // 対象ディレクトリ名
            DirectoryName = _directoryPath
                .Select(x => DirectoryItem.GetDirectoryName(x))
                .ToReadOnlyReactiveProperty();

            // 対象ディレクトリ内のディレクトリ
            ChildDirectoryItems = _directoryPath
                .Select(x => DirectoryNode.GetChildDirectoryItems(x).ToList())
                .Cast<IList<DirectoryItem>>()
                .ToReadOnlyReactiveProperty();

            // 子ディレクトリを持つか(末端ディレクトリ)フラグ
            HasChildDirectory = _directoryPath
                .Select(x => DirectoryNode.GetChildDirectoryItems(x).Any())
                .ToReadOnlyReactiveProperty();

            // Viewで選択されたディレクトリ
            SelectedDirectory
                .Where(x => x != null)
                .Subscribe(x => modelMaster.TargetDirectoryPath = x.FullPath);

        }

        #region INavigationAware

        public static NavigationParameters GetNavigationParameters(string directoryPath) =>
            new NavigationParameters
                {
                    { DirectoryPathKey, directoryPath },
                };

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[DirectoryPathKey] is string directoryPath)
            {
                _directoryPath.Value = directoryPath;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[DirectoryPathKey] is string directoryPath)
                return directoryPath != null && _directoryPath.Value == directoryPath;
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion

    }
}
