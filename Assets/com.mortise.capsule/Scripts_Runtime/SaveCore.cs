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

        public byte Register(Type saveType, string fileName, int index = 0) {
            return ctx.RegisterSave(saveType, index, fileName);
        }

        public bool TryLoad(byte key, out ISave save) {
            byte[] buff = ctx.ReadBuffer_Get();
            ctx.ReadBuffer_Clear();

            var saveName = ctx.GetSaveFileName(key);
            var path = Path.Combine(ctx.RootPath, saveName);
            if (FileHelper.Exists(path)) {
                FileHelper.LoadBytes(path, buff);
            } else {
                save = null;
                return false;
            }

            var offset = 0;
            save = ctx.GetSave(key) as ISave;
            save.FromBytes(buff, ref offset);
            CLog.Log($"Load Succ: length = {offset}; key = {key}; path = {path}");

            return true;
        }

        public string Save(ISave save, byte key) {
            byte[] buff = ctx.WriteBuffer_Get();
            ctx.WriteBuffer_Clear();

            int offset = 0;
            save.WriteTo(buff, ref offset);

            var saveName = ctx.GetSaveFileName(key);
            var path = Path.Combine(ctx.RootPath, saveName);
            FileHelper.SaveBytes(path, buff, offset);

            CLog.Log($"Save Succ: length = {offset}; key = {key}; path = {path}");
            return path;
        }

        public void DeleteAllFiles() {
            ctx.DeleteAllFiles();
        }

        public void DeleteFile(byte key) {
            ctx.DeleteFile(key);
        }

        public void Clear() {
            ctx.Clear();
        }

    }

}