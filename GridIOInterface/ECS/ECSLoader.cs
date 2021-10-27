using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public class ECSLoader {
        private static ECSFramework framework => ECSFramework.instance;

        public static void LoadAll() {
            LoadComponents();
            LoadSystems();
        }

        private static void LoadComponents() {
            framework.LoadComponentType(typeof(EntityInfo));
            framework.LoadComponentType(typeof(Camera));
            framework.LoadComponentType(typeof(Texture));
            framework.LoadComponentType(typeof(Transform));
            framework.LoadComponentType(typeof(Rigidbody));
        }

        private static void LoadSystems() {
            _ = new CameraSystem();
            _ = new PreviewCameraSystem();
            _ = new TextureRenderSystem();
            _ = new RigidbodySystem();
        }
    }
}
