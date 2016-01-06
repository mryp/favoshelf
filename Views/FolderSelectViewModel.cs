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
    /// <summary>
    /// フォルダ一覧画面のビューモデル
    /// </summary>
    public class FolderSelectViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// フォルダ・ファイルリスト
        /// </summary>
        private ObservableCollection<FolderListItem> m_itemList = new ObservableCollection<FolderListItem>();

        /// <summary>
        /// フォルダ・ファイルリスト
        /// </summary>
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

        public FolderSelectViewModel()
        {
        }
        
        /// <summary>
        /// 初期化（フォルダパス用）
        /// </summary>
        /// <param name="folderPath"></param>
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
                    FolderListItem item = new FolderListItem()
                    {
                        Name = file.DisplayName,
                        Path = file.Path,
                        Token = "",
                        Type = getFileType(file)
                    };
                    ItemList.Add(item);
                }
            }
        }

        /// <summary>
        /// 初期化（トークン用）
        /// </summary>
        /// <param name="tokenList"></param>
        public async void InitFromToken(List<string> tokenList)
        {
            ItemList.Clear();
            foreach (string token in tokenList)
            {
                IStorageItem storageItem = null;
                if (StorageApplicationPermissions.MostRecentlyUsedList.ContainsItem(token))
                {
                    storageItem = await StorageApplicationPermissions.MostRecentlyUsedList.GetItemAsync(token);

                }
                else if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                {
                    storageItem = await StorageApplicationPermissions.FutureAccessList.GetItemAsync(token);
                }
                else
                {
                    continue;
                }
                if (storageItem.IsOfType(StorageItemTypes.Folder))
                {
                    ItemList.Add(new FolderListItem()
                    {
                        Name = storageItem.Name,
                        Path = storageItem.Path,
                        Token = token,
                        Type = FolderListItem.FileType.Folder
                    });
                }
                else
                {
                    FolderListItem item = new FolderListItem()
                    {
                        Name = storageItem.Name,
                        Path = storageItem.Path,
                        Token = token,
                        Type = getFileType((StorageFile)storageItem)
                    };
                    ItemList.Add(item);
                }
            }
        }


        /// <summary>
        /// 指定したファイルのタイプを取得する
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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
