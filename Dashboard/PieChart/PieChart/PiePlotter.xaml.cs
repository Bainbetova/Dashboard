using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace Dashboard
{
    public partial class PiePlotter : UserControl
    {
        #region dependency properties

        /// The property of the bound object that will be plotted
        public String PlottedProperty
        {
            get { return PieChartLayout.GetPlottedProperty(this); }
            set { PieChartLayout.SetPlottedProperty(this, value); }
        }

        /// A class which selects a color based on the item being rendered.
        public IColorSelector ColorSelector
        {
            get { return PieChartLayout.GetColorSelector(this); }
            set { PieChartLayout.SetColorSelector(this, value); }
        }

        /// The size of the hole in the centre of circle (as a percentage)
        public double HoleSize
        {
            get { return (double)GetValue(HoleSizeProperty); }
            set
            {
                SetValue(HoleSizeProperty, value);
                ConstructPiePieces();
            }
        }

        public static readonly DependencyProperty HoleSizeProperty =
                       DependencyProperty.Register("HoleSize", typeof(double), typeof(PiePlotter), new UIPropertyMetadata(0.0));
        #endregion

        private List<PiePiece> piePieces = new List<PiePiece>();

        public PiePlotter()
        {
            // register any dependency property change handlers
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(PieChartLayout.PlottedPropertyProperty, typeof(PiePlotter));
            dpd.AddValueChanged(this, PlottedPropertyChanged);

            InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(DataContextChangedHandler);
        }

        #region property change handlers

        void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            // handle the events that occur when the bound collection changes
            if (this.DataContext is INotifyCollectionChanged)
            {
                INotifyCollectionChanged observable = (INotifyCollectionChanged)this.DataContext;
                observable.CollectionChanged += new NotifyCollectionChangedEventHandler(BoundCollectionChanged);
            }

            // handle the selection change events
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.DataContext);
            collectionView.CurrentChanged += new EventHandler(CollectionViewCurrentChanged);
            collectionView.CurrentChanging += new CurrentChangingEventHandler(CollectionViewCurrentChanging);

            ConstructPiePieces();
            ObserveBoundCollectionChanges();
        }

        private void PlottedPropertyChanged(object sender, EventArgs e)
        {
            ConstructPiePieces();
        }

        #endregion

        #region event handlers
        void PiePieceMouseUp(object sender, MouseButtonEventArgs e)
        {
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.DataContext);
            if (collectionView == null)
                return;

            PiePiece piece = sender as PiePiece;
            if (piece == null)
                return;

            // select the item which this pie piece represents
            int index = (int)piece.Tag;
            collectionView.MoveCurrentToPosition(index);
        }

        void CollectionViewCurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            CollectionView collectionView = (CollectionView)sender;

            if (collectionView != null && collectionView.CurrentPosition >= 0 && collectionView.CurrentPosition <= piePieces.Count)
            {
                PiePiece piece = piePieces[collectionView.CurrentPosition];

                DoubleAnimation a = new DoubleAnimation();
                a.To = 0;
                a.Duration = new Duration(TimeSpan.FromMilliseconds(200));

                piece.BeginAnimation(PiePiece.PushOutProperty, a);
            }
        }

        void CollectionViewCurrentChanged(object sender, EventArgs e)
        {
            CollectionView collectionView = (CollectionView)sender;

            if (collectionView != null && collectionView.CurrentPosition >= 0 && collectionView.CurrentPosition <= piePieces.Count)
            {
                PiePiece piece = piePieces[collectionView.CurrentPosition];

                DoubleAnimation a = new DoubleAnimation();
                a.To = 10;
                a.Duration = new Duration(TimeSpan.FromMilliseconds(200));

                piece.BeginAnimation(PiePiece.PushOutProperty, a);
            }
        }
        
        private void BoundCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ConstructPiePieces();
            ObserveBoundCollectionChanges();
        }

        /// <summary>
        /// Iterates over the items inthe bound collection, adding handlers for PropertyChanged events
        /// </summary>
        private void ObserveBoundCollectionChanges()
        {
            CollectionView myCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.DataContext);

            foreach (object item in myCollectionView)
            {
                if (item is INotifyPropertyChanged)
                {
                    INotifyPropertyChanged observable = (INotifyPropertyChanged)item;
                    observable.PropertyChanged += new PropertyChangedEventHandler(ItemPropertyChanged);
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // if the property which this pie chart represents has changed, re-construct the pie
            if (e.PropertyName.Equals(PlottedProperty))
            {
                ConstructPiePieces();
            }
        }

        #endregion

        private double GetPlottedPropertyValue(object item)
        {
            PropertyDescriptorCollection filterPropDesc = TypeDescriptor.GetProperties(item);
            object itemValue = filterPropDesc[PlottedProperty].GetValue(item);

            return (double)itemValue;
        }

        /// Constructs pie pieces and adds them to the visual tree for this control's canvas
        private void ConstructPiePieces()
        {
            CollectionView myCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.DataContext);
            if (myCollectionView == null)
                return;

            double halfWidth = this.Width / 2;
            double innerRadius = halfWidth * HoleSize;

            // compute the total for the property which is being plotted
            double total = 0;
            foreach (Object item in myCollectionView)
            {
                total += GetPlottedPropertyValue(item);
            }

            // add the pie pieces
            canvas.Children.Clear();
            piePieces.Clear();

            double accumulativeAngle = 0;
            foreach (Object item in myCollectionView)
            {
                bool selectedItem = item == myCollectionView.CurrentItem;

                double wedgeAngle = GetPlottedPropertyValue(item) * 360 / total;

                PiePiece piece = new PiePiece()
                {
                    Radius = halfWidth,
                    InnerRadius = innerRadius,
                    CentreX = halfWidth,
                    CentreY = halfWidth,
                    PushOut = (selectedItem ? 10.0 : 0),
                    WedgeAngle = wedgeAngle,
                    RotationAngle = accumulativeAngle,
                    Fill = ColorSelector != null ? ColorSelector.SelectBrush(myCollectionView.IndexOf(item)) : Brushes.Black,
                    Tag = myCollectionView.IndexOf(item),
                };

                piece.MouseUp += new MouseButtonEventHandler(PiePieceMouseUp);

                piePieces.Add(piece);
                canvas.Children.Insert(0, piece);

                accumulativeAngle += wedgeAngle;
            }
        }
    }
}