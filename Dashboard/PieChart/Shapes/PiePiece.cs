using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace Dashboard
{
    /// <summary>
    /// A pie piece shape
    /// </summary>
    public class PiePiece : Shape
    {
        #region dependency properties

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("RadiusProperty", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The radius of this pie piece
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty PushOutProperty =
            DependencyProperty.Register("PushOutProperty", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The distance to 'push' this pie piece out from the centre.
        /// </summary>
        public double PushOut
        {
            get { return (double)GetValue(PushOutProperty); }
            set { SetValue(PushOutProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadiusProperty", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The inner radius of this pie piece
        /// </summary>
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty WedgeAngleProperty =
            DependencyProperty.Register("WedgeAngleProperty", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The wedge angle of this pie piece in degrees
        /// </summary>
        public double WedgeAngle
        {
            get { return (double)GetValue(WedgeAngleProperty); }
            set
            {
                SetValue(WedgeAngleProperty, value);
            }
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngleProperty", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The rotation, in degrees, from the Y axis vector of this pie piece.
        /// </summary>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        public static readonly DependencyProperty CentreXProperty =
            DependencyProperty.Register("CentreXProperty", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The X coordinate of centre of the circle from which this pie piece is cut.
        /// </summary>
        public double CentreX
        {
            get { return (double)GetValue(CentreXProperty); }
            set { SetValue(CentreXProperty, value); }
        }

        public static readonly DependencyProperty CentreYProperty =
            DependencyProperty.Register("CentreYProperty", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The Y coordinate of centre of the circle from which this pie piece is cut.
        /// </summary>
        public double CentreY
        {
            get { return (double)GetValue(CentreYProperty); }
            set { SetValue(CentreYProperty, value); }
        }

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("PercentageProperty", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0));

        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    DrawGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        /// <summary>
        /// Draws the pie piece
        /// </summary>
        private void DrawGeometry(StreamGeometryContext context)
        {
            Point startPoint = new Point(CentreX, CentreY);

            Point innerArcStartPoint = CalculateCoordinate(RotationAngle, InnerRadius);
            innerArcStartPoint.Offset(CentreX, CentreY);

            Point innerArcEndPoint = CalculateCoordinate(RotationAngle + WedgeAngle, InnerRadius);
            innerArcEndPoint.Offset(CentreX, CentreY);

            Point outerArcStartPoint = CalculateCoordinate(RotationAngle, Radius);
            outerArcStartPoint.Offset(CentreX, CentreY);

            Point outerArcEndPoint = CalculateCoordinate(RotationAngle + WedgeAngle, Radius);
            outerArcEndPoint.Offset(CentreX, CentreY);

            bool largeArc = WedgeAngle > 180.0;

            if (PushOut > 0)
            {
                Point offset = CalculateCoordinate(RotationAngle + WedgeAngle / 2, PushOut);
                innerArcStartPoint.Offset(offset.X, offset.Y);
                innerArcEndPoint.Offset(offset.X, offset.Y);
                outerArcStartPoint.Offset(offset.X, offset.Y);
                outerArcEndPoint.Offset(offset.X, offset.Y);
            }

            Size outerArcSize = new Size(Radius, Radius);
            Size innerArcSize = new Size(InnerRadius, InnerRadius);

            context.BeginFigure(innerArcStartPoint, true, true);
            context.LineTo(outerArcStartPoint, true, true);
            context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
            context.LineTo(innerArcEndPoint, true, true);
            context.ArcTo(innerArcStartPoint, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, true, true);
        }

        public Point CalculateCoordinate(double angle, double radius)
        {
            // convert to radians
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }
    }
}