using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public class EntityInfo : Component {

        public override string typeName => "Entity Information";
        public string name;

        public EntityInfo() {
            this.name = "New Entity";
        }

        public EntityInfo(string name) {
            this.name = name;
        }
    }
}
