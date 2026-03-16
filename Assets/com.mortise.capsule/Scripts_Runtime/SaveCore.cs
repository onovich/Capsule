using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MortiseFrame.LitIO;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SaveCore {

        SaveContext ctx;

        public SaveCore(int bufferLength = 4069, string path = null, byte version = 1) {
            this.ctx = new SaveContext(bufferLength, path, version);
        }

        public ushort Register(Type saveType, string fileName, int index = 0) {
            return ctx.RegisterSave(saveType, index, fileName);
        }

        public bool TryLoad(ushort key, out ISave save) {
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

            if (buff[0] != ctx.Version) {
                CLog.LogWarning($"版本不匹配：文件版本 {buff[0]}，当前版本 {ctx.Version}；key = {key}");
                save = null;
                return false;
            }

            var offset = 1;
            save = ctx.GetSave(key) as ISave;
            save.FromBytes(buff, ref offset);
            CLog.Log($"Load Succ: length = {offset}; key = {key}; path = {path}");

            return true;
        }

        public string Save(ISave save, ushort key) {
            byte[] buff = ctx.WriteBuffer_Get();
            ctx.WriteBuffer_Clear();

            buff[0] = ctx.Version;
            int offset = 1;
            save.WriteTo(buff, ref offset);

            if (offset > buff.Length) {
                throw new InvalidOperationException(
                    $"存档数据超出缓冲区大小！需要 {offset} 字节，但缓冲区只有 {buff.Length} 字节。" +
                    $"请在 SaveCore 构造时增大 bufferLength 参数。"
                );
            }

            var saveName = ctx.GetSaveFileName(key);
            var path = Path.Combine(ctx.RootPath, saveName);
            FileHelper.SaveBytes(path, buff, offset);

            CLog.Log($"Save Succ: length = {offset}; key = {key}; path = {path}");
            return path;
        }

        public void DeleteAllFiles() {
            ctx.DeleteAllFiles();
        }

        public void DeleteFile(ushort key) {
            ctx.DeleteFile(key);
        }

        public void Clear() {
            ctx.Clear();
        }

        public async Task<string> SaveAsync(ISave save, ushort key,
            CancellationToken ct = default) {
            var sem = ctx.GetOrCreateLock(key);
            await sem.WaitAsync(ct);
            try {
                byte[] buff = new byte[ctx.BufferLength];
                buff[0] = ctx.Version;
                int offset = 1;
                save.WriteTo(buff, ref offset);

                if (offset > buff.Length) {
                    throw new InvalidOperationException(
                        $"存档数据超出缓冲区大小！需要 {offset} 字节，但缓冲区只有 {buff.Length} 字节。" +
                        $"请在 SaveCore 构造时增大 bufferLength 参数。"
                    );
                }

                var saveName = ctx.GetSaveFileName(key);
                var path = Path.Combine(ctx.RootPath, saveName);
                await FileHelper.SaveBytesAsync(path, buff, offset, ct);

                CLog.Log($"SaveAsync Succ: length = {offset}; key = {key}; path = {path}");
                return path;
            } finally {
                sem.Release();
            }
        }

        public async Task<(bool success, ISave save)> TryLoadAsync(ushort key,
            CancellationToken ct = default) {
            var sem = ctx.GetOrCreateLock(key);
            await sem.WaitAsync(ct);
            try {
                var saveName = ctx.GetSaveFileName(key);
                var path = Path.Combine(ctx.RootPath, saveName);

                if (!FileHelper.Exists(path)) {
                    return (false, null);
                }

                byte[] buff = new byte[ctx.BufferLength];
                await FileHelper.LoadBytesAsync(path, buff, ct);

                if (buff[0] != ctx.Version) {
                    CLog.LogWarning($"版本不匹配：文件版本 {buff[0]}，当前版本 {ctx.Version}；key = {key}");
                    return (false, null);
                }

                int offset = 1;
                var save = ctx.GetSave(key) as ISave;
                save.FromBytes(buff, ref offset);

                CLog.Log($"TryLoadAsync Succ: length = {offset}; key = {key}; path = {path}");
                return (true, save);
            } finally {
                sem.Release();
            }
        }

    }

}
