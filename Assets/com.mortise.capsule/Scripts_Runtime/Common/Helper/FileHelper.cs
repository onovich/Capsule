using System;
using System.IO;
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

        public static void WriteFileToPersistent(string filename, byte[] data) {
            string path = Path.Combine(GetPersistentDir(), filename);
            SaveBytes(path, data, data.Length);
        }

        public static void CreateDirIfNotExist(string dir) {
            string path = Path.Combine(GetPersistentDir(), dir);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        public static byte[] ReadFileFromPersistent(string filename) {
            string path = Path.Combine(GetPersistentDir(), filename);
            if (File.Exists(path)) {
                return File.ReadAllBytes(path);
            } else {
                Debug.LogError($"File not found: {path}");
                return null;
            }
        }

        public static string GetPersistentDir() {
            return Application.persistentDataPath;
        }

    }

}