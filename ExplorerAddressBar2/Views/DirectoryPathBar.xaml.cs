using ExplorerAddressBar2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ExplorerAddressBar2.Views
{
    /// <summary>
    /// DirectoryPathBar.xaml の相互作用ロジック
    /// </summary>
    public partial class DirectoryPathBar : UserControl
    {
        private double _nodeBarVisibleWidth;    // NodeBarの表示幅
        private double _nodeBarUnlimitedWidth;  // NodeBarの制限なし幅(理想幅)

        /// <summary>
        /// NodeBarの子要素と積み上げ幅の逆順リスト(逆=末端ディレクトリが先頭)
        /// FrameworkElementのVisibilityをCollapsedにするとサイズが取得できなくなるので、
        /// 表示中にサイズを保持する
        /// </summary>
        private readonly IList<(FrameworkElement Element, double SumWidth)> fwElementWidths =
            new List<(FrameworkElement Element, double SumWidth)>();

        public DirectoryPathBar()
        {
            InitializeComponent();

            // NodeBarの表示幅
            this.SizeChanged += (sender, e) =>
            {
                _nodeBarVisibleWidth = e.NewSize.Width;
                UpdateNodesVisibility();
            };

            // NodeBarの制限なし幅
            NodeItemsControl.SizeChanged += (sender, e) =>
            {
                _nodeBarUnlimitedWidth = e.NewSize.Width;
                UpdateNodesVisibility();
            };

            // テキストボックスの表示時にフォーカス移行＋カーソル最終文字
            DirectoryPathTextBox.IsVisibleChanged += (sender, e) =>
            {
                if (sender is TextBox textBox && e.NewValue is bool b && b)
                {
                    textBox.Focus();
                    textBox.Select(DirectoryPathTextBox.Text.Length, 0);
                }
            };

            // テキストボックスの外領域クリックで非表示か
            this.Loaded += (_, __) =>
            {
                var window = Window.GetWindow(this);
                window.PreviewMouseDown += (sender, e) =>
                {
                    // ViewModelのメソッドを操作する(◆無理やり感…)
                    (this.DataContext as DirectoryPathBarViewModel).SetInvisibleTextBox();
                };
            };

        }

        // アドレスバーのNode(ディレクトリ)の表示を切り替える
        private void UpdateNodesVisibility()
        {
            double visibleWidth = _nodeBarVisibleWidth;
            double unlimitedWidth = _nodeBarUnlimitedWidth;
            if (visibleWidth == 0 || unlimitedWidth == 0) return;

            //System.Diagnostics.Debug.WriteLine($"DirectoryPathBar Width: {visibleWidth:f2} / {unlimitedWidth:f2}");

            // ItemsControlの子要素達
            var sources = GetItemsControlSources(NodeItemsControl);

            // 全コントロールがVisibleの時点でバッファする
            var feWidths = buffItemSourcesWidths(sources);

            // 表示の余白幅(正数なら表示させる方向)
            if (visibleWidth - unlimitedWidth < 0)
            {
                // 表示幅が狭まったので非表示化する
                toCollapseNodes(feWidths, visibleWidth);
            }
            else
            {
                // 表示幅が広がったので再表示化する
                toVisibleNodes(feWidths, visibleWidth);
            }

            // 全コントロールがVisibleの時点でバッファする
            IList<(FrameworkElement Element, double SumWidth)>
                buffItemSourcesWidths(IEnumerable<FrameworkElement> elements)
            {
                if (elements.All(x => x.Visibility == Visibility.Visible))
                {
                    fwElementWidths.Clear();

                    // リストは逆管理
                    double sum = 0;
                    foreach (var element in elements.Reverse())
                    {
                        sum += element.ActualWidth;
                        fwElementWidths.Add((element, sum));
                    }
                }
                return fwElementWidths;
            }

            // 表示幅が狭まったので非表示化する
            void toCollapseNodes(IList<(FrameworkElement Element, double SumWidth)> ews, double viewWidth)
            {
                // 最小でも2つは表示させる(現＋上ディレクトリ)
                foreach (var (Element, SumWidth) in ews.Skip(2))
                {
                    if (SumWidth > viewWidth)
                        Element.Visibility = Visibility.Collapsed;
                }
            }

            // 表示幅が広がったので再表示化する
            void toVisibleNodes(IList<(FrameworkElement Element, double SumWidth)> ews, double viewWidth)
            {
                foreach (var (Element, SumWidth) in ews)
                {
                    if (Element.Visibility != Visibility.Visible)
                    {
                        if (SumWidth < viewWidth)
                            Element.Visibility = Visibility.Visible;
                        break;  // 1つ判定したら終わり
                    }
                }
            }
        }

        // ItemsControl内の子要素を取得する(DirectoryPathNode)
        private static IEnumerable<FrameworkElement> GetItemsControlSources(ItemsControl itemsControl)
        {
            if (itemsControl?.ItemsSource is null) yield break;

            foreach (var item in itemsControl.ItemsSource)
            {
                if (item is FrameworkElement fe)
                    yield return fe;
            }
        }

    }
}
