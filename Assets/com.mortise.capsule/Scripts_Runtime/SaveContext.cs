using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SaveContext {

        // Buffer
        byte[] readBuffer;
        byte[] writeBuffer;

        // Protocol
        BiDictionary<ushort, (Type, int)> protocolDicts;
        Dictionary<ushort, string> fileNameDict;

        // Service
        IDService idService;
        internal IDService IDService => idService;

        // Lock
        Dictionary<ushort, SemaphoreSlim> keyLocks;

        // Path
        string rootPath;
        internal string RootPath => GetRootPath();
        internal int BufferLength => readBuffer.Length;

        // Version
        byte version;
        internal byte Version => version;

        internal SaveContext(int bufferLength, string path, byte version) {
            readBuffer = new byte[bufferLength];
            writeBuffer = new byte[bufferLength];
            protocolDicts = new BiDictionary<ushort, (Type, int)>();
            fileNameDict = new Dictionary<ushort, string>();
            idService = new IDService();
            keyLocks = new Dictionary<ushort, SemaphoreSlim>();
            this.rootPath = path;
            this.version = version;
        }

        // Path
        string GetRootPath() {
            if (rootPath == null) {
                return Application.persistentDataPath;
            } else {
                return rootPath;
            }
        }

        // Buffer
        internal void ReadBuffer_Clear() {
            Array.Clear(readBuffer, 0, readBuffer.Length);
        }

        internal void WriteBuffer_Clear() {
            Array.Clear(writeBuffer, 0, writeBuffer.Length);
        }

        internal byte[] ReadBuffer_Get() {
            return readBuffer;
        }

        internal byte[] WriteBuffer_Get() {
            return writeBuffer;
        }

        // Protocol
        internal ushort RegisterSave(Type saveType, int index, string fileName) {
            if (!protocolDicts.ContainsValue((saveType, index))) {
                var saveId = IDService.PickSaveId();
                protocolDicts.Add(saveId, (saveType, index));
            }
            var key = GetKey(saveType, index);
            fileNameDict[key] = fileName;
            if (!keyLocks.ContainsKey(key)) {
                keyLocks[key] = new SemaphoreSlim(1, 1);
            }
            return key;
        }

        internal SemaphoreSlim GetOrCreateLock(ushort key) {
            if (!keyLocks.TryGetValue(key, out var sem)) {
                sem = new SemaphoreSlim(1, 1);
                keyLocks[key] = sem;
            }
            return sem;
        }

        internal object GetSave(ushort id) {
            var has = protocolDicts.TryGetByKey(id, out (Type type, int index) tuple);
            if (!has) {
                throw new ArgumentException("No type found for the given ID.", id.ToString());
            }
            if (tuple.type == null) {
                throw new ArgumentException("No type found for the given ID.", id.ToString());
            }
            return Activator.CreateInstance(tuple.type);
        }

        internal string GetSaveFileName(ushort key) {
            var has = fileNameDict.TryGetValue(key, out string saveName);
            if (!has) {
                Debug.LogError($"No name found for the given ID: {key}");
            }
            return saveName;
        }

        internal ushort GetKey(Type saveType, int index) {
            var has = protocolDicts.TryGetByValue((saveType, index), out ushort id);
            if (!has) {
                throw new ArgumentException("ID Not Found");
            }
            return id;
        }

        internal ushort GetKey<T>(int index) {
            var has = protocolDicts.TryGetByValue((typeof(T), index), out ushort id);
            if (!has) {
                throw new ArgumentException("ID Not Found");
            }
            return id;
        }

        internal void DeleteAllFiles() {
            foreach (var kvp in fileNameDict) {
                var path = Path.Combine(GetRootPath(), kvp.Value);
                if (File.Exists(path)) {
                    File.Delete(path);
                }
            }
        }

        internal void DeleteFile(ushort key) {
            if (fileNameDict.TryGetValue(key, out string fileName)) {
                var path = Path.Combine(GetRootPath(), fileName);
                if (File.Exists(path)) {
                    File.Delete(path);
                }
            } else {
                throw new ArgumentException("No file found for the given ID.", key.ToString());
            }
        }

        internal void Clear() {
            Array.Clear(readBuffer, 0, readBuffer.Length);
            Array.Clear(writeBuffer, 0, writeBuffer.Length);
            protocolDicts.Clear();
            fileNameDict.Clear();
            idService.Reset();
            foreach (var kvp in keyLocks) {
                kvp.Value.Dispose();
            }
            keyLocks.Clear();
        }

    }

}
