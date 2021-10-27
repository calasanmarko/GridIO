using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

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
    }
}
