using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace favoshelf.Views
{
    /// <summary>
    /// 画像めくり対応ページ
    /// </summary>
    public sealed partial class ImageFlipPage : Page
    {
        #region 定数
        /// <summary>
        /// コマンドバーの高さ
        /// </summary>
        private const int COMMAND_AREA_HEIGHT = 48;

        /// <summary>
        /// 上部エリアの高さ
        /// </summary>
        private const int TOP_AREA_HEIGHT = 96;

        /// <summary>
        /// モバイル・PC切り替え判定横幅
        /// </summary>
        private const int UI_MODE_MIN_WINDOW_WIDTH = 720;

        /// <summary>
        /// タッチエリア
        /// </summary>
        private enum TouchArea
        {
            CommandArea,
            Top,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// ローカルDB
        /// </summary>
        private LocalDatabase m_db;

        /// <summary>
        /// ビューモデル
        /// </summary>
        public ImageFlipViewModel ViewModel
        {
            get;
            private set;
        }
        #endregion

        #region 初期化・画面遷移処理メソッド
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageFlipPage()
        {
            this.InitializeComponent();

            m_db = new LocalDatabase();
            this.ViewModel = new ImageFlipViewModel();
        }
        
        /// <summary>
        /// 画面遷移してきたときの処理
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(mainGrid_PointerReleased), true);

            ImageNavigateParameter param = e.Parameter as ImageNavigateParameter;
            if (param == null)
            {
                return;
            }

            IImageFileReader reader = null;
            if (param.Type == ImageNavigateParameter.DataType.ImageFile
            || param.Type == ImageNavigateParameter.DataType.Folder)
            {
                reader = new FolderImageFileReader(param);
            }
            else if (param.Type == ImageNavigateParameter.DataType.Archive)
            {
                reader = new ZipImageFileReader(param);
            }
            else
            {
                return;
            }
            await this.ViewModel.Init(reader, m_db);

            initBookCategory();
            initScrapbookCategory();
            setFirstImage();
        }

        /// <summary>
        /// 画面から離れるとき
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.RemoveHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(mainGrid_PointerReleased));

            //終了処理
            this.ViewModel.Dispose();

            //フルスクリーンだったら元に戻す
            if (ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
            }

            //ナビメニューの表示状態を元に戻す
            AppShell shell = Window.Current.Content as AppShell;
            if (shell != null)
            {
                shell.ShowMenuPane();
            }
        }
        
        /// <summary>
        /// 最初の画面を表示する
        /// </summary>
        private void setFirstImage()
        {
            AppShell shell = Window.Current.Content as AppShell;
            if (shell != null)
            {
                shell.HideMenu();
            }
        }
        #endregion

        #region 本棚処理メソッド
        /// <summary>
        /// 本棚リストを初期化する
        /// </summary>
        private void initBookCategory()
        {
            foreach (BookCategory category in m_db.QueryBookCategoryAll())
            {
                addBookCategoryMenuItem(category);
            }
        }

        /// <summary>
        /// 本棚アイテムを作成しメニューに追加する
        /// </summary>
        /// <param name="category"></param>
        private void addBookCategoryMenuItem(BookCategory category)
        {
            ToggleMenuFlyoutItem buttonItem = new ToggleMenuFlyoutItem()
            {
                Text = category.Label,
                Tag = category,
            };
            buttonItem.Click += BookshelfButtonItem_Click;
            if (m_db.QueryBookItemFromPath(category.Id, this.ViewModel.ParentStorage.Path) == null)
            {
                buttonItem.IsChecked = false;
            }
            else
            {
                buttonItem.IsChecked = true;
            }
            bookshelfMenu.Items.Add(buttonItem);
            bookshelfMenuBottom.Items.Add(buttonItem);
        }
        
        /// <summary>
        /// 新しい本棚を作成して追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NewBookShelfMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            InsertBookshelfDialog dialog = new InsertBookshelfDialog();
            await dialog.ShowAsync();
            if (!string.IsNullOrEmpty(dialog.Label))
            {
                BookCategory category = new BookCategory()
                {
                    Label = dialog.Label,
                };
                if (m_db.InsertBookCategory(category))
                {
                    addBookItem(category);
                    addBookCategoryMenuItem(category);
                }
            }
        }
        
        /// <summary>
        /// カテゴリに登録・解除を行う
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BookshelfButtonItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item == null)
            {
                return;
            }
            BookCategory category = item.Tag as BookCategory;
            if (category == null)
            {
                return;
            }

            Debug.WriteLine("label=" + category.Label + " toggle=" + item.IsChecked.ToString());
            if (item.IsChecked)
            {
                addBookItem(category);
            }
            else
            {
                removeBookItem(category);
            }
        }

        /// <summary>
        /// 指定した本棚カテゴリに追加する
        /// </summary>
        /// <param name="category"></param>
        private void addBookItem(BookCategory category)
        {
            string token = StorageHistoryManager.AddStorage(this.ViewModel.ParentStorage, StorageHistoryManager.DataType.Bookshelf);
            m_db.InsertBookItem(new BookItem()
            {
                BookCategoryId = category.Id,
                Token = token,
                Path = this.ViewModel.ParentStorage.Path,
                Uptime = DateTime.Now
            });
        }

        /// <summary>
        /// 指定した本棚カテゴリから削除する
        /// </summary>
        /// <param name="category"></param>
        private void removeBookItem(BookCategory category)
        {
            BookItem bookItem = m_db.QueryBookItemFromPath(category.Id, this.ViewModel.ParentStorage.Path);
            if (bookItem != null)
            {
                StorageHistoryManager.RemoveStorage(bookItem.Token, StorageHistoryManager.DataType.Bookshelf);
                m_db.DeleteBookItem(bookItem);
            }
        }
        #endregion

        #region スクラップブック処理メソッド
        /// <summary>
        /// スクラップブックカテゴリ一覧を作成する
        /// </summary>
        private void initScrapbookCategory()
        {
            foreach (ScrapbookCategory category in m_db.QueryScrapbookCategoryAll())
            {
                addScrapbookCategoryMenuItem(category);
            }
        }

        /// <summary>
        /// スクラップブックカテゴリをメニューに追加する
        /// </summary>
        /// <param name="category"></param>
        private void addScrapbookCategoryMenuItem(ScrapbookCategory category)
        {
            MenuFlyoutItem buttonItem = new MenuFlyoutItem()
            {
                Text = category.FolderName,
                Tag = category,
            };
            buttonItem.Click += ScrapbookButtonItem_Click;
            scrapbookMenu.Items.Add(buttonItem);
            scrapbookMenuBottom.Items.Add(buttonItem);
        }

        /// <summary>
        /// 新しいスクラップカテゴリを作成し登録する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NewScrapbookMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            InsertScrapbookDialog dialog = new InsertScrapbookDialog();
            await dialog.ShowAsync();
            if (!string.IsNullOrEmpty(dialog.FolderName))
            {
                ScrapbookCategory category = new ScrapbookCategory()
                {
                    FolderName = dialog.FolderName,
                };
                if (EnvPath.GetScrapbookSubFolder(category.FolderName) != null)
                {
                    if (m_db.InsertScrapbookCategory(category))
                    {
                        addScrapbookItem(category);
                        addScrapbookCategoryMenuItem(category);
                    }
                }
            }
        }

        /// <summary>
        /// スクラップ登録ボタンを押下した時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrapbookButtonItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            if (item == null)
            {
                return;
            }
            ScrapbookCategory category = item.Tag as ScrapbookCategory;
            if (category == null)
            {
                return;
            }

            Debug.WriteLine("FolderName=" + category.FolderName);
            addScrapbookItem(category);
        }
        
        /// <summary>
        /// スクラップカテゴリに現在表示している画像を登録する
        /// </summary>
        /// <param name="category"></param>
        private async void addScrapbookItem(ScrapbookCategory category)
        {
            ScrapbookItem item = await this.ViewModel.CreateScrapbookItem(category);
            if (item == null)
            {
                Debug.WriteLine("ファイルコピー失敗");
                return;
            }

            m_db.InsertScrapbookItem(item);
        }
        #endregion

        #region タッチ操作メソッド
        private void mainGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            PointerPoint point = e.GetCurrentPoint(this.mainGrid);
            Size areaSize = new Size(mainGrid.ActualWidth, mainGrid.ActualHeight);

            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                TouchArea area = getTouchArea(point.Position, areaSize);
                switch (area)
                {
                    case TouchArea.Top:
                        touchEventTop(areaSize);
                        break;
                    case TouchArea.TopLeft:
                        touchEventTopLeft();
                        break;
                    case TouchArea.TopRight:
                        touchEventTopRight();
                        break;
                    case TouchArea.BottomLeft:
                        touchEventBottomLeft();
                        break;
                    case TouchArea.BottomRight:
                        touchEventBottomRight();
                        break;
                }
            }
            else if (point.Properties.PointerUpdateKind == PointerUpdateKind.XButton1Released)
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                    e.Handled = true;
                    return;
                }
            }

            e.Handled = false;
        }

        /// <summary>
        /// 視程した位置からタッチエリアを取得する
        /// </summary>
        /// <param name="point"></param>
        /// <param name="areaSize"></param>
        /// <returns></returns>
        private TouchArea getTouchArea(Point point, Size areaSize)
        {
            double splitAreaHeight = (areaSize.Height - COMMAND_AREA_HEIGHT) / 2.0;
            double splitAreaWidth = areaSize.Width / 2.0;

            double topHeight = TOP_AREA_HEIGHT;
            if (titleOnlyBar.Visibility == Visibility.Collapsed 
            || topAppBar.Visibility == Visibility.Collapsed)
            {
                topHeight = TOP_AREA_HEIGHT + COMMAND_AREA_HEIGHT;
            }

            if (point.Y < 0)
            {
                return TouchArea.CommandArea;
            }
            else if (point.Y < topHeight)
            {
                return TouchArea.Top;
            }
            else if (point.Y < (splitAreaHeight + topHeight))
            {
                if (point.X < splitAreaWidth)
                {
                    return TouchArea.TopLeft;
                }
                else
                {
                    return TouchArea.TopRight;
                }
            }
            else
            {
                if (point.X < splitAreaWidth)
                {
                    return TouchArea.BottomLeft;
                }
                else
                {
                    return TouchArea.BottomRight;
                }
            }
        }

        /// <summary>
        /// コマンドエリアがタッチされた時の処理を行う
        /// </summary>
        /// <param name="areaSize"></param>
        private void touchEventTop(Size areaSize)
        {
            if (areaSize.Width < UI_MODE_MIN_WINDOW_WIDTH)
            {
                if (titleOnlyBar.Visibility == Visibility.Visible)
                {
                    titleOnlyBar.Visibility = Visibility.Collapsed;
                    bottomAppBar.Visibility = Visibility.Collapsed;
                    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                }
                else
                {
                    titleOnlyBar.Visibility = Visibility.Visible;
                    bottomAppBar.Visibility = Visibility.Visible;
                    ApplicationView.GetForCurrentView().ExitFullScreenMode();
                }
            }
            else
            {
                if (topAppBar.Visibility == Visibility.Visible)
                {
                    topAppBar.Visibility = Visibility.Collapsed;
                    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                }
                else
                {
                    topAppBar.Visibility = Visibility.Visible;
                    ApplicationView.GetForCurrentView().ExitFullScreenMode();
                }
            }
        }

        /// <summary>
        /// 左上エリアをタッチした時
        /// </summary>
        private void touchEventTopLeft()
        {
            this.ViewModel.SelectPrev();
        }

        /// <summary>
        /// 右上エリアをタッチした時
        /// </summary>
        private void touchEventTopRight()
        {
            this.ViewModel.SelectNext();
        }

        /// <summary>
        /// 左下エリアをタッチした時
        /// </summary>
        private void touchEventBottomLeft()
        {
            this.ViewModel.SelectPrev();
        }

        /// <summary>
        /// 右下エリアをタッチした時
        /// </summary>
        private void touchEventBottomRight()
        {
            this.ViewModel.SelectNext();
        }
        #endregion

        #region FlipView操作メソッド
        /// <summary>
        /// FlipView読み込み時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageFlipView_Loaded(object sender, RoutedEventArgs e)
        {
            hideFlipViewbutton((FlipView)sender, "PreviousButtonHorizontal");
            hideFlipViewbutton((FlipView)sender, "NextButtonHorizontal");
            hideFlipViewbutton((FlipView)sender, "PreviousButtonVertical");
            hideFlipViewbutton((FlipView)sender, "NextButtonVertical");
        }

        /// <summary>
        /// FlipViewの左右ボタンを非表示にする
        /// </summary>
        /// <param name="f"></param>
        /// <param name="name"></param>
        private void  hideFlipViewbutton(FlipView f, string name)
        {
            Button b;
            b = findVisualChild<Button>(f, name);
            b.Opacity = 0.0;
            b.IsHitTestVisible = false;
        }

        /// <summary>
        /// オブジェクトツリーから指定した名前のアイテムを取得
        /// 参考：https://social.msdn.microsoft.com/Forums/windowsapps/en-US/0ea7eaf4-3f11-4bcb-93f7-b5f6e6a02418/hiding-the-next-and-previous-buttons-of-a-flipview-programatically?forum=winappswithcsharp
        /// </summary>
        /// <typeparam name="childItemType"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private childItemType findVisualChild<childItemType>(DependencyObject obj, string name) where childItemType : FrameworkElement
        {
            // Exec
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is childItemType && ((FrameworkElement)child).Name == name)
                    return (childItemType)child;
                else
                {
                    childItemType childOfChild = findVisualChild<childItemType>(child, name);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 移動ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void JumpPageButton_Click(object sender, RoutedEventArgs e)
        {
            JumpPageDialog dialog = new JumpPageDialog();
            await dialog.ShowAsync();
            if (dialog.ViewModel.SelectedIndex != -1
            &&  dialog.ViewModel.SelectedIndex != this.ViewModel.SelectedIndex)
            {
                this.ViewModel.SelectedIndex = dialog.ViewModel.SelectedIndex;
            }
        }

        /// <summary>
        /// ページ領域フッターの背景サイズ変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void footerPageBackground_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ViewModel.UpdatePageBarMaxWidth(e.NewSize.Width);
        }
    }
}
