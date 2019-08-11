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
    class DirectoryPathBarViewModel : BindableBase
    {
        private const string DirectoryPathNodeRegionName = "DirectoryPathNodeRegion";

        private readonly IRegionManager _regionManager;
        //private readonly ModelMaster _modelMaster;

        public DirectoryPathBarViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            var modelMaster = container.Resolve<ModelMaster>();

            // 選択中ディレクトリの更新
            modelMaster
                .ObserveProperty(x => x.TargetDirectoryPath)
                .Select(path => new DirectoryTree(path).Nodes)
                .Subscribe(AddViewNode);

        }

        // メタ情報クラスからView用のTabを読み出し
        private void AddViewNode(IList<DirectoryNode> directoryNodes)
        {
            if (directoryNodes is null) throw new ArgumentNullException(nameof(directoryNodes));

            var regionName = DirectoryPathNodeRegionName;

            // 登録済みRegionViewの削除
            if (_regionManager.Regions.ContainsRegionWithName(regionName))
                _regionManager.Regions[regionName].RemoveAll();

            foreach (var directoryNode in directoryNodes)
            {
                var parameters = DirectoryPathNodeViewModel.GetNavigationParameters(directoryNode);
                _regionManager.RequestNavigate(regionName, nameof(DirectoryPathNode), parameters);
            }
        }

    }
}
