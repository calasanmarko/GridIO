using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    class TextureRenderSystem : GameSystem {
        protected override bool isPreviewSystem => true;
        protected override bool isPlaySystem => true;

        protected override void Start() {
            foreach (Component textureComp in framework.GetComponentsOfType(typeof(Texture))) {
                Texture texture = (Texture)textureComp;
                if (!texture.isLoaded) {
                    texture.textureID = OpenGL.LoadTexture(texture.path, texture.path.EndsWith(".png"));
                    texture.drawnEntityID = OpenGL.CreateDrawnEntity(texture.textureID);
                }
            }
            Update();
        }

        protected override void Update() {
            foreach (Component textureComp in framework.GetComponentsOfType(typeof(Texture))) {
                Texture texture = (Texture)textureComp;
                Vector3 position = texture.entity.transform.position;
                Vector3 scale = texture.entity.transform.scale;
                OpenGL.SetDrawnEntityPosition(texture.drawnEntityID, position.x, position.y, position.z);
                OpenGL.SetDrawnEntityScale(texture.drawnEntityID, scale.x, scale.y, scale.z);
            }
        }
    }
}
