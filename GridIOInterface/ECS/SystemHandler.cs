using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public class SystemHandler {
        public delegate void Start();
        public delegate void Update();

        public readonly Start start;
        public readonly Update update;

        public SystemHandler(Start start, Update update) {
            this.start = start;
            this.update = update;
        }
    }
}
