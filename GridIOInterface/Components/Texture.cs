using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    class Texture : Component {
        public override string typeName => "Texture";

        public string path;
        [DontSave]
        public int drawnEntityID;
        [DontSave]
        public int textureID;

        public bool isLoaded => drawnEntityID != -1 && textureID != -1;

        public Texture() {
            this.path = "";
            this.drawnEntityID = -1;
            this.textureID = -1;
        }

        public Texture(string path) {
            this.path = path;
            this.drawnEntityID = -1;
            this.textureID = -1;
        }

        public override void OnGUIAdd() {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            path = openFileDialog.FileName;
        }

        public override void OnDestroy() {
            OpenGL.RemoveDrawnEntity(drawnEntityID);
        }
    }
}
