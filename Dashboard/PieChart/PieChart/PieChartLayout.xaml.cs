using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dashboard
{
    public partial class PieChartLayout : UserControl
    {
        #region dependency properties

        /// The property of the bound object that will be plotted (CLR wrapper)
        public String PlottedProperty
        {
            get { return GetPlottedProperty(this); }
            set { SetPlottedProperty(this, value); }
        }

        // PlottedProperty dependency property
        public static readonly DependencyProperty PlottedPropertyProperty =
                       DependencyProperty.RegisterAttached("PlottedProperty", typeof(String), typeof(PieChartLayout),
                       new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits));

        // PlottedProperty attached property accessors
        public static void SetPlottedProperty(UIElement element, String value)
        {
            element.SetValue(PlottedPropertyProperty, value);
        }
        public static String GetPlottedProperty(UIElement element)
        {
            return (String)element.GetValue(PlottedPropertyProperty);
        }

        public IColorSelector ColorSelector
        {
            get { return GetColorSelector(this); }
            set { SetColorSelector(this, value); }
        }

        // ColorSelector dependency property
        public static readonly DependencyProperty ColorSelectorProperty =
                       DependencyProperty.RegisterAttached("ColorSelectorProperty", typeof(IColorSelector), typeof(PieChartLayout),
                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        // ColorSelector attached property accessors
        public static void SetColorSelector(UIElement element, IColorSelector value)
        {
            element.SetValue(ColorSelectorProperty, value);
        }
        public static IColorSelector GetColorSelector(UIElement element)
        {
            return (IColorSelector)element.GetValue(ColorSelectorProperty);
        }

        #endregion
        public PieChartLayout()
        {
            InitializeComponent();
        }
    }
}
