using ExplorerAddressBar2.Models;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace ExplorerAddressBar2.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        // 選択中ディレクトリPATH
        public ReadOnlyReactiveProperty<string> BasePath { get; }

        // 選択中ディレクトリの子ファイル
        public ReadOnlyReactiveProperty<string> ChildFiles { get; }

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            var modelMaster = container.Resolve<ModelMaster>();

            // Modelのプロパティを参照
            BasePath = modelMaster
                .ObserveProperty(x => x.TargetDirectoryPath)
                .ToReadOnlyReactiveProperty();

            // 対象ディレクトリ内のファイル一覧
            ChildFiles = BasePath
                .Select(path => DirectoryNode.GetChildFileNames(path))
                .Select(x => string.Join(Environment.NewLine, x))
                .ToReadOnlyReactiveProperty();
        }

    }
}
