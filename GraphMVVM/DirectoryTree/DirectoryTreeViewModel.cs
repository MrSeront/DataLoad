using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace GraphMVVM.DirectoryTree
{
    public static class DirectoryTreeViewModel
    {
        static ObservableCollection<Item> EmptyNode = new ObservableCollection<Item>();

        public static ObservableCollection<DirectoryModel> InitializeDirectoryTree()
        {
            ObservableCollection<DirectoryModel> Item = new ObservableCollection<DirectoryModel>();

            Item.Add(new FavoriteFolder { Name = "Рабочий стол", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) });
            Item.Add(new FavoriteFolder { Name = "Мои документы", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) });

            foreach (string s in Directory.GetLogicalDrives())
            {
                Item.Add(new DirectoryModel { Name = s, Path = s });
            }
            return Item;
        }

        public static ObservableCollection<DirectoryModel> GetDirectoryTreeData(string lastpath)
        {
            ObservableCollection<DirectoryModel> Item = new ObservableCollection<DirectoryModel>();

            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            if (lastpath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)) || lastpath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)))
            {
                if (lastpath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)))
                {
                    Item.Add(new FavoriteFolder { Name = "Рабочий стол", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), IsExpanded = true, Children = GetTreeData(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), lastpath) });
                    Item.Add(new FavoriteFolder { Name = "Мои документы", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) });
                }
                if (lastpath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)))
                {
                    Item.Add(new FavoriteFolder { Name = "Рабочий стол", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), });
                    Item.Add(new FavoriteFolder { Name = "Мои документы", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), IsExpanded = true, Children = GetTreeData(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), lastpath) });

                }

                foreach (string s in Directory.GetLogicalDrives())
                {
                    if (lastpath.StartsWith(s))
                    {
                        Item.Add(new DirectoryModel { Name = s, Path = s, IsExpanded = true });
                    }
                    else
                    {
                        Item.Add(new DirectoryModel { Name = s, Path = s, Children = EmptyNode });
                    }

                    //  Item.Add(new DirectoryModel { Name = s, Path = s, Children=GetTreeData(s)});

                }
            }
            else
            {
                Item.Add(new FavoriteFolder { Name = "Рабочий стол", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) });
                Item.Add(new FavoriteFolder { Name = "Мои документы", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) });

                foreach (string s in Directory.GetLogicalDrives())
                {
                    if (lastpath.StartsWith(s))
                    {
                        Item.Add(new DirectoryModel { Name = s, Path = s, IsExpanded = true, Children = GetTreeData(s, lastpath) });
                    }
                    else
                    {
                        Item.Add(new DirectoryModel { Name = s, Path = s, Children = EmptyNode });
                    }

                    //  Item.Add(new DirectoryModel { Name = s, Path = s, Children=GetTreeData(s)});

                }
            }
            return Item;
        }
        
        public static ObservableCollection<Item> GetTreeData(string path, string lastpath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            ObservableCollection<Item> Item = new ObservableCollection<Item>();

            try
            {

                foreach (DirectoryInfo directory in dirInfo.GetDirectories())
                {

                    if (!directory.Attributes.HasFlag(FileAttributes.Hidden) && !directory.Attributes.HasFlag(FileAttributes.System))
                    {
                        if (lastpath.StartsWith(directory.FullName))
                        {
                            Item.Add(new DirectoryModel { Name = directory.Name, Path = directory.FullName, IsExpanded = true, Children = GetTreeData(directory.FullName, lastpath) });

                            //if (lastpath == directory.FullName)
                            //{

                            //}
                            //else
                            //{
                            //    Item.Add(new DirectoryModel { Name = directory.Name, Path = directory.FullName, Children = EmptyNode });
                            //}
                        }
                        else
                        {
                            Item.Add(new DirectoryModel { Name = directory.Name, Path = directory.FullName });
                        }
                    }

                }

                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    //if (getFileExtension(file.FullName) == "csv" || getFileExtension(file.FullName) == "txt" || getFileExtension(file.FullName) == "xml")
                    if (getFileExtension(file.FullName) == "imp" || getFileExtension(file.FullName) == "csv" || getFileExtension(file.FullName) == "txt")
                    {
                        var item = new FileModel
                        {
                            Name = file.Name,
                            Path = file.FullName
                        };
                        Item.Add(item);
                    }
                }

            }
            catch { }
            return Item;
        }

        private static string getFileExtension(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf(".") + 1);
        }
    }
}
