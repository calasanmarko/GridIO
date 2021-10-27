using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GridIOInterface {
    public static class OpenGL {
        [DllImport("GridIO.dll")]
        public static extern void InitContext();
        [DllImport("GridIO.dll")]
        public static extern void GetOutput(StringBuilder builder, int len);
        [DllImport("GridIO.dll")]
        public static extern void SetSize(int width, int height);
        [DllImport("GridIO.dll")]
        public static extern void SetGridSettings(float startX, float startY, float startZ, int width, int height);
        [DllImport("GridIO.dll")]
        public static extern void SetCameraPos(float camX, float camY, float camZ, float fov);
        [DllImport("GridIO.dll")]
        public static extern void SetValidCamera(bool valid);
        [DllImport("GridIO.dll")]
        public static extern void SetBackgroundColor(float r, float g, float b);
        [DllImport("GridIO.dll")]
        public static extern int LoadTexture([MarshalAs(UnmanagedType.LPWStr)] string path, bool alphaChannel);
        [DllImport("GridIO.dll")]
        public static extern int CreateDrawnEntity(int textureID);
        [DllImport("GridIO.dll")]
        public static extern void RemoveDrawnEntity(int drawnEntityID);
        [DllImport("GridIO.dll")]
        public static extern int SetDrawnEntityPosition(int drawnEntityID, float x, float y, float z);
        [DllImport("GridIO.dll")]
        public static extern int SetDrawnEntityScale(int drawnEntityID, float x, float y, float z);
        [DllImport("GridIO.dll")]
        public static extern void StartFrameLoop();
        [DllImport("GridIO.dll")]
        public static extern void DrawFrame();
        [DllImport("GridIO.dll")]
        public static extern IntPtr GetPixels();
        [DllImport("GridIO.dll")]
        public static extern double GetFPS();
        [DllImport("GridIO.dll")]
        public static extern void CloseGLWindow();
    }
}
