using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Threading;
using GridIOInterface;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GridIOInterface {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static ECSFramework framework => ECSFramework.instance;
        private StringBuilder debugBuilder;
        private float[] pixels;
        private int renderWidth;
        private int renderHeight;
        private System.Timers.Timer pixelTimer;
        public EntityBarManager entityBarManager { get; private set; }
        public ComponentBarManager componentBarManager { get; private set; }

        public MainWindow() {
            InitializeComponent();
            entityBarManager = new EntityBarManager(this, entityBar);
            componentBarManager = new ComponentBarManager(this, componentBar);
        }

        private void PixelTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            Dispatcher.Invoke(new Action(() => {
                framework.UpdateSystems();
                IntPtr pixelPtr = OpenGL.DrawFrame();
                if (pixelPtr != IntPtr.Zero) {
                    Marshal.Copy(pixelPtr, pixels, 0, renderWidth * renderHeight * 4);
                    int stride = ((renderWidth * 32 + 31) & ~31) / 8;
                    BitmapSource source = BitmapSource.Create(renderWidth, renderHeight, 1, 1, PixelFormats.Rgba128Float, null, pixels, stride * 4);
                    renderImage.Source = source;
                }
            }));
        }

        private void Window_Closed(object sender, EventArgs e) {
            pixelTimer.Stop();
            OpenGL.CloseGLWindow();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            CalcRenderSize();
            pixels = new float[renderWidth * renderHeight * 4];

            OpenGL.SetSize(renderWidth, renderHeight);
            OpenGL.InitContext();
            framework.StartSystems();

            pixelTimer = new System.Timers.Timer(1);
            pixelTimer.Elapsed += PixelTimer_Elapsed;
            pixelTimer.Start();
        }

        private void CalcRenderSize() {
            renderWidth = (int)renderImage.Width;
            renderHeight = (int)renderImage.Height;
        }

        private void ImageGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
            renderImage.Width = e.NewSize.Width;
            renderImage.Height = e.NewSize.Width * (9.0 / 16.0);
        }
    }
}
