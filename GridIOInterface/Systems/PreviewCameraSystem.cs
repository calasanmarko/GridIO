using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    class PreviewCameraSystem : GameSystem {
        protected override bool isPreviewSystem => true;
        protected override bool isPlaySystem => false;

        protected override void Start() {
            OpenGL.SetValidCamera(true);
            Update();

            Vector3 pos = framework.previewCamera.entity.transform.position;
            OpenGL.SetCameraPos(pos.x, pos.y, pos.z, 45);

            Vector3 color = framework.previewCamera.backgroundColor;
            OpenGL.SetBackgroundColor(color.x, color.y, color.z);
        }
    }
}
