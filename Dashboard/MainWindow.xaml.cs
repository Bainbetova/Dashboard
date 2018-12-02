using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dashboard
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<AssetClass> classes;

        public MainWindow()
        {
            InitializeComponent();

            classes = new ObservableCollection<AssetClass>(AssetClass.ConstructTestData());
            this.DataContext = classes;
        }

        private void OnColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumn column = ((GridViewColumnHeader)e.OriginalSource).Column;
            piePlotter.PlottedProperty = column.Header.ToString();
        }

        private void AddNewItem(object sender, RoutedEventArgs e)
        {
            AssetClass asset = new AssetClass { Category = "Источник расходов" };
            classes.Add(asset);
        }
    }
}