using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GridIOInterface {
    public static class ProjectSaver {
        public static ECSFramework framework => ECSFramework.instance;

        public static void Save(string filePath) {
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate)) {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream)) {
                    ReadOnlyHashSet<Component> components = framework.GetComponents();
                    TypeDeconstructor.Print(components.Count, binaryWriter);
                    foreach (Component component in framework.GetComponents()) {
                        FieldInfo[] fields = component.GetType().GetFields().Where((field) => field.GetCustomAttribute(typeof(DontSaveAttribute)) == null).ToArray();
                        TypeDeconstructor.Print(component.GetType().AssemblyQualifiedName, binaryWriter);
                        TypeDeconstructor.Print(fields.Length, binaryWriter);
                        foreach (FieldInfo field in fields) {
                            if (field.GetCustomAttribute(typeof(DontSaveAttribute)) == null) {
                                TypeDeconstructor.Print(field.Name, binaryWriter);
                                TypeDeconstructor.Print(field.GetValue(component), binaryWriter);
                            }
                        }

                        ReadOnlyHashSet<Entity> entities = framework.GetEntitiesOfComponent(component);
                        TypeDeconstructor.Print(entities.Count, binaryWriter);
                        foreach (Entity entity in entities) {
                            TypeDeconstructor.Print(entity.id, binaryWriter);
                        }
                    }
                }
            }
        }
    }
}
