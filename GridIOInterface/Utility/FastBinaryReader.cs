using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class FastBinaryReader
{
    private readonly byte[] data;
    public int position { get; private set; }

    public FastBinaryReader(ref byte[] data) {
        this.data = data;
        this.position = 0;
    }

    public bool StreamOver() {
        return position >= data.Length;
    }

    public void BlockCopy<T>(Array destArray, int blockCount, int byteSize) {
        Buffer.BlockCopy(data, position, destArray, 0, blockCount * byteSize);
        position += blockCount * byteSize;
    }

    public T[] BlockCopy<T>(int blockCount, int byteSize) {
        T[] res = new T[blockCount];
        Buffer.BlockCopy(data, position, res, 0, blockCount * byteSize);
        position += blockCount * byteSize;
        return res;
    }

    public float ReadFloat() {
        float res = BitConverter.ToSingle(data, position);
        position += sizeof(float);
        return res;
    }

    public short ReadShort() {
        short res = BitConverter.ToInt16(data, position);
        position += sizeof(short);
        return res;
    }

    public int ReadInt() {
        int res = BitConverter.ToInt32(data, position);
        position += sizeof(int);
        return res;
    }

    public long ReadLong() {
        long res = BitConverter.ToInt64(data, position);
        position += sizeof(long);
        return res;
    }

    public ushort ReadUShort() {
        ushort res = BitConverter.ToUInt16(data, position);
        position += sizeof(ushort);
        return res;
    }

    public uint ReadUInt() {
        uint res = BitConverter.ToUInt32(data, position);
        position += sizeof(uint);
        return res;
    }

    public ulong ReadULong() {
        ulong res = BitConverter.ToUInt64(data, position);
        position += sizeof(ulong);
        return res;
    }

    public char ReadChar() {
        char res = BitConverter.ToChar(data, position);
        position += sizeof(char);
        return res;
    }

    public bool ReadBool() {
        bool res = BitConverter.ToBoolean(data, position);
        position += sizeof(bool);
        return res;
    }

    public double ReadDouble() {
        double res = BitConverter.ToDouble(data, position);
        position += sizeof(double);
        return res;
    }

    public sbyte ReadSByte() {
        return (sbyte)data[position++];
    }

    public byte ReadByte() {
        return data[position++];
    }

    public string ReadString() {
        int length = ReadInt();
        StringBuilder builder = new StringBuilder(length);
        for (int i = 0; i < length; i++) {
            builder.Append((char)ReadByte());
        }
        return builder.ToString();
    }
}
