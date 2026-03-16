using System;
using MortiseFrame.Capsule;

namespace MortiseFrame.Capsule.Tests {

    public class MockSave : ISave {

        public int IntValue;
        public float FloatValue;

        public void WriteTo(byte[] dst, ref int offset) {
            var intBytes = BitConverter.GetBytes(IntValue);
            Buffer.BlockCopy(intBytes, 0, dst, offset, 4);
            offset += 4;
            var floatBytes = BitConverter.GetBytes(FloatValue);
            Buffer.BlockCopy(floatBytes, 0, dst, offset, 4);
            offset += 4;
        }

        public void FromBytes(byte[] src, ref int offset) {
            IntValue = BitConverter.ToInt32(src, offset);
            offset += 4;
            FloatValue = BitConverter.ToSingle(src, offset);
            offset += 4;
        }

    }

    public class OversizeMockSave : ISave {

        public int Size;

        public OversizeMockSave(int size) {
            Size = size;
        }

        public void WriteTo(byte[] dst, ref int offset) {
            offset += Size;
        }

        public void FromBytes(byte[] src, ref int offset) {
            offset += Size;
        }

    }

}
