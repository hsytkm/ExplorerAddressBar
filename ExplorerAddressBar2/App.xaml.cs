using ExplorerAddressBar2.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace ExplorerAddressBar2
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
