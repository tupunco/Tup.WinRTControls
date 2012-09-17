using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tup.WinRTControls.Controls
{
    /// <summary>
    /// 惰性 Value 改变 Slider LazyValue EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void LazySliderValueChangedEventHandler(object sender, LazySliderValueChangedEventArgs e);
    /// <summary>
    /// 惰性 Value 改变 Slider LazyValued EventArgs
    /// </summary>
    public sealed class LazySliderValueChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public LazySliderValueChangedEventArgs(double oldValue, double newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
        /// <summary>
        /// 
        /// </summary>
        public double NewValue { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double OldValue { get; private set; }
    }

    /// <summary>
    /// 惰性 Value 改变 Slider
    /// </summary>
    /// <remarks>
    /// 获取 Value 请使用 LazyValue 字段 
    /// </remarks>
    public class LazySlider : Slider
    {
        /// <summary>
        /// 
        /// </summary>
        public LazySlider()
        {
            DefaultStyleKey = typeof(LazySlider);
        }

        #region LazyValue 字段
        /// <summary>
        /// Identifies the LazyValue DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("LazyValue", typeof(double), typeof(LazySlider), new PropertyMetadata(0D, OnLazyValueChanged));
        /// <summary>
        /// Gets or sets the LazyValue.
        /// </summary>
        public double LazyValue
        {
            get { return (double)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void OnLazyValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is LazySlider)
            {
                var slider = (obj as LazySlider);
                var changed = slider.LazyValueChanged;
                if (changed != null)
                    changed(obj, new LazySliderValueChangedEventArgs((double)e.OldValue, (double)e.NewValue));

                if (slider.Value != slider.LazyValue)
                    slider.Value = slider.LazyValue;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            var value = this.Value;

            //INFO 鼠标离开时通知 LazyValue 字段改变
            if (this.Value > this.Maximum)
                value = this.Maximum;
            if (this.Value < this.Minimum)
                value = this.Minimum;

            this.LazyValue = value;
        }
        /// <summary>
        /// LazyValue 字段改变事件
        /// </summary>
        public event LazySliderValueChangedEventHandler LazyValueChanged;
    }
}
