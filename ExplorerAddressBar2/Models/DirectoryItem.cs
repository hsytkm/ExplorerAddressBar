namespace ExplorerAddressBar2.Models
{
    public class DirectoryItem
    {
        public string FullPath { get; }
        public string Name { get; }

        public DirectoryItem(string fullPath, string dirName)
        {
            FullPath = fullPath;
            Name = dirName;
        }
    }
}
