using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GridIOInterface {
    public static class Time {
        public static float lastUpdate = time;
        public static float time => Stopwatch.GetTimestamp() / (float)Stopwatch.Frequency;
        public static float deltaTime => time - lastUpdate;
    }
}
