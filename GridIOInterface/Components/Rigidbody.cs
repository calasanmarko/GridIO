using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public class Rigidbody : Component {
        public override string typeName => "Rigidbody";

        public Vector3 velocity;
        public Vector3 acceleration;
        public float gravityScale;
        public Vector3 totalAcceleration => acceleration + Physics.gravity * gravityScale;

        public Rigidbody() {
            velocity = Vector3.zero;
            acceleration = Vector3.zero;
            gravityScale = 1;
        }
    }
}
