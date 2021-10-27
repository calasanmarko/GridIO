using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GridIOInterface {
    public class RigidbodySystem : GameSystem {
        protected override bool isPreviewSystem => false;
        protected override bool isPlaySystem => true;

        protected override void Start() {

        }

        protected override void Update() {
            ReadOnlyHashSet<Component> rigidBodies = framework.GetComponentsOfType(typeof(Rigidbody));
            foreach (Component rigidBodyComp in rigidBodies) {
                Rigidbody rigidbody = (Rigidbody)rigidBodyComp;
                ReadOnlyHashSet<Entity> entities = rigidBodyComp.entities;
                foreach (Entity entity in entities) {
                    rigidbody.velocity += rigidbody.totalAcceleration * Time.deltaTime;
                    entity.transform.position += rigidbody.velocity * Time.deltaTime;
                }
            }
        }
    }
}
