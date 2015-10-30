//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Pink;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private  Pen trackedBonePen = new Pen(Brushes.Pink, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>

        /// <summary>
        /// Objeto que controlara la correcta ejecucion de los movimientos
        /// </summary>
        private Moves mov;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            mov = new Moves();
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            SkeletalImage.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug,
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {

                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.ColorImage.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = "No se ha encontrado ningún Kinect conectado.";
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            mov.movimiento(skel);
                            drawMovementRoute(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                                this.centerPointBrush,
                                null,
                                this.SkeletonPointToScreen(skel.Position),
                                BodyCenterThickness,
                                BodyCenterThickness);
                        }

                    }

                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        private void drawMovementRoute(Skeleton skeleton, DrawingContext drawingContext) {
            Point hombroIzq = SkeletonPointToScreen(skeleton.Joints[JointType.ShoulderLeft].Position);
            Point hombroDer = SkeletonPointToScreen(skeleton.Joints[JointType.ShoulderRight].Position);

            switch (mov.getPostura()) {
                case Moves.posturas.Inicial:
                    this.lbEstados.Foreground = Brushes.Blue;
                    this.lbEstados.Content = "Levante los brazos hasta estar en cruz";
                    //dibujo pos manos en cruz
                    drawingContext.DrawEllipse(Brushes.Blue, null, new Point(hombroIzq.X + (hombroIzq.Y - Math.Abs(mov.getManoIzqInicial().Y)), hombroIzq.Y), 10, 10);
                    drawingContext.DrawEllipse(Brushes.Blue, null, new Point(hombroDer.X - (hombroDer.Y - Math.Abs(mov.getManoDerInicial().Y)), hombroDer.Y), 10, 10);
                break;
                case Moves.posturas.Brazos_En_Cruz:
                    this.lbEstados.Foreground = Brushes.Blue;
                    this.lbEstados.Content = "Continúe el movimiento de los brazos hasta arriba";
                    //Meta conseguida brazos en cruz
                    drawingContext.DrawEllipse(Brushes.Green, null, new Point(hombroIzq.X + (hombroIzq.Y - Math.Abs(mov.getManoIzqInicial().Y)), hombroIzq.Y), 15, 15);
                    drawingContext.DrawEllipse(Brushes.Green, null, new Point(hombroDer.X - (hombroDer.Y - Math.Abs(mov.getManoDerInicial().Y)), hombroDer.Y), 15, 15);
                    //Marcamos el siguiente punto
                    drawingContext.DrawEllipse(Brushes.Blue, null, new Point(hombroIzq.X , hombroIzq.Y - Math.Abs(hombroIzq.Y - (mov.getManoIzqInicial().Y)) +10), 10, 10);
                    drawingContext.DrawEllipse(Brushes.Blue, null, new Point(hombroDer.X , hombroDer.Y - Math.Abs(hombroIzq.Y - (mov.getManoIzqInicial().Y)) +10), 10, 10);
                break;
                case Moves.posturas.Brazos_Arriba:
                    this.lbEstados.Foreground = Brushes.Green;
                    this.lbEstados.Content = "¡¡Perfecto!!";
                    //Meta conseguida brazos en cruz
                    drawingContext.DrawEllipse(Brushes.Green, null, new Point(hombroIzq.X + (hombroIzq.Y - Math.Abs(mov.getManoIzqInicial().Y)), hombroIzq.Y), 15, 15);
                    drawingContext.DrawEllipse(Brushes.Green, null, new Point(hombroDer.X - (hombroDer.Y - Math.Abs(mov.getManoDerInicial().Y)), hombroDer.Y), 15, 15);
                    //Meta conseguida brazos arriba
                    drawingContext.DrawEllipse(Brushes.Green, null, new Point(hombroIzq.X, hombroIzq.Y - Math.Abs(hombroIzq.Y - Math.Abs(mov.getManoIzqInicial().Y)) +10), 15, 15);
                    drawingContext.DrawEllipse(Brushes.Green, null, new Point(hombroDer.X, hombroDer.Y - Math.Abs(hombroIzq.Y - Math.Abs(mov.getManoIzqInicial().Y)) +10), 15, 15);
                break;
                case Moves.posturas.Mal:
                    this.lbEstados.Foreground = Brushes.Red;
                    this.lbEstados.Content = "Póngase en estado de Reposo";
                    break;

            }           
        }   
}
}