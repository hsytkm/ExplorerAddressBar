using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace ExplorerAddressBar.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private const string InitialPath = @"C:\data\";

        public ReactiveProperty<string> BasePath { get; } = new ReactiveProperty<string>(initialValue: InitialPath);

        public ReadOnlyReactiveProperty<string> ChildDirectories { get; }
        public ReadOnlyReactiveProperty<string> ChildFiles { get; }

        public MainWindowViewModel()
        {
            var directoryTree = BasePath
                .Select(path => new DirectoryTree(path))
                .ToReadOnlyReactivePropertySlim();

            ChildDirectories = directoryTree.Select(x => GetChildDirectories(x.BasePath))
                .Select(dirs => string.Join(Environment.NewLine, dirs))
                .ToReadOnlyReactiveProperty();

            ChildFiles = directoryTree.Select(x => GetChildFiles(x.BasePath))
                .Select(files => string.Join(Environment.NewLine, files))
                .ToReadOnlyReactiveProperty();

        }

        // 引数pathディレクトリ内の子ディレクトリを返す
        private static IEnumerable<string> GetChildDirectories(string path)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(path)) return Enumerable.Empty<string>();

            return Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly)
                .Select(p => p.Replace(path, ""))
                .Select(p => p.TrimStart(Path.DirectorySeparatorChar)); // ディレクトリ先頭の\を削除
        }

        // 引数pathディレクトリ内の子ファイルを返す
        private static IEnumerable<string> GetChildFiles(string path)
        {
            // Exists()の中で、null/存在しないPATHもチェックしてくれる
            if (!Directory.Exists(path)) return Enumerable.Empty<string>();

            return Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly)
                .Select(p => Path.GetFileName(p));
        }

    }
}
