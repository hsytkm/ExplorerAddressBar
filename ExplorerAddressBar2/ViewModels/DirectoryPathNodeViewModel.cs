using ExplorerAddressBar2.Models;
using Prism.Ioc;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace ExplorerAddressBar2.ViewModels
{
    class DirectoryPathNodeViewModel : BindableBase
    {
        // 対象ディレクトリNodeクラス
        public ReactiveProperty<DirectoryNode> TargetDirectoryNode { get; } = new ReactiveProperty<DirectoryNode>();

        // Viewディレクトリ選択コンボボックス
        public ReactiveProperty<DirectoryNode> SelectedDirectoryNode { get; } = new ReactiveProperty<DirectoryNode>();

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
            SelectedDirectoryNode
                .Where(x => x != null)
                .Select(x => DirectoryNode.GetExistsDirectoryPath(x.FullPath))
                .Do(path => modelMaster.TargetDirectoryPath = path)     // 選択結果を通知
                .Subscribe(x => SelectedDirectoryNode.Value = null);    // 通知したら空に戻す(値保持しない)

            // ディレクトリ選択ボタン
            SelectDirectoryCommand
                .Where(x => x != null)
                .Cast<DirectoryNode>()
                .Select(x => DirectoryNode.GetExistsDirectoryPath(x.FullPath))
                .Subscribe(path => modelMaster.TargetDirectoryPath = path);

        }

        // 対象ディレクトリの設定(◆コンストラクタに渡す方法が分からなかったので裏からこっそり渡す)
        public void SetDirectoryNode(DirectoryNode directoryNode) =>
            TargetDirectoryNode.Value = directoryNode;

    }
}
