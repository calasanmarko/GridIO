using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GridIOInterface {
    public class RenderManager {
        private static readonly ECSFramework framework = ECSFramework.instance;
        public bool previewStarted { get; private set; }
        public bool playStarted { get; private set; }
        private bool stopRenderThread = false;

        public readonly IntPtr currentFPS;

        private Thread renderThread;
        private Thread glThread;
        private IntPtr engineDoneFrame;
        private IntPtr glDoneFrame;
        private float lastRequestedPreviewTime = 0;

        private bool toLock = false;
        private bool toUnlock = false;

        private readonly MainWindow mainWindow;
        private readonly WriteableBitmap writeableBitmap;

        private static readonly int renderWidth = 1280;
        private static readonly int renderHeight = 720;
        private static readonly Int32Rect renderRect = new Int32Rect(0, 0, renderWidth, renderHeight);
        private static readonly float previewKeepAliveTime = 1;

        public RenderManager(MainWindow mainWindow, Image renderImage) {
            this.mainWindow = mainWindow;


            engineDoneFrame = Marshal.AllocHGlobal(sizeof(int));
            glDoneFrame = Marshal.AllocHGlobal(sizeof(int));
            currentFPS = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(engineDoneFrame, 0);
            Marshal.WriteInt32(glDoneFrame, 0);
            Marshal.WriteInt32(currentFPS, 0);

            previewStarted = false;
            playStarted = false;

            writeableBitmap = new WriteableBitmap(renderWidth, renderHeight, 96, 96, PixelFormats.Bgr24, null);
            renderImage.Source = writeableBitmap;

            OpenGL.SetSize(renderWidth, renderHeight);
            OpenGL.InitContext(engineDoneFrame, glDoneFrame, writeableBitmap.BackBuffer, currentFPS);
        }

        private void UpdateFrame() {
            toLock = true;
            mainWindow.Dispatcher.Invoke(() => writeableBitmap.Lock());
            toLock = false;

            OpenGL.ReleaseContext();
            Marshal.WriteInt32(engineDoneFrame, 1);
            while (Marshal.ReadInt32(glDoneFrame) == 0 && !stopRenderThread) { }
            Marshal.WriteInt32(glDoneFrame, 0);
            OpenGL.AttachContext();

            mainWindow.Dispatcher.Invoke(() => {
                writeableBitmap.AddDirtyRect(renderRect);
                writeableBitmap.Unlock();
            });
        }

        private void StartPreviewLoop() {
            if (!stopRenderThread && !previewStarted && !playStarted) {
                previewStarted = true;
                renderThread = new Thread(() => {
                    OpenGL.AttachContext();
                    while (!stopRenderThread && Time.time - lastRequestedPreviewTime < previewKeepAliveTime) {
                        framework.StartPreviewSystems();
                        UpdateFrame();
                        Thread.Sleep(1);
                    }
                    OpenGL.ReleaseContext();
                    SyncStopGLLoop();
                    stopRenderThread = false;
                    previewStarted = false;
                });
                StartGLLoop();
                renderThread.Start();
            }
            else {
                throw new InvalidOperationException("Another frame loop is already in progress.");
            }
        }

        public void StartPlayLoop() {
            mainWindow.componentBarManager.StartRefreshTimer();

            if (!stopRenderThread && !previewStarted && !playStarted) {
                playStarted = true;
                renderThread = new Thread(() => {
                    OpenGL.AttachContext();
                    framework.StartPlaySystems();

                    while (!stopRenderThread) {
                        framework.UpdatePlaySystems();
                        UpdateFrame();
                        Thread.Sleep(1);
                    }
                    OpenGL.ReleaseContext();
                    SyncStopGLLoop();
                    stopRenderThread = false;
                    playStarted = false;
                });
                StartGLLoop();
                renderThread.Start();
            }
            else {
                throw new InvalidOperationException("Another frame loop is already in progress.");
            }
        }

        public void StopFrameLoop() {
            mainWindow.componentBarManager.StopRefreshTimer();
            mainWindow.StopFPSUpdateTimer();

            if (previewStarted || playStarted) {
                stopRenderThread = true;
            }
        }

        public void SyncStopFrameLoop() {
            StopFrameLoop();
            while (previewStarted || playStarted) {
                Dispatcher.Run();
            }
        }

        private void SyncStopGLLoop() {
            OpenGL.StopFrameLoop();
            while (glThread.IsAlive) { }
        }

        private void StartGLLoop() {
            if (glThread == null || !glThread.IsAlive) {
                mainWindow.StartFPSUpdateTimer();
                glThread = new Thread(() => {
                    OpenGL.StartFrameLoop();
                    OpenGL.ReleaseContext();
                    mainWindow.Dispatcher.BeginInvoke(() => OpenGL.AttachContext());
                    mainWindow.StopFPSUpdateTimer();
                });

                OpenGL.ReleaseContext();
                glThread.Start();
            }
            else {
                throw new InvalidOperationException("Another GL thread is already running.");
            }
        }

        public void RequestPreview() {
            if (!playStarted && !stopRenderThread) {
                lastRequestedPreviewTime = Time.time;
                if (!previewStarted) {
                    StartPreviewLoop();
                }
            }
        }
    }
}
