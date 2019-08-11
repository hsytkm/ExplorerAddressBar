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
        private double windowWidth;
        private double myControlWidth;

        public DirectoryPathBar()
        {
            InitializeComponent();

#if true    // サイズ確認
            PathBarBody.SizeChanged += (sender, e) =>
            {
                myControlWidth = e.NewSize.Width;
            };

            this.Loaded += (_, __) =>
            {
                var window = Window.GetWindow(this);
                window.SizeChanged += (sender, e) =>
                {
                    windowWidth = e.NewSize.Width;
                    Console.WriteLine($"DirectoryPathBar Width: {myControlWidth:f2} / {windowWidth:f2}");
                };
            };
#endif

        }

    }
}
