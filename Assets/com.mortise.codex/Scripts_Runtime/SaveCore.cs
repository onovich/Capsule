using System;
using System.IO;
using MortiseFrame.LitIO;

namespace MortiseFrame.Capsule {

    public class SaveCore {

        SaveContext ctx;

        public SaveCore(int bufferLength = 4069, string path = null) {
            this.ctx = new SaveContext(bufferLength, path);
        }

        public void Register(Type saveType, string fileName) {
            ctx.RegisterSave(saveType, fileName);
        }

        public ISave Load(byte key) {
            byte[] buff = ctx.ReadBuffer_Get();
            ctx.ReadBuffer_Clear();

            ISave save = null;

            var saveName = ctx.GetSaveFileName(key);
            var path = Path.Combine(ctx.Path, saveName);
            if (FileHelper.Exists(path)) {
                FileHelper.LoadBytes(path, buff);
            }

            var count = buff.Length;
            var offset = 0;
            while (offset < count) {
                var len = ByteReader.Read<int>(buff, ref offset);
                if (len == 0) {
                    break;
                }
                save = ctx.GetSave(key) as ISave;
                save.FromBytes(buff, ref offset);
            }

            CLog.Log($"Load from {path}, key: {key}");
            return save;
        }

        public byte Save(ISave save) {
            byte[] buff = ctx.WriteBuffer_Get();
            ctx.WriteBuffer_Clear();

            int offset = 4;
            byte key = ctx.GetKey(save.GetType());
            save.WriteTo(buff, ref offset);

            int len = offset;
            offset = 0;
            ByteWriter.Write<int>(buff, len, ref offset);

            if (len == 0) {
                return key;
            }

            var saveName = ctx.GetSaveFileName(key);
            var path = Path.Combine(ctx.Path, saveName);
            FileHelper.SaveBytes(path, buff, offset);

            CLog.Log($"Save to {path}, key: {key}");
            return key;
        }

    }

}