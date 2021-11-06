using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GridIOInterface {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static ECSFramework framework => ECSFramework.instance;
        public EntityBarManager entityBarManager { get; private set; }
        public ComponentBarManager componentBarManager { get; private set; }
        public RenderManager renderManager { get; private set; }

        private readonly StringBuilder outputBuilder;
        private readonly Timer fpsUpdateTimer;
        private Vector2 startMousePos;

        private static readonly int maxOutputLength = 1024;
        private static readonly int renderWidth = 1280;
        private static readonly int renderHeight = 720;

        public MainWindow() {
            InitializeComponent();
            ProjectLoader.Open("save.txt");
            entityBarManager = new EntityBarManager(this, entityBar);
            componentBarManager = new ComponentBarManager(this, componentBar);
            outputBuilder = new StringBuilder(maxOutputLength);
            fpsUpdateTimer = new Timer(5);
            fpsUpdateTimer.Elapsed += FpsUpdateTimer_Elapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            renderManager = new RenderManager(this, renderImage);
            renderManager.RequestPreview();

            Timer outputTimer = new Timer(50);
            outputTimer.Elapsed += OutputTimer_Elapsed;
            outputTimer.Start();
        }

        private void Window_Closed(object sender, EventArgs e) {
            renderManager.SyncStopFrameLoop();
            OpenGL.CloseGLWindow();
        }

        private void FpsUpdateTimer_Elapsed(object sender, ElapsedEventArgs e) {
            Dispatcher.Invoke(() => fpsLabel.Content = "FPS " + Marshal.ReadInt32(renderManager.currentFPS));
        }

        public void StartFPSUpdateTimer() {
            fpsUpdateTimer.Start();
        }

        public void StopFPSUpdateTimer() {
            fpsUpdateTimer.Stop();
        }

        private void PrintOutput(string output) {
            Label label = new Label() { Content = output };
            outputPanel.Children.Add(label);
        }

        private void OutputTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            OpenGL.GetOutput(outputBuilder, maxOutputLength);
            string newOutput = outputBuilder.ToString();

            if (newOutput.Length > 0) {
                outputBuilder.Clear();

                Dispatcher.Invoke(new Action(() => {
                    PrintOutput(newOutput);
                }));
            }
        }

        private void ImageGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
            renderImage.Width = e.NewSize.Width;
            renderImage.Height = e.NewSize.Width * (9.0 / 16.0);
        }

        private void addEntityButton_Click(object sender, RoutedEventArgs e) {
            entityBarManager.AddEntityLabel(framework.CreateEntity("New Entity"));
        }

        private void playButton_Click(object sender, RoutedEventArgs e) {
            renderManager.SyncStopFrameLoop();
            renderManager.StartPlayLoop();
            playButton.IsEnabled = false;
            pauseButton.IsEnabled = true;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e) {
            renderManager.SyncStopFrameLoop();
            renderManager.RequestPreview();
            playButton.IsEnabled = true;
            pauseButton.IsEnabled = false;
        }


        private Vector2 MousePosInRender() {
            Point mousePoint = Mouse.GetPosition(renderImage);
            return new Vector2((float)(mousePoint.X * (renderWidth / renderImage.Width)),
                               (float)(mousePoint.Y * (renderHeight / renderImage.Height)));
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            ProjectSaver.Save("save.txt");
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void renderImage_MouseWheel(object sender, MouseWheelEventArgs e) {
            framework.previewCamera.entity.transform.position += new Vector3(0, 0, e.Delta / 150f);
            renderManager.RequestPreview();
        }

        private void renderImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            if (!renderManager.playStarted) {
                startMousePos = framework.previewCamera.ScreenToWorldPos(MousePosInRender());
                renderImage.CaptureMouse();
                renderImage.Cursor = Cursors.Hand;
            }
        }

        private void renderImage_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            if (!renderManager.playStarted) {
                renderImage.ReleaseMouseCapture();
                renderImage.Cursor = null;
            }
        }

        private void renderImage_MouseMove(object sender, MouseEventArgs e) {
            if (renderImage.IsMouseCaptured) {
                Vector2 mousePos = framework.previewCamera.ScreenToWorldPos(MousePosInRender());
                Vector2 mouseDiff = mousePos - startMousePos;
                framework.previewCamera.entity.transform.position += mouseDiff;
                renderManager.RequestPreview();
            }
        }
    }
}
