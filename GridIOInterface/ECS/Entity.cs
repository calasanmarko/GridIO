using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public readonly struct Entity {
        private static ECSFramework framework => ECSFramework.instance;

        public readonly int id;

        private Entity(int id) {
            this.id = id;
        }

        public override bool Equals(object obj) => obj is Entity other && id == other.id;

        public override int GetHashCode() {
            return id;
        }

        public static explicit operator Entity(int id) {
            return new Entity(id);
        }

        public T GetComponent<T>() where T : Component {
            return framework.GetComponentOfEntity<T>(this);
        }

        public EntityInfo entityInfo {
            get {
                return GetComponent<EntityInfo>();
            }
        }

        public Transform transform {
            get {
                return GetComponent<Transform>();
            }
        }
    }
}
