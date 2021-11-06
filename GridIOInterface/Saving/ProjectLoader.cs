using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GridIOInterface {
    public static class ProjectLoader {
        public static ECSFramework framework => ECSFramework.instance;

        public static void Open(string filePath) {
            byte[] loadData = File.ReadAllBytes(filePath);
            FastBinaryReader binaryReader = new FastBinaryReader(ref loadData);

            int componentCount = binaryReader.ReadInt();
            for (int i = 0; i < componentCount; i++) {
                string compTypeStr = binaryReader.ReadString();
                Type compType = Type.GetType(compTypeStr);
                Component component = (Component)compType.GetConstructor(new Type[0]).Invoke(null);

                int fieldCount = binaryReader.ReadInt();
                for (int j = 0; j < fieldCount; j++) {
                    string fieldName = binaryReader.ReadString();
                    FieldInfo field = compType.GetField(fieldName);
                    object fieldValue = TypeDeconstructor.Read(field.FieldType, binaryReader);
                    field.SetValue(component, fieldValue);
                }

                int entityCount = binaryReader.ReadInt();
                for (int j = 0; j < entityCount; j++) {
                    Entity entity = (Entity)binaryReader.ReadInt();
                    framework.LoadEntity(entity);
                    framework.AddComponent(component, entity);
                }
            }
        }
    }
}
