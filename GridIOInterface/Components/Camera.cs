using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Input;

namespace GridIOInterface {
    public class Camera : Component {
        public override string typeName => "Camera";

        public Vector3 backgroundColor;

        public Camera() {
            this.backgroundColor = new Vector3(0.5f, 0, 0);
        }

        public Camera(Vector3 backgroundColor) {
            this.backgroundColor = backgroundColor;
        }

        public Camera(Color backgroundColor) {
            this.backgroundColor = new Vector3(backgroundColor.R / 255f, backgroundColor.G / 255f, backgroundColor.B / 255f);
        }

        public Vector2 ScreenToWorldPos(Vector2 screenPos) {
            IntPtr output = OpenGL.ScreenToWorldPos(screenPos.x, screenPos.y);
            unsafe {
                float* x = (float*)output;
                float* y = x + 1;
                return new Vector2(*x, *y);
            }
        }
    }
}
