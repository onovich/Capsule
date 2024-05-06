using System;
using System.IO;
using MortiseFrame.LitIO;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SaveCore {

        SaveContext ctx;

        public SaveCore(int bufferLength = 4069, string path = null) {
            this.ctx = new SaveContext(bufferLength, path);
        }

        public byte Register(Type saveType, string fileName) {
            return ctx.RegisterSave(saveType, fileName);
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

            var offset = 0;
            save = ctx.GetSave(key) as ISave;
            save.FromBytes(buff, ref offset);

            return save;
        }

        public void Save(ISave save) {
            byte[] buff = ctx.WriteBuffer_Get();
            ctx.WriteBuffer_Clear();

            int offset = 0;
            byte key = ctx.GetKey(save.GetType());
            save.WriteTo(buff, ref offset);

            var saveName = ctx.GetSaveFileName(key);
            var path = Path.Combine(ctx.Path, saveName);
            FileHelper.SaveBytes(path, buff, offset);
        }

    }

}