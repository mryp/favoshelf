using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace favoshelf.Controls
{
    public class FolderGridView : GridView
    {
        private ScrollViewer _sv;
        
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _sv = (ScrollViewer)this.GetTemplateChild("ScrollViewer");
            _sv.SizeChanged += _sv_SizeChanged;
        }
        
        public double HorizontalOffset
        {
            get
            {
                return (_sv == null) ? 0.0 : _sv.HorizontalOffset;
            }
        }
        
        public void ScrollToHorizontalOffset(double value)
        {
            if (_sv != null)
                _sv.ScrollToHorizontalOffset(value);
        }

        public double VerticalOffset
        {
            get
            {
                return (_sv == null) ? 0.0 : _sv.VerticalOffset;
            }
        }
        
        public void ScrollToVerticalOffset(double value)
        {
            if (_sv != null)
                _sv.ScrollToVerticalOffset(value);
        }

        // ScrollViewerコントロールのSizeChangedイベントを公開する
        public event EventHandler<SizeChangedEventArgs> ScrollViewerSizeChanged;
        void _sv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ScrollViewerSizeChanged != null)
                ScrollViewerSizeChanged.Invoke(this, e);
        }
    }
}
