using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace favoshelf.Views
{
    /// <summary>
    /// 画像表示用ページ
    /// </summary>
    public sealed partial class ImageMainPage : Page
    {
        /// <summary>
        /// タッチ場所定義
        /// </summary>
        private enum TouchPosition
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }

        /// <summary>
        /// ビューモデル
        /// </summary>
        private ImageViewModelBase m_viewModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageMainPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        /// <summary>
        /// 画面遷移してきたときの処理
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            FolderListItem item = e.Parameter as FolderListItem;
            if (item == null)
            {
                return;
            }

            if (item.Type == FolderListItem.FileType.ImageFile)
            {
                m_viewModel = new ImageFolderViewModel();
            }
            else if (item.Type == FolderListItem.FileType.Archive)
            {
                m_viewModel = new ImageZipViewModel();
            }

            await m_viewModel.Init(item);
            this.DataContext = m_viewModel;
            setFirstImage(item);
        }

        /// <summary>
        /// ウィンドウでキャッチするキー入力イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Left:
                case Windows.System.VirtualKey.Down:
                    setNextImage();
                    break;
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.Up:
                    setPrevImage();
                    break;
            }
        }
        
        /// <summary>
        /// 最初の画面を表示する
        /// </summary>
        /// <param name="item"></param>
        private void setFirstImage(FolderListItem item)
        {
            m_viewModel.GetImage();
        }

        /// <summary>
        /// 次の画像を表示する
        /// </summary>
        private void setNextImage()
        {
            m_viewModel.GetNextImage();
        }

        /// <summary>
        /// 前の画像を表示する
        /// </summary>
        private void setPrevImage()
        {
            m_viewModel.GetPrevImage();
        }
        
        /// <summary>
        /// タッチ・クリック検知イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void touchPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            PointerPoint point = e.GetCurrentPoint(this.mainGrid);
            TouchPosition touchPos = getTouchPosition(sender as UIElement);
            
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.XButton1Released)
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            else if (touchPos == TouchPosition.TopLeft || touchPos == TouchPosition.BottomLeft)
            {
                setNextImage();
            }
            else if (touchPos == TouchPosition.TopRight || touchPos == TouchPosition.BottomRight)
            {
                setPrevImage();
            }
        }

        /// <summary>
        /// タッチされたパネルからタッチ位置を取得する
        /// </summary>
        /// <param name="touchControl"></param>
        /// <returns></returns>
        private TouchPosition getTouchPosition(UIElement touchControl)
        {
            if (touchControl == this.touchPanelTopLeft)
            {
                return TouchPosition.TopLeft;
            }
            if (touchControl == this.touchPanelTopRight)
            {
                return TouchPosition.TopRight;
            }
            if (touchControl == this.touchPanelBottomLeft)
            {
                return TouchPosition.BottomLeft;
            }
            if (touchControl == this.touchPanelBottomRight)
            {
                return TouchPosition.BottomRight;
            }

            return TouchPosition.None;
        }

        /// <summary>
        /// マウスホイールイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void touchPanel_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint point = e.GetCurrentPoint(this.imageView);
                int mouseWheelDelta = point.Properties.MouseWheelDelta;
                if (mouseWheelDelta > 0)    //上
                {
                    setPrevImage();
                }
                else
                {
                    setNextImage();
                }
            }
        }
    }
}
