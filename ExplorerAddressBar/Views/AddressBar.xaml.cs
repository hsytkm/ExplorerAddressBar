using ExplorerAddressBar.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ExplorerAddressBar.Views
{
    /// <summary>
    /// AddressBar.xaml の相互作用ロジック
    /// </summary>
    public partial class AddressBar : UserControl
    {
        private static readonly Type SelfType = typeof(AddressBar);

        #region DirectoryNodeChainProperty(OneWayToSource)

        // ディレクトリノードチェーン(OneWay)
        private static readonly DependencyProperty DirectoryNodeChainProperty =
            DependencyProperty.Register(
                nameof(DirectoryNodeChain),
                typeof(IList<DirectoryNode>),
                SelfType,
                new FrameworkPropertyMetadata(
                    default(IList<DirectoryNode>),
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if (d is AddressBar addrBar && e.NewValue is IList<DirectoryNode> nodes)
                            addrBar.Update(nodes);
                    }));

        internal IList<DirectoryNode> DirectoryNodeChain
        {
            get => (IList<DirectoryNode>)GetValue(DirectoryNodeChainProperty);
            set => SetValue(DirectoryNodeChainProperty, value);
        }

        #endregion

        public AddressBar()
        {
            InitializeComponent();

            this.SizeChanged += (sender, e) =>
            {
                Console.WriteLine($"SizeChanged-ActualWidth: {e.NewSize.Width}");
            };

        }

        // ディレクトリ変更をトリガにViewを更新する
        private void Update(IList<DirectoryNode> nodes)
        {
            // ◆未実装
            Console.WriteLine($"Update-ActualWidth: {ActualWidth}");

            foreach (var node in nodes)
            {
                Console.WriteLine($"FullPath: {node.BasePath}");
            }
        }

    }
}
