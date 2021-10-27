using System;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public abstract class Component {
        protected static ECSFramework framework = ECSFramework.instance;
        public abstract string typeName { get; }
        public ReadOnlyHashSet<Entity> entities {
            get {
                return framework.GetEntitiesOfComponent(this);
            }
        }
        public Entity entity {
            get {
                ReadOnlyHashSet<Entity> entities = this.entities;
                if (entities.Count == 1) {
                    return entities.First;
                }
                else {
                    throw new InvalidOperationException("There is more than one Entity attached to this Component.");
                }
            }
        }

        public virtual void OnGUIAdd() { }
        public virtual void OnDestroy() { }
    }
}
