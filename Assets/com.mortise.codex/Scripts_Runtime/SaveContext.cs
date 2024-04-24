using System;
using System.Collections.Generic;

namespace MortiseFrame.Capsule {


    public class SaveContext {

        // Buffer
        byte[] readBuffer;
        byte[] writeBuffer;

        // Protocol
        BiDictionary<byte, Type> protocolDicts;
        Dictionary<byte, string> fileNameDict;

        // Service
        IDService idService;
        internal IDService IDService => idService;

        // Path
        internal string path;

        internal SaveContext(int bufferLength, string path) {
            readBuffer = new byte[bufferLength];
            writeBuffer = new byte[bufferLength];
            protocolDicts = new BiDictionary<byte, Type>();
            fileNameDict = new Dictionary<byte, string>();
            idService = new IDService();
            this.path = path;
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
        internal void RegisterSave(Type saveType, string fileName) {
            if (!protocolDicts.ContainsValue(saveType)) {
                var saveId = IDService.PickSaveId();
                protocolDicts.Add(saveId, saveType);
            }
            var id = GetSaveID(saveType);
            fileNameDict[id] = fileName;
        }

        internal object GetSave(byte id) {
            var has = protocolDicts.TryGetByKey(id, out Type type);
            if (!has) {
                throw new ArgumentException("No type found for the given ID.", id.ToString());
            }
            if (type == null) {
                throw new ArgumentException("No type found for the given ID.", id.ToString());
            }
            return Activator.CreateInstance(type);
        }

        internal string GetSaveFileName(byte saveID) {
            var has = fileNameDict.TryGetValue(saveID, out string saveName);
            if (!has) {
                throw new ArgumentException("No name found for the given ID.", saveID.ToString());
            }
            return saveName;
        }

        internal byte GetSaveID(Type saveType) {
            var has = protocolDicts.TryGetByValue(saveType, out byte id);
            if (!has) {
                throw new ArgumentException("ID Not Found");
            }
            return id;
        }

        internal byte GetSaveID<T>() {
            var has = protocolDicts.TryGetByValue(typeof(T), out byte id);
            if (!has) {
                throw new ArgumentException("ID Not Found");
            }
            return id;
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