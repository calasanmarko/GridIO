using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Drawing;

namespace GridIOInterface {
    public class ECSFramework {
        public static readonly ECSFramework instance;

        private readonly HashSet<Entity> entities;
        private readonly HashSet<Component> components;
        private readonly List<Type> componentTypes;

        private readonly HashSet<Type> nonAddableComponents;
        private readonly Dictionary<Type, HashSet<Component>> componentsOfType;
        private readonly Dictionary<Component, HashSet<Entity>> entitiesOfComponent;
        private readonly Dictionary<Entity, HashSet<Component>> componentsOfEntity;

        private readonly List<SystemHandler> previewSystems;
        private readonly List<SystemHandler> playSystems;

        private readonly Dictionary<string, object> globals;

        public int nextEntityID { get; private set; }
        public int nextComponentID { get; private set; }

        public Camera mainCamera;
        public Camera previewCamera;
        private Entity previewCameraEntity;

        private ECSFramework() {
            entities = new HashSet<Entity>();
            components = new HashSet<Component>();
            componentTypes = new List<Type>();
            nonAddableComponents = new HashSet<Type>() {
                typeof(EntityInfo),
                typeof(Transform)
            };
            componentsOfType = new Dictionary<Type, HashSet<Component>>();
            entitiesOfComponent = new Dictionary<Component, HashSet<Entity>>();
            componentsOfEntity = new Dictionary<Entity, HashSet<Component>>();
            previewSystems = new List<SystemHandler>();
            playSystems = new List<SystemHandler>();
            globals = new Dictionary<string, object>();
            nextEntityID = 0;
            nextComponentID = 0;
        }

        static ECSFramework() {
            instance = new ECSFramework();
            ECSLoader.LoadAll();
            instance.LoadPreviewCamera();
        }

        public void LoadPreviewCamera() {
            previewCameraEntity = CreateEntity("Preview Camera", true);
            previewCameraEntity.transform.position = new Vector3(0, 0, -10);
            previewCamera = new Camera(Color.SkyBlue);
            AddComponent(previewCamera, previewCameraEntity, true);
        }

        public void LoadEntity(Entity entity) {
            if (!entities.Contains(entity)) {
                entities.Add(entity);
                if (nextEntityID <= entity.id) {
                    nextEntityID = entity.id + 1;
                }
            }
        }

        public Entity CreateEntity(string name) {
            return CreateEntity(name, false);
        }

        private Entity CreateEntity(string name, bool hidden) {
            Entity entity = (Entity)nextEntityID;
            AddComponent(new EntityInfo(name), entity, hidden);
            AddComponent(new Transform(), entity, hidden);
            if (!hidden) {
                entities.Add(entity);
            }
            nextEntityID++;
            return entity;
        }

        public void RemoveEntity(Entity entity) {
            entities.Remove(entity);
            foreach (Component component in componentsOfEntity[entity]) {
                entitiesOfComponent[component].Remove(entity);
            }
            componentsOfEntity.Remove(entity);
        }

        public void LoadComponentType(Type componentType) {
            componentTypes.Add(componentType);
        }

        public void AddComponent(Component component, Entity entity) {
            AddComponent(component, entity, false);
        }

        private void AddComponent(Component component, Entity entity, bool hidden) {
            if (!hidden) {
                components.Add(component);
                if (componentsOfType.TryGetValue(component.GetType(), out HashSet<Component> typeComponents)) {
                    typeComponents.Add(component);
                }
                else {
                    componentsOfType.Add(component.GetType(), new HashSet<Component>() { component });
                }
            }

            if (entitiesOfComponent.TryGetValue(component, out HashSet<Entity> componentEntities)) {
                componentEntities.Add(entity);
            }
            else {
                entitiesOfComponent.Add(component, new HashSet<Entity>() { entity });
            }

            if (componentsOfEntity.TryGetValue(entity, out HashSet<Component> entityComponents)) {
                entityComponents.Add(component);
            }
            else {
                componentsOfEntity.Add(entity, new HashSet<Component>() { component });
            }
            nextComponentID++;
        }

        public void RemoveComponent(Component component, Entity entity) {
            component.OnDestroy();

            entitiesOfComponent.TryGetValue(component, out HashSet<Entity> currEntities);
            currEntities.Remove(entity);
            if (currEntities.Count == 0) {
                entitiesOfComponent.Remove(component);

                componentsOfType.TryGetValue(component.GetType(), out HashSet<Component> currComponents);
                currComponents.Remove(component);
                if (currComponents.Count == 0) {
                    componentsOfType.Remove(component.GetType());
                }
            }
            componentsOfEntity.TryGetValue(entity, out HashSet<Component> componentsOfCurrEntity);
            componentsOfCurrEntity.Remove(component);
        }

        public ReadOnlyHashSet<Entity> GetEntities() {
            return (ReadOnlyHashSet<Entity>)entities;
        }

        public ReadOnlyHashSet<Component> GetComponents() {
            return (ReadOnlyHashSet<Component>)components;
        }

        public T GetComponentOfEntity<T>(Entity entity) where T : Component{
            foreach (Component component in componentsOfEntity[entity]) {
                if (component.GetType() == typeof(T)) {
                    return (T)component;
                }
            }
            return null;
        }

        public ReadOnlyHashSet<Component> GetComponentsOfEntity(Entity entity) {
            if (componentsOfEntity.TryGetValue(entity, out HashSet<Component> res)) {
                return (ReadOnlyHashSet<Component>)res;
            }
            else {
                return new ReadOnlyHashSet<Component>();
            }
        }

        public ReadOnlyHashSet<Component> GetComponentsOfType(Type type) {
            if (componentsOfType.TryGetValue(type, out HashSet<Component> res)) {
                return (ReadOnlyHashSet<Component>)res;
            }
            else {
                return new ReadOnlyHashSet<Component>();
            }
        }

        public ReadOnlyHashSet<Entity> GetEntitiesOfComponent(Component component) {
            if (entitiesOfComponent.TryGetValue(component, out HashSet<Entity> res)) {
                return (ReadOnlyHashSet<Entity>)res;
            }
            else {
                return new ReadOnlyHashSet<Entity>();
            }
        }

        public ReadOnlyCollection<Type> GetComponentTypes() {
            return componentTypes.AsReadOnly();
        }

        public bool IsComponentAddable(Type componentType) {
            return !nonAddableComponents.Contains(componentType);
        }

        public void AddPreviewSystem(SystemHandler system) {
            previewSystems.Add(system);
        }

        public void AddPlaySystem(SystemHandler system) {
            playSystems.Add(system);
        }

        public void StartPreviewSystems() {
            foreach (SystemHandler system in previewSystems) {
                system.start();
            }
        }

        public void StartPlaySystems() {
            foreach (SystemHandler system in playSystems) {
                system.start();
            }
            Time.lastUpdate = Time.time;
        }

        public void UpdatePlaySystems() {
            foreach (SystemHandler system in playSystems) {
                system.update();
            }
            Time.lastUpdate = Time.time;
        }

        public void SetGlobal(string name, object val) {
            globals.Add(name, val);
        }

        public T GetGlobal<T>(string name) {
            return (T)globals[name];
        }
    }
}