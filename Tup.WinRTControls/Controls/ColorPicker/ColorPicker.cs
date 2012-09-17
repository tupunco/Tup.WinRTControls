﻿using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Tup.WinRTControls.Controls
{
    /// <summary>
    /// Represents a Color Picker control which allows a user to select a color.
    /// </summary>
    /// <remarks>
    /// FROM: http://silverlightcontrib.codeplex.com
    /// DEMO: http://silverlightcontrib.codeplex.com/wikipage?title=Feature%20Overview&referringTitle=Home&ANCHOR#ColorPicker
    ///       http://blogs.msdn.com/b/kirillosenkov/archive/2009/09/25/colorpicker-control-for-wpf-silverlight.aspx
    /// </remarks>
    public class ColorPicker : Control
    {
        /// <summary>
        /// Event fired when the selected color changes.  This event occurs when the 
        /// left-mouse button is lifted after clicking.
        /// </summary>
        public event SelectedColorChangedHandler SelectedColorChanged;

        /// <summary>
        /// Event fired when the selected color is changing.  This event occurs when the 
        /// left-mouse button is pressed and the user is moving the mouse.
        /// </summary>
        public event SelectedColorChangingHandler SelectedColorChanging;

        private readonly ColorSpace m_colorSpace;
        private bool m_hueMonitorMouseCaptured;
        private bool m_sampleMouseCaptured;
        private double m_huePos;
        private double m_sampleX;
        private double m_sampleY;

        private Panel m_rootElement;
        private Rectangle m_hueMonitor;
        private Canvas m_sampleSelector;
        private Canvas m_hueSelector;

        private Rectangle m_selectedColorView;
        private Rectangle m_colorSample;
        private TextBlock m_hexValue;
        private ScaleTransform m_scale;

        /// <summary>
        /// Create a new instance of the ColorPicker control.
        /// </summary>
        public ColorPicker()
        {
            DefaultStyleKey = typeof(ColorPicker);
            m_colorSpace = new ColorSpace();
        }

        /// <summary>
        /// Builds the visual tree for the ColorPicker control when the template is applied. 
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_rootElement = GetTemplateChild("RootElement") as Panel;
            m_hueMonitor = GetTemplateChild("HueMonitor") as Rectangle;
            m_sampleSelector = GetTemplateChild("SampleSelector") as Canvas;
            m_hueSelector = GetTemplateChild("HueSelector") as Canvas;
            m_selectedColorView = GetTemplateChild("SelectedColorView") as Rectangle;
            m_colorSample = GetTemplateChild("ColorSample") as Rectangle;
            m_hexValue = GetTemplateChild("HexValue") as TextBlock;


            m_rootElement.RenderTransform = m_scale = new ScaleTransform();

            m_hueMonitor.PointerPressed += m_hueMonitor_PointerPressed;
            m_hueMonitor.PointerReleased += m_hueMonitor_PointerReleased;
            m_hueMonitor.PointerMoved += m_hueMonitor_PointerMoved;

            m_colorSample.PointerPressed += m_colorSample_PointerPressed;
            m_colorSample.PointerReleased += m_colorSample_PointerReleased;
            m_colorSample.PointerMoved += m_colorSample_PointerMoved;

            m_sampleX = m_colorSample.Width;
            m_sampleY = 0;
            m_huePos = 0;

            UpdateVisuals();
        }



        #region hueMonitor 鼠标事件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_hueMonitor_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var cPoint = e.GetCurrentPoint((UIElement)sender);
            if (cPoint == null || e.Pointer == null)
                return;

            m_hueMonitorMouseCaptured = m_hueMonitor.CapturePointer(e.Pointer);
            DragSliders(0, cPoint.Position.Y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_hueMonitor_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            m_hueMonitor.ReleasePointerCaptures();
            m_hueMonitorMouseCaptured = false;
            SetValue(SelectedColorProperty, GetColor());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_hueMonitor_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!m_hueMonitorMouseCaptured)
                return;

            var cPoint = e.GetCurrentPoint((UIElement)sender);
            if (cPoint == null || e.Pointer == null)
                return;

            DragSliders(0, cPoint.Position.Y);
        }
        #endregion

        #region colorSample 鼠标事件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_colorSample_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var cPoint = e.GetCurrentPoint((UIElement)sender);
            if (cPoint == null || e.Pointer == null)
                return;

            m_sampleMouseCaptured = m_colorSample.CapturePointer(e.Pointer);
            var pos = cPoint.Position;
            DragSliders(pos.X, pos.Y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_colorSample_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            m_colorSample.ReleasePointerCaptures();
            m_sampleMouseCaptured = false;
            SetValue(SelectedColorProperty, GetColor());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_colorSample_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!m_sampleMouseCaptured)
                return;

            var cPoint = e.GetCurrentPoint((UIElement)sender);
            if (cPoint == null || e.Pointer == null)
                return;

            Point pos = cPoint.Position;
            DragSliders(pos.X, pos.Y);
        }
        #endregion

        private Color GetColor()
        {
            double yComponent = 1 - (m_sampleY / m_colorSample.Height);
            double xComponent = m_sampleX / m_colorSample.Width;
            double hueComponent = (m_huePos / m_hueMonitor.Height) * 360;

            return m_colorSpace.ConvertHsvToRgb(hueComponent, xComponent, yComponent);
        }

        private void UpdateSatValSelection()
        {
            if (m_colorSample == null)
                return;

            m_sampleSelector.SetValue(Canvas.LeftProperty, m_sampleX - (m_sampleSelector.Height / 2));
            m_sampleSelector.SetValue(Canvas.TopProperty, m_sampleY - (m_sampleSelector.Height / 2));

            Color currColor = GetColor();
            m_selectedColorView.Fill = new SolidColorBrush(currColor);
            m_hexValue.Text = m_colorSpace.GetHexCode(currColor);

            FireSelectedColorChangingEvent(currColor);
        }

        private void UpdateHueSelection()
        {
            if (m_hueMonitor == null)
                return;
            double huePos = m_huePos / m_hueMonitor.Height * 255;
            Color c = m_colorSpace.GetColorFromPosition(huePos);
            m_colorSample.Fill = new SolidColorBrush(c);

            m_hueSelector.SetValue(Canvas.TopProperty, m_huePos - (m_hueSelector.Height / 2));

            Color currColor = GetColor();

            m_selectedColorView.Fill = new SolidColorBrush(currColor);
            m_hexValue.Text = m_colorSpace.GetHexCode(currColor);

            FireSelectedColorChangingEvent(currColor);
        }

        private void UpdateVisuals()
        {
            if (m_hueMonitor == null)
                return;

            Color c = this.SelectedColor;
            ColorSpace cs = new ColorSpace();
            HSV hsv = cs.ConvertRgbToHsv(c);

            m_huePos = (hsv.Hue / 360 * m_hueMonitor.Height);
            m_sampleY = -1 * (hsv.Value - 1) * m_colorSample.Height;
            m_sampleX = hsv.Saturation * m_colorSample.Width;
            if (!double.IsNaN(m_huePos))
                UpdateHueSelection();
            UpdateSatValSelection();
        }

        private void DragSliders(double x, double y)
        {
            if (m_hueMonitorMouseCaptured)
            {
                if (y < 0)
                    m_huePos = 0;
                else if (y > m_hueMonitor.Height)
                    m_huePos = m_hueMonitor.Height;
                else
                    m_huePos = y;
                UpdateHueSelection();
            }
            else if (m_sampleMouseCaptured)
            {
                if (x < 0)
                    m_sampleX = 0;
                else if (x > m_colorSample.Width)
                    m_sampleX = m_colorSample.Width;
                else
                    m_sampleX = x;

                if (y < 0)
                    m_sampleY = 0;
                else if (y > m_colorSample.Height)
                    m_sampleY = m_colorSample.Height;
                else
                    m_sampleY = y;

                UpdateSatValSelection();
            }
        }

        /// <summary>
        /// Called by the layout system during a layout pass.
        /// </summary>
        /// <param name="availableSize">The size available to the child elements.</param>
        /// <returns>The size set by the child elements.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size();
            if (m_rootElement != null)
            {
                m_rootElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size desiredSize = m_rootElement.DesiredSize;

                Size scale = ComputeScaleFactor(availableSize, desiredSize);

                size.Width = scale.Width * desiredSize.Width;
                size.Height = scale.Height * desiredSize.Height;
            }

            return size;
        }

        /// <summary>
        /// Called by the layout system during a layout pass.
        /// </summary>
        /// <param name="finalSize">The size determined to be availble to the child elements.</param>
        /// <returns>The final size used by the child elements.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (m_rootElement != null)
            {
                // Determine the scale factor given the final size
                Size desiredSize = m_rootElement.DesiredSize;
                Size scale = ComputeScaleFactor(finalSize, desiredSize);

                m_scale.ScaleX = scale.Width;
                m_scale.ScaleY = scale.Height;

                // Position the ChildElement to fill the ChildElement
                Rect originalPosition = new Rect(0, 0, desiredSize.Width, desiredSize.Height);
                m_rootElement.Arrange(originalPosition);

                // Determine the final size used by the Viewbox
                finalSize.Width = scale.Width * desiredSize.Width;
                finalSize.Height = scale.Height * desiredSize.Height;
            }
            return finalSize;
        }


        private Size ComputeScaleFactor(Size availableSize, Size contentSize)
        {
            double scaleX = 1.0;
            double scaleY = 1.0;

            bool isConstrainedWidth = !double.IsPositiveInfinity(availableSize.Width);
            bool isConstrainedHeight = !double.IsPositiveInfinity(availableSize.Height);

            // Don't scale if we shouldn't stretch or the scaleX and scaleY are both infinity.
            if (isConstrainedWidth || isConstrainedHeight)
            {
                // Compute the individual scaleX and scaleY scale factors
                scaleX = contentSize.Width == 0 ? 0.0 : (availableSize.Width / contentSize.Width);
                scaleY = contentSize.Height == 0 ? 0.0 : (availableSize.Height / contentSize.Height);

                // Make the scale factors uniform by setting them both equal to
                // the larger or smaller (depending on infinite lengths and the
                // Stretch value)
                if (!isConstrainedWidth)
                {
                    scaleX = scaleY;
                }
                else if (!isConstrainedHeight)
                {
                    scaleY = scaleX;
                }
            }

            return new Size(scaleX, scaleY);
        }

        private void FireSelectedColorChangingEvent(Color selectedColor)
        {
            if (SelectedColorChanging != null)
            {
                SelectedColorEventArgs args = new SelectedColorEventArgs(selectedColor);
                SelectedColorChanging(this, args);
            }
        }

        #region SelectedColor Dependency Property
        /// <summary>
        /// Gets or sets the currently selected color in the Color Picker.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set
            {
                SetValue(SelectedColorProperty, value);
                this.UpdateVisuals();
            }
        }

        /// <summary>
        /// SelectedColor Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                "SelectedColor",
                typeof(Color),
                typeof(ColorPicker),
                new PropertyMetadata(Colors.Blue, new PropertyChangedCallback(SelectedColorPropertyChanged)));

        private static void SelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker p = d as ColorPicker;
            if (p != null && p.SelectedColorChanged != null)
            {
                SelectedColorEventArgs args = new SelectedColorEventArgs((Color)e.NewValue);
                p.SelectedColorChanged(p, args);
            }
        }
        #endregion
    }
}