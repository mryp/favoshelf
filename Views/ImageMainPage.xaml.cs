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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace favoshelf.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ImageMainPage : Page
    {
        private ImageFolderViewModel m_viewModel;

        public ImageMainPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            m_viewModel = new ImageFolderViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            FolderListItem item = e.Parameter as FolderListItem;
            if (item == null)
            {
                return;
            }

            if (item.Type == FolderListItem.FileType.ImageFile)
            {
                setFirstImage(item);
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Left:
                    setNextImage();
                    break;
                case Windows.System.VirtualKey.Right:
                    setPrevImage();
                    break;
            }
        }
        
        private async void setFirstImage(FolderListItem item)
        {
            await m_viewModel.Init(item);
            imageView.Source = await m_viewModel.GetImage();
        }

        private async void setNextImage()
        {
            imageView.Source = await m_viewModel.GetNextImage();
        }

        private async void setPrevImage()
        {
            imageView.Source = await m_viewModel.GetPrevImage();
        }

        private enum TouchPosition
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }
        
        private void touchPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            PointerPoint point = e.GetCurrentPoint(this.mainGrid);
            TouchPosition touchPos = getTouchPosition(sender as UIElement);
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
                {
                    if (touchPos == TouchPosition.TopLeft || touchPos == TouchPosition.BottomLeft)
                    {
                        setNextImage();
                    }
                    else if (touchPos == TouchPosition.TopRight || touchPos == TouchPosition.BottomRight)
                    {
                        setPrevImage();
                    }
                }
            }
        }

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

        private void touchPanel_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint point = e.GetCurrentPoint(this.imageView);
                int mouseWheelDelta = point.Properties.MouseWheelDelta;
                Debug.WriteLine("WHEEL Delta=" + mouseWheelDelta);
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
