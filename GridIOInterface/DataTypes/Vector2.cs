using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;

namespace GridIOInterface {
    public class Vector2 {
        public readonly float x;
        public readonly float y;

        public static Vector2 zero;
        public static Vector2 one;

        public Vector2(float x, float y) {
            this.x = x;
            this.y = y;
        }

        static Vector2() {
            zero = new Vector2(0, 0);
            one = new Vector2(1, 1);
        }

        public static explicit operator Vector2(Point point) {
            return new Vector2((float)point.X, (float)point.Y);
        }

        public static explicit operator Vector2(Vector3 vec3) {
            return new Vector2(vec3.x, vec3.y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b) {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator *(Vector2 a, float b) {
            return new Vector2(a.x * b, a.y * b);
        }

        public static Vector2 operator /(Vector2 a, float b) {
            return new Vector2(a.x / b, a.y / b);
        }

        public override string ToString() {
            return "(" + x + ", " + y + ")";
        }
    }
}
