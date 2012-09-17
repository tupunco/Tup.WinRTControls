using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace Tup.WinRTControls.Controls
{
    /// <summary>
    /// 遮挡层
    /// </summary>
    /// <remarks>
    /// FROM:Coding4Fun.Phone.Controls
    /// </remarks>
    //[ContentProperty(Name = "Content")]
    public class ProgressOverlay : ContentControl
    {
        private Storyboard _fadeIn;
        private Storyboard _fadeOut;
        private Grid _layoutGrid;

        private const string FadeInName = "fadeIn";
        private const string FadeOutName = "fadeOut";
        private const string LayoutGridName = "LayoutGrid";

        public ProgressOverlay()
        {
            DefaultStyleKey = typeof(ProgressOverlay);
        }

        /*
        public object ProgressControl
        {
            get { return (object)GetValue(ProgressControlProperty); }
            set { SetValue(ProgressControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressControlProperty =
            DependencyProperty.Register("ProgressControl", typeof(object), typeof(ProgressOverlay), new PropertyMetadata(null));

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ProgressOverlay), new PropertyMetadata(null));
        */
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!DesignMode.DesignModeEnabled)
            {
                _fadeIn = GetTemplateChild(FadeInName) as Storyboard;
                _fadeOut = GetTemplateChild(FadeOutName) as Storyboard;
                _layoutGrid = GetTemplateChild(LayoutGridName) as Grid;

                if (_fadeOut != null)
                    _fadeOut.Completed += fadeOut_Completed;
            }
        }

        void fadeOut_Completed(object sender, object e)
        {
            _layoutGrid.Opacity = 1;
            Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示当前遮挡层
        /// </summary>
        public void Show()
        {
            if (_fadeIn == null)
                ApplyTemplate();

            Visibility = Visibility.Visible;

            if (_fadeOut != null)
                _fadeOut.Stop();

            if (_fadeIn != null)
                _fadeIn.Begin();
        }
        /// <summary>
        /// 隐藏当前遮挡层
        /// </summary>
        public void Hide()
        {
            if (_fadeOut == null)
                ApplyTemplate();

            if (_fadeIn != null)
                _fadeIn.Stop();

            if (_fadeOut != null)
                _fadeOut.Begin();
        }
        /// <summary>
        /// 当前遮挡层显示与否
        /// </summary>
        /// <returns></returns>
        public bool IsVisibility()
        {
            return this.Visibility == Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
