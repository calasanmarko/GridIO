using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public class Vector3 {
        public readonly float x;
        public readonly float y;
        public readonly float z;

        public static Vector3 zero;
        public static Vector3 one;

        public Vector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Vector2 xy, float z) {
            this.x = xy.x;
            this.y = xy.y;
            this.z = z;
        }

        static Vector3() {
            zero = new Vector3(0, 0, 0);
            one = new Vector3(1, 1, 1);
        }

        public static explicit operator Vector3(Vector2 vec2) {
            return new Vector3(vec2.x, vec2.y, 0);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator +(Vector3 a, Vector2 b) {
            return new Vector3(a.x + b.x, a.y + b.y, a.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b) {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector2 b) {
            return new Vector3(a.x - b.x, a.y - b.y, a.z);
        }

        public static Vector3 operator *(Vector3 a, float b) {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator /(Vector3 a, float b) {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        public override string ToString() {
            return "(" + x + ", " + y + ", " + z + ")";
        }
    }
}
