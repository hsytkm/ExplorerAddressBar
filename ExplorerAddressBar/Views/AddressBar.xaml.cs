using ExplorerAddressBar.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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
                            addrBar.Update(addrBar.ActualWidth, nodes);
                    }));

        internal IList<DirectoryNode> DirectoryNodeChain
        {
            get => (IList<DirectoryNode>)GetValue(DirectoryNodeChainProperty);
            set => SetValue(DirectoryNodeChainProperty, value);
        }

        #endregion

        #region SelectedDirectoryFullPathProperty(TwoWay)

        // 選択中のディレクトリ
        private static readonly DependencyProperty SelectedDirectoryFullPathProperty =
            DependencyProperty.Register(
                nameof(SelectedDirectoryFullPath),
                typeof(string),
                SelfType,
                new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        internal string SelectedDirectoryFullPath
        {
            get => (string)GetValue(SelectedDirectoryFullPathProperty);
            set => SetValue(SelectedDirectoryFullPathProperty, value);
        }

        #endregion

        #region SelectedDirectoryItem

        // コードビハインドで生成されるComboBoxでBindingされるプロパティ
        public DirectoryItem SelectedDirectoryItem
        {
            get => _SelectedDirectoryItem;
            set
            {
                if (_SelectedDirectoryItem != value)
                {
                    _SelectedDirectoryItem = value;
                    SelectedDirectoryFullPath = value.FullPath;
                }
            }
        }
        private DirectoryItem _SelectedDirectoryItem;

        #endregion

        public AddressBar()
        {
            InitializeComponent();

            this.SizeChanged += (sender, e) => Update(e.NewSize.Width, DirectoryNodeChain);
        }

        // ディレクトリ変更をトリガにViewを更新する
        private void Update(double actualWidth, IList<DirectoryNode> nodes)
        {
            if (actualWidth == 0) return;
            if (nodes is null || !nodes.Any()) return;

            // ディレクトリ名のボタンを追加
            groundPanel.Children.Clear();
            foreach (var dirButton in GetDirectoryNameButtons(nodes))
            {
                groundPanel.Children.Add(dirButton);
            }

            // 末尾ディレクトリの子ディレクトリ選択コントロール
            var tailDirChildren = GetTailDirectoryChildren(nodes.LastOrDefault());
            if (tailDirChildren != null)
            {
                groundPanel.Children.Add(tailDirChildren);
            }
        }

        // ディレクトリ名のボタンコントロール
        private IEnumerable<UIElement> GetDirectoryNameButtons(IList<DirectoryNode> nodes)
        {
            if (nodes is null) yield break;
            foreach (var node in nodes)
            {
                var dirButton = new Button()
                {
                    Padding = new Thickness(7, 2, 7, 2),
                    Margin = new Thickness(1, 0, 0, 0),
                    Content = node.Name,
                    Background = new SolidColorBrush(Colors.Transparent),
                    Command = new RelayCommand(() => this.SelectedDirectoryFullPath = node.BasePath),
                    //CommandParameter = null,
                };
                yield return dirButton;
            }
        }

        //<ComboBox ItemsSource="{Binding ChildDirectories, Mode=OneWay}"
        //          SelectedValue="{Binding SelectedDirectory.Value, Mode=TwoWay}"
        //          IsEnabled="{Binding HasChildDirectory.Value, Mode=OneWay}" >
        //    <ComboBox.ItemTemplate>
        //        <DataTemplate>
        //            <TextBlock Text="{Binding Name}"/>
        //        </DataTemplate>
        //    </ComboBox.ItemTemplate>
        //</ComboBox>
        private UIElement GetTailDirectoryChildren(DirectoryNode tailNode)
        {
            if (tailNode is null) return null;

            // ComboBox.ItemTemplate
            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetValue(TextBlock.TextProperty, new Binding("Name"));
            var template = new DataTemplate
            {
                VisualTree = textBlockFactory
            };

            var element = new ComboBox()
            {
                Width = 18,
                Margin = new Thickness(1, 0, 0, 0),
                ItemsSource = tailNode.ChildDirectoryNames,
                IsEnabled = tailNode.ChildDirectoryNames.Any(),
                ItemTemplate = template,
            };

            element.SetBinding(Selector.SelectedValueProperty, new Binding()
            {
                Path = new PropertyPath(nameof(SelectedDirectoryItem)),
                Mode = BindingMode.TwoWay,
                Source = this,
            });

            return element;
        }

        // MVVM:とにかく適当なICommandを実装したい時のサンプル http://running-cs.hatenablog.com/entry/2016/09/03/211015
        private class RelayCommand : ICommand
        {
            private readonly Action _action;

            public RelayCommand(Action action) => _action = action;

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => _action != null;

            public void Execute(object parameter) => _action?.Invoke();
        }

    }
}
