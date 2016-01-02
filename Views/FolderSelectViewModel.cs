using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace favoshelf.Views
{
    public class FolderSelectViewModel : INotifyPropertyChanged
    {
        private List<FolderListItem> m_itemList = new List<FolderListItem>();

        public List<FolderListItem> ItemList
        {
            get
            {
                return m_itemList;
            }
            set
            {
                if (value != m_itemList)
                {
                    m_itemList = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public async void Init(string[] tokenList)
        {
            List<FolderListItem> itemList = new List<FolderListItem>();
            foreach (string token in tokenList)
            {
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                {
                    StorageFolder folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                    if (folder != null)
                    {
                        Debug.WriteLine("有効トークン token=" + token + " folder=" + folder.Path);
                        FolderListItem.FileType fileType = await getFileType(folder);
                        itemList.Add(new FolderListItem()
                        {
                            Name = folder.DisplayName,
                            Path = folder.Path,
                            Token = token,
                            Type = fileType
                        });
                    }
                }
                else
                {
                    removeToken(token);
                }
            }
            ItemList = itemList;
        }

        private void removeToken(string token)
        {
            Debug.WriteLine("無効トークン token=" + token);
            if (AppSettings.Current.FolderTokenList.Contains(token))
            {
                List<string> folderTokenList = new List<string>(AppSettings.Current.FolderTokenList);
                folderTokenList.Remove(token);
                AppSettings.Current.FolderTokenList = folderTokenList.ToArray();
            }
        }
        
        public async void InitFromFolderPath(string folderPath)
        {
            List<FolderListItem> itemList = new List<FolderListItem>();
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            if (folder != null)
            {
                IReadOnlyList<StorageFolder> subFolderList = await folder.GetFoldersAsync();
                foreach (StorageFolder subFolder in subFolderList)
                {
                    FolderListItem.FileType fileType = await getFileType(subFolder);
                    itemList.Add(new FolderListItem()
                    {
                        Name = subFolder.DisplayName,
                        Path = subFolder.Path,
                        Token = "",
                        Type = fileType
                    });
                }

                IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
                foreach (StorageFile file in fileList)
                {
                    itemList.Add(new FolderListItem()
                    {
                        Name = file.DisplayName,
                        Path = file.Path,
                        Token = "",
                        Type = getFileType(file)
                    });
                }
            }

            ItemList = itemList;
        }

        private FolderListItem.FileType getFileType(StorageFile file)
        {
            FolderListItem.FileType resultType = FolderListItem.FileType.File;
            switch (Path.GetExtension(file.Path))
            {
                case ".zip":
                    resultType = FolderListItem.FileType.Archive;
                    break;
            }

            return resultType;
        }

        private async Task<FolderListItem.FileType> getFileType(StorageFolder folder)
        {
            IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
            foreach (StorageFile file in fileList)
            {
                switch (Path.GetExtension(file.Path))
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        return FolderListItem.FileType.ImageFolder;
                }
            }

            return FolderListItem.FileType.Folder;
        }

        #region INotifyPropertyChanged member

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
