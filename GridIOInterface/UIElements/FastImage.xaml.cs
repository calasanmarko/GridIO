using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing.Imaging;

namespace GridIOInterface {
    /// <summary>
    /// Interaction logic for FastImage.xaml
    /// </summary>
    public partial class FastImage : UserControl {
        public FastWriteableBitmap bitmap;
        private Rect rect;

        [DllImport("Gdiplus.dll")]
        private static extern int GdipDrawImage(HandleRef graphics, HandleRef image, float x, float y);

        public FastImage() {
            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e) {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            if (bitmap != null) {
                BitmapSource source = BitmapSource.Create(bitmap.width, bitmap.height, 96, 96, PixelFormats.Bgr24, null, bitmap.pixels, bitmap.byteLength, bitmap.width * 3);
                drawingContext.DrawImage(source, rect);
            }
            base.OnRender(drawingContext);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
            rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
        }
    }
}
