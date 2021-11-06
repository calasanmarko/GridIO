using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GridIOInterface {
    public class BackgroundThread {
        private readonly Thread thread;
        private readonly Queue<Action> actions;
        private bool stopThread = false;

        public BackgroundThread() {
            actions = new Queue<Action>();
            thread = new Thread(new ThreadStart(() => {
                while (!stopThread) {
                    if (actions.Count > 0) {
                        actions.Dequeue()();
                    }
                }
            }));
        }

        public void Start() {
            thread.Start();
        }

        public void Stop() {
            stopThread = true;
        }

        public void Invoke(Action action) {
            actions.Enqueue(action);
        }
    }
}
