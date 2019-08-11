using System;
using System.Windows;
using System.Windows.Controls;

namespace ExplorerAddressBar2.Views
{
    /// <summary>
    /// DirectoryPathBar.xaml の相互作用ロジック
    /// </summary>
    public partial class DirectoryPathBar : UserControl
    {
        private double _visibleWidth;
        private double _unlimitWidth;

        public DirectoryPathBar()
        {
            InitializeComponent();

#if true    // サイズ確認
            PathBarBody.SizeChanged += (sender, e) =>
            {
                _unlimitWidth = e.NewSize.Width;
            };

            this.Loaded += (_, __) =>
            {
                this.SizeChanged += (sender, e) =>
                {
                    _visibleWidth = e.NewSize.Width;
                    Console.WriteLine($"DirectoryPathBar Width: {_unlimitWidth:f2} / {_visibleWidth:f2}");
                };
            };
#endif

        }

    }
}
