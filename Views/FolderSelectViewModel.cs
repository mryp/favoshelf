using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<FolderListItem> m_itemList = new ObservableCollection<FolderListItem>();

        public ObservableCollection<FolderListItem> ItemList
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
            ItemList.Clear();
            foreach (string token in tokenList)
            {
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                {
                    StorageFolder folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                    if (folder != null)
                    {
                        Debug.WriteLine("有効トークン token=" + token + " folder=" + folder.Path);
                        ItemList.Add(new FolderListItem()
                        {
                            Name = folder.DisplayName,
                            Path = folder.Path,
                            Token = token,
                            Type = FolderListItem.FileType.Folder
                        });
                    }
                }
                else
                {
                    removeToken(token);
                }
            }
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
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            if (folder != null)
            {
                IReadOnlyList<StorageFolder> subFolderList = await folder.GetFoldersAsync();
                foreach (StorageFolder subFolder in subFolderList)
                {
                    ItemList.Add(new FolderListItem()
                    {
                        Name = subFolder.DisplayName,
                        Path = subFolder.Path,
                        Token = "",
                        Type = FolderListItem.FileType.Folder
                    });
                }

                IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
                foreach (StorageFile file in fileList)
                {
                    ItemList.Add(new FolderListItem()
                    {
                        Name = file.DisplayName,
                        Path = file.Path,
                        Token = "",
                        Type = getFileType(file)
                    });
                }
            }
        }

        private FolderListItem.FileType getFileType(StorageFile file)
        {
            FolderListItem.FileType resultType = FolderListItem.FileType.OtherFile;
            if (FileKind.IsImageFile(file.Path))
            {
                resultType = FolderListItem.FileType.ImageFile;
            }
            else if (FileKind.IsArchiveFile(file.Path))
            {
                resultType = FolderListItem.FileType.Archive;
            }

            return resultType;
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
