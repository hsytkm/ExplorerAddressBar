using ExplorerAddressBar2.Models;
using ExplorerAddressBar2.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ExplorerAddressBar2.ViewModels
{
    class DirectoryPathBarViewModel : BindableBase
    {
        private const string DirectoryPathNodeRegionName = "DirectoryPathNodeRegion";

        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;

        // テキストボックスの表示フラグ(ボタンバーと論理逆)
        public ReactiveProperty<bool> IsVisibleTextBoxPath { get; } = new ReactiveProperty<bool>(initialValue: false);

        // テキストボックスの表示コマンド
        public ReactiveProperty<Unit> VisibleTextBoxPathCommand { get; } = new ReactiveProperty<Unit>(mode: ReactivePropertyMode.None);

        // テキストボックスの確定(+非表示)コマンド
        public ReactiveProperty<string> SetTextBoxPathCommand { get; } = new ReactiveProperty<string>(mode: ReactivePropertyMode.None);

        // テキストボックスの表示文字列OneWay(表示時に設定、非表示時にクリア)
        public string TargetDirectoryPath
        {
            get => _targetDirectoryPath;
            private set => SetProperty(ref _targetDirectoryPath, value);
        }
        private string _targetDirectoryPath;

        public DirectoryPathBarViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

            var modelMaster = container.Resolve<ModelMaster>();

            // TextBoxの表示コマンド
            VisibleTextBoxPathCommand
                .Subscribe(_ => IsVisibleTextBoxPath.Value = true);

            // テキストボックスの確定(+非表示)コマンド
            SetTextBoxPathCommand
                .Subscribe(path =>
                {
                    // 入力のディレクトリが存在したらModel反映
                    if (Directory.Exists(path)) modelMaster.TargetDirectoryPath = path;

                    IsVisibleTextBoxPath.Value = false;
                });

            // テキストボックスの表示文字列は、表示時に設定/非表示時にクリア
            IsVisibleTextBoxPath
                .Subscribe(b => TargetDirectoryPath = b ? modelMaster.TargetDirectoryPath : null);

            // 選択中ディレクトリの更新(RegionにView読み出し)
            modelMaster
                .ObserveProperty(x => x.TargetDirectoryPath)
                .Subscribe(AddViewNode);
        }

        // メタ情報クラスからView用のTabを読み出し
        private void AddViewNode(string targetDirectoryPath)
        {
            if (string.IsNullOrEmpty(targetDirectoryPath))
                throw new ArgumentNullException(nameof(targetDirectoryPath));

            var regionName = DirectoryPathNodeRegionName;
            var directoryNodes = DirectoryNode.GetDirectoryNodes(targetDirectoryPath);

            // 登録済みRegionViewの削除
            if (_regionManager.Regions.ContainsRegionWithName(regionName))
                _regionManager.Regions[regionName].RemoveAll();

            // Viewを順に作成してRegionに登録
            foreach (var directoryNode in directoryNodes)
            {
                _regionManager.RegisterViewWithRegion(regionName, () =>
                {
                    var view = _container.Resolve<DirectoryPathNode>();
                    (view.DataContext as DirectoryPathNodeViewModel)?.SetDirectoryNode(directoryNode);
                    return view;
                });
            }
        }

    }
}
