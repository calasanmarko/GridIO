using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public abstract class GameSystem {
        protected static ECSFramework framework => ECSFramework.instance;
        protected abstract bool isPreviewSystem { get; }
        protected abstract bool isPlaySystem { get; }

        protected GameSystem() {
            SystemHandler systemHandler = new SystemHandler(Start, Update);
            if (isPreviewSystem) {
                framework.AddPreviewSystem(systemHandler);
            }
            if (isPlaySystem) {
                framework.AddPlaySystem(systemHandler);
            }
        }

        protected virtual void Initialize() { }
        protected virtual void Start() { }
        protected virtual void Update() { }
    }
}
