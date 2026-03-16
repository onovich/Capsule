using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public static class FileHelper {

        public static void SaveBytes(string path, byte[] data, int len) {
            using (var stream = File.Open(path, FileMode.Create)) {
                stream.Write(data, 0, len);
                stream.Flush();
            }
        }

        public static void LoadBytes(string path, byte[] buffer) {
            using (var stream = File.Open(path, FileMode.Open)) {
                stream.Read(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }

        public static bool Exists(string path) {
            return File.Exists(path);
        }

        public static async Task SaveBytesAsync(string path, byte[] data, int len,
            CancellationToken ct = default) {
            ct.ThrowIfCancellationRequested();
            using (var stream = new FileStream(path, FileMode.Create,
                FileAccess.Write, FileShare.None, 4096, useAsync: true)) {
                await stream.WriteAsync(data, 0, len, ct);
                await stream.FlushAsync(ct);
            }
        }

        public static async Task LoadBytesAsync(string path, byte[] buffer,
            CancellationToken ct = default) {
            ct.ThrowIfCancellationRequested();
            using (var stream = new FileStream(path, FileMode.Open,
                FileAccess.Read, FileShare.Read, 4096, useAsync: true)) {
                int totalRead = 0;
                int remaining = buffer.Length;
                while (remaining > 0) {
                    int read = await stream.ReadAsync(buffer, totalRead, remaining, ct);
                    if (read == 0) break;
                    totalRead += read;
                    remaining -= read;
                }
            }
        }

    }

}