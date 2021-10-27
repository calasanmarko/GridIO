using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public class CameraSystem : GameSystem {
        protected override bool isPreviewSystem => false;
        protected override bool isPlaySystem => true;
        private bool validCamera = false;

        protected override void Start() {
            ReadOnlyHashSet<Component> cameras = framework.GetComponentsOfType(typeof(Camera));
            if (cameras == null || cameras.Count == 0) {
                validCamera = false;
            }
            else if (cameras.Count > 1) {
                validCamera = false;
            }
            else {
                validCamera = true;
                framework.mainCamera = (Camera)cameras.First;
            }
            OpenGL.SetValidCamera(validCamera);
        }

        protected override void Update() {
            if (framework.mainCamera != null) {
                Vector3 pos = framework.mainCamera.entity.transform.position;
                OpenGL.SetCameraPos(pos.x, pos.y, pos.z, 45);

                Vector3 color = framework.mainCamera.backgroundColor;
                OpenGL.SetBackgroundColor(color.x, color.y, color.z);
            }
            else if (validCamera) {
                validCamera = false;
                OpenGL.SetValidCamera(validCamera);
            }
        }
    }
}
