using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Timers;
using GridIOInterface;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Interop;
using System.Windows.Input;
using System.Reflection;
using System.Linq;

namespace GridIOInterface {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public static ECSFramework framework => ECSFramework.instance;
        public EntityBarManager entityBarManager { get; private set; }
        public ComponentBarManager componentBarManager { get; private set; }

        private readonly StringBuilder outputBuilder;
        private readonly WriteableBitmap writeableBitmap;
        private IntPtr pixelPtr;
        private bool stopPlay;

        private static readonly int maxOutputLength = 1024;
        private static readonly int renderWidth = 1280;
        private static readonly int renderHeight = 720;
        private static readonly Int32Rect dirtyRect = new Int32Rect(0, 0, renderWidth, renderHeight);
        private static readonly int stride = ((renderWidth * 32 + 31) & ~31) / 2;
        private static readonly uint byteLength = (uint)(renderWidth * renderHeight * 16);

        public MainWindow() {
            InitializeComponent();
            LoadProject();
            entityBarManager = new EntityBarManager(this, entityBar);
            componentBarManager = new ComponentBarManager(this, componentBar);
            outputBuilder = new StringBuilder(maxOutputLength);
            writeableBitmap = new WriteableBitmap(renderWidth, renderHeight, 96, 96, PixelFormats.Rgba128Float, null);
            renderImage.Source = writeableBitmap;
            pixelPtr = IntPtr.Zero;
            stopPlay = true;
        }

        public void NewPreview() {
            if (stopPlay) {
                framework.StartPreviewSystems();
                OpenGL.DrawFrame();
                UpdateImage();
            }
        }

        public void StartPlay() {
            stopPlay = false;
            System.Threading.Thread playThread = new System.Threading.Thread(new System.Threading.ThreadStart(Play));
            playThread.Start();
        }

        private void Play() {
            stopPlay = false;
            framework.StartPlaySystems();
            while (!stopPlay) {
                /*Dispatcher.Invoke(new Action(() => {
                    PrintOutput(Time.deltaTime.ToString());
                }));*/
                framework.UpdatePlaySystems();
                Dispatcher.Invoke(new Action(() => {
                    OpenGL.DrawFrame();
                    UpdateImage();
                    componentBarManager.RefreshValues();
                }));
            }
        }

        private void UpdateImage() {
            writeableBitmap.Lock();
            unsafe {
                float* backBuffer = (float*)writeableBitmap.BackBuffer;
                float* currPixel = (float*)pixelPtr;
                for (int i = 0; i < byteLength; i += 4) {
                    *backBuffer = *currPixel * *currPixel;
                    backBuffer++;
                    currPixel++;
                }
            }
            writeableBitmap.AddDirtyRect(dirtyRect);
            writeableBitmap.Unlock();
        }

        private void Window_Closed(object sender, EventArgs e) {
            stopPlay = true;
            OpenGL.CloseGLWindow();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Timer outputTimer = new Timer(50);
            outputTimer.Elapsed += OutputTimer_Elapsed;
            outputTimer.Start();

            OpenGL.SetSize(renderWidth, renderHeight);
            OpenGL.InitContext();
            pixelPtr = OpenGL.GetPixels();
            NewPreview();
        }

        private void PrintOutput(string output) {
            Label label = new Label() { Content = output };
            outputPanel.Children.Add(label);
        }

        private void OutputTimer_Elapsed(object sender, ElapsedEventArgs e) {
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
            StartPlay();
            playButton.IsEnabled = false;
            pauseButton.IsEnabled = true;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e) {
            stopPlay = true;
            playButton.IsEnabled = true;
            pauseButton.IsEnabled = false;
            NewPreview();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.ExtentHeightChange != 0) {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
        }

        private void LoadProject() {
            byte[] loadData = File.ReadAllBytes("save.txt");
            FastBinaryReader binaryReader = new FastBinaryReader(ref loadData);

            int componentCount = binaryReader.ReadInt();
            for (int i = 0; i < componentCount; i++) {
                string compTypeStr = binaryReader.ReadString();
                Type compType = Type.GetType(compTypeStr);
                Component component = (Component)compType.GetConstructor(new Type[0]).Invoke(null);

                int fieldCount = binaryReader.ReadInt();
                for (int j = 0; j < fieldCount; j++) {
                    string fieldName = binaryReader.ReadString();
                    FieldInfo field = compType.GetField(fieldName);
                    object fieldValue = TypeDeconstructor.Read(field.FieldType, binaryReader);
                    field.SetValue(component, fieldValue);
                }

                int entityCount = binaryReader.ReadInt();
                for (int j = 0; j < entityCount; j++) {
                    Entity entity = (Entity)binaryReader.ReadInt();
                    framework.LoadEntity(entity);
                    framework.AddComponent(component, entity);
                }
            }
        }

        private void Save() {
            using (FileStream fileStream = new FileStream("save.txt", FileMode.OpenOrCreate)) {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream)) {
                    ReadOnlyHashSet<Component> components = framework.GetComponents();
                    TypeDeconstructor.Print(components.Count, binaryWriter);
                    foreach (Component component in framework.GetComponents()) {
                        FieldInfo[] fields = component.GetType().GetFields().Where((field) => field.GetCustomAttribute(typeof(DontSaveAttribute)) == null).ToArray();
                        TypeDeconstructor.Print(component.GetType().AssemblyQualifiedName, binaryWriter);
                        TypeDeconstructor.Print(fields.Length, binaryWriter);
                        foreach (FieldInfo field in fields) {
                            if (field.GetCustomAttribute(typeof(DontSaveAttribute)) == null) {
                                TypeDeconstructor.Print(field.Name, binaryWriter);
                                TypeDeconstructor.Print(field.GetValue(component), binaryWriter);
                            }
                        }

                        ReadOnlyHashSet<Entity> entities = framework.GetEntitiesOfComponent(component);
                        TypeDeconstructor.Print(entities.Count, binaryWriter);
                        foreach (Entity entity in entities) {
                            TypeDeconstructor.Print(entity.id, binaryWriter);
                        }
                    }
                }
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            Save();
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void renderImage_MouseWheel(object sender, MouseWheelEventArgs e) {
            framework.previewCamera.entity.transform.position += new Vector3(0, 0, e.Delta / 150f);
            NewPreview();
        }
    }
}
