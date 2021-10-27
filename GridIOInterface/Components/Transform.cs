using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public class Transform : Component {
        public override string typeName => "Transform";
        public Vector3 position;
        public Vector3 scale;

        public Transform() {
            this.position = Vector3.zero;
            this.scale = Vector3.one;
        }

        public Transform(Vector3 position, Vector3 scale) {
            this.position = position;
            this.scale = scale;
        }
    }
}
