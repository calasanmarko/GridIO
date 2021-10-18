using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlockIOInterface {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        [DllImport("GridIO.dll")]
        private static extern int MainRoutine();
        [DllImport("GridIO.dll")]
        private static extern void GetOutput(StringBuilder builder, int len);
        [DllImport("GridIO.dll")]
        private static extern void SetGridSettings(float startX, float startY, float startZ, int width, int height);
        [DllImport("GridIO.dll")]
        private static extern void SetTextureScale(float texScale);
        [DllImport("GridIO.dll")]
        private static extern void SetCameraPos(float camX, float camY, float camZ, float fov);
        [DllImport("GridIO.dll")]
        private static extern double GetFPS();
        [DllImport("GridIO.dll")]
        private static extern void CloseGLWindow();

        private StringBuilder debugBuilder;

        public MainWindow() {
            InitializeComponent();

            debugBuilder = new StringBuilder(300);
            System.Timers.Timer debugTimer = new System.Timers.Timer(100);
            debugTimer.Elapsed += DebugTimer_Elapsed;
            debugTimer.Start();

            Thread thread = new Thread(new ThreadStart(() => { MainRoutine(); }));
            thread.Start();

            System.Timers.Timer fpsTimer = new System.Timers.Timer(250);
            fpsTimer.Elapsed += FpsTimer_Elapsed;
            fpsTimer.Start();

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() => {
                gridStartX.TextChanged = gridParamsChanged;
                gridStartY.TextChanged = gridParamsChanged;
                gridStartZ.TextChanged = gridParamsChanged;
                gridWidth.TextChanged = gridParamsChanged;
                gridHeight.TextChanged = gridParamsChanged;
                textureScale.TextChanged = scaleChanged;
                cameraX.TextChanged = camChanged;
                cameraY.TextChanged = camChanged;
                cameraZ.TextChanged = camChanged;

                CommitGridSettings();
                CommitCameraSettings();
                CommitScale();
            }));
        }

        private void FpsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            double fps = GetFPS();
            Dispatcher.Invoke(new Action(() => { fpsLabel.Content = (int)fps; }));
        }

        private void DebugTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            GetOutput(debugBuilder, debugBuilder.Capacity);
            if (debugBuilder.Length > 0) {
                string output = debugBuilder.ToString();
                Action action = new Action(() => { DebugBox.Text += output; });
                Dispatcher.Invoke(action);
            }
            debugBuilder.Clear();
        }

        private void CommitGridSettings() {
            SetGridSettings(gridStartX.DecimalValue, gridStartY.DecimalValue, gridStartZ.DecimalValue, gridWidth.IntValue, gridHeight.IntValue);
        }

        private void CommitScale() {
            SetTextureScale(textureScale.DecimalValue);
        }

        private void CommitCameraSettings() {
            SetCameraPos(cameraX.DecimalValue, cameraY.DecimalValue, cameraZ.DecimalValue, 45);
        }

        private void gridParamsChanged(object sender, TextChangedEventArgs e) {
            CommitGridSettings();
        }

        private void scaleChanged(object sender, TextChangedEventArgs e) {
            CommitScale();
        }

        private void camChanged(object sender, TextChangedEventArgs e) {
            CommitCameraSettings();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            
        }

        private void Window_Closed(object sender, EventArgs e) {
            CloseGLWindow();
        }
    }
}
