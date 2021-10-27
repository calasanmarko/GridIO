using System.Runtime.InteropServices;
using System.Windows;

namespace GridIOInterface {
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        public int x;
        public int y;

        public POINT(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static explicit operator Point(POINT p) {
            return new Point(p.x, p.y);
        }

        public static explicit operator POINT(Point p) {
            return new POINT((int)p.X, (int)p.Y);
        }
    }

}
