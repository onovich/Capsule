namespace MortiseFrame.Capsule {

    public interface ISave {

        void WriteTo(byte[] dst, ref int offset);
        void FromBytes(byte[] src, ref int offset);

    }

}