using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GridIOInterface {
    public static class TypeDeconstructor {
        public static void Print(object value, BinaryWriter writer) {
            if (value is ulong ulongVal) {
                writer.Write(ulongVal);
            }
            else if (value is uint uintVal) {
                writer.Write(uintVal);
            }
            else if (value is ushort ushortVal) {
                writer.Write(ushortVal);
            }
            else if (value is string stringVal) {
                char[] charArray = stringVal.ToCharArray();
                writer.Write(charArray.Length);
                writer.Write(charArray);
            }
            else if (value is float floatVal) {
                writer.Write(floatVal);
            }
            else if (value is sbyte sbyteVal) {
                writer.Write(sbyteVal);
            }
            else if (value is long longVal) {
                writer.Write(longVal);
            }
            else if (value is int intVal) {
                writer.Write(intVal);
            }
            else if (value is double doubleVal) {
                writer.Write(doubleVal);
            }
            else if (value is char charVal) {
                writer.Write(charVal);
            }
            else if (value is byte byteVal) {
                writer.Write(byteVal);
            }
            else if (value is Vector2 vec2Val) {
                writer.Write(vec2Val.x);
                writer.Write(vec2Val.y);
            }
            else if (value is Vector3 vec3Val) {
                writer.Write(vec3Val.x);
                writer.Write(vec3Val.y);
                writer.Write(vec3Val.z);
            }
            else {
                throw new InvalidOperationException("Unable to print object of type " + value.GetType().Name);
            }
        }

        public static object Read(Type type, FastBinaryReader reader) {
            if (type == typeof(ulong)) {
                return reader.ReadULong();
            }
            else if (type == typeof(uint)) {
                return reader.ReadUInt();
            }
            else if (type == typeof(ushort)) {
                return reader.ReadUShort();
            }
            else if (type == typeof(string)) {
                return reader.ReadString();
            }
            else if (type == typeof(float)) {
                return reader.ReadFloat();
            }
            else if (type == typeof(sbyte)) {
                return reader.ReadSByte();
            }
            else if (type == typeof(long)) {
                return reader.ReadLong();
            }
            else if (type == typeof(int)) {
                return reader.ReadInt();
            }
            else if (type == typeof(double)) {
                return reader.ReadDouble();
            }
            else if (type == typeof(char)) {
                return reader.ReadChar();
            }
            else if (type == typeof(byte)) {
                return reader.ReadByte();
            }
            else if (type == typeof(Vector2)) {
                return new Vector2(reader.ReadFloat(), reader.ReadFloat());
            }
            else if (type == typeof(Vector3)) {
                return new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
            }
            else {
                throw new InvalidOperationException("Unable to read object of type " + type.Name);
            }
        }
    }
}
