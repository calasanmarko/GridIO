using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GridIOInterface {
    public class FastWriteableBitmap {
        public BitmapSource source => BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr24, null, pixels, byteLength, width * 3);
        public readonly IntPtr pixels;
        public readonly int width;
        public readonly int height;
        public int byteLength => width * height * 4;

        public FastWriteableBitmap(int width, int height) {
            this.width = width;
            this.height = height;
            this.pixels = Marshal.AllocHGlobal(byteLength);
        }
    }
}
