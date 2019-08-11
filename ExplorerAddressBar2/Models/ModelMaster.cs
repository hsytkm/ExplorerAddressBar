using Prism.Mvvm;

namespace ExplorerAddressBar2.Models
{
    class ModelMaster : BindableBase
    {
        private const string InitialDirectoryPath = @"C:\data\";

        // 選択中ディレクトリのフルPATH
        public string TargetDirectoryPath
        {
            get => _targetDirectoryPath;
            set => SetProperty(ref _targetDirectoryPath, value);
        }
        private string _targetDirectoryPath = InitialDirectoryPath;

        public ModelMaster() { }

    }
}
