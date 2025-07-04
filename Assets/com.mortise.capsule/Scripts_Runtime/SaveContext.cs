using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SaveContext {

        // Buffer
        byte[] readBuffer;
        byte[] writeBuffer;

        // Protocol
        BiDictionary<byte, (Type, int)> protocolDicts;
        Dictionary<byte, string> fileNameDict;

        // Service
        IDService idService;
        internal IDService IDService => idService;

        // Path
        string rootPath;
        internal string RootPath => GetRootPath();
        internal SaveContext(int bufferLength, string path) {
            readBuffer = new byte[bufferLength];
            writeBuffer = new byte[bufferLength];
            protocolDicts = new BiDictionary<byte, (Type, int)>();
            fileNameDict = new Dictionary<byte, string>();
            idService = new IDService();
            this.rootPath = path;
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
        internal byte RegisterSave(Type saveType, int index, string fileName) {
            if (!protocolDicts.ContainsValue((saveType, index))) {
                var saveId = IDService.PickSaveId();
                protocolDicts.Add(saveId, (saveType, index));
            }
            var key = GetKey(saveType, index);
            fileNameDict[key] = fileName;
            return key;
        }

        internal object GetSave(byte id) {
            var has = protocolDicts.TryGetByKey(id, out (Type type, int index) tuple);
            if (!has) {
                throw new ArgumentException("No type found for the given ID.", id.ToString());
            }
            if (tuple.type == null) {
                throw new ArgumentException("No type found for the given ID.", id.ToString());
            }
            return Activator.CreateInstance(tuple.type);
        }

        internal string GetSaveFileName(byte key) {
            var has = fileNameDict.TryGetValue(key, out string saveName);
            if (!has) {
                // throw new ArgumentException("No name found for the given ID.", key.ToString());
                Debug.LogError($"No name found for the given ID: {key}");
            }
            return saveName;
        }

        internal byte GetKey(Type saveType, int index) {
            var has = protocolDicts.TryGetByValue((saveType, index), out byte id);
            if (!has) {
                throw new ArgumentException("ID Not Found");
            }
            return id;
        }

        internal byte GetKey<T>(int index) {
            var has = protocolDicts.TryGetByValue((typeof(T), index), out byte id);
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

        internal void DeleteFile(byte key) {
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
        }

    }

}