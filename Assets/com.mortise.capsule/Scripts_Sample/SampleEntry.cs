using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SampleEntry : MonoBehaviour {

        [SerializeField] SampleRoleEntity role;
        [SerializeField] SamplePanel panel;
        [SerializeField] SampleRoleEditorModel em;

        SaveCore saveCore;
        ushort roleSaveKey;

        static readonly byte XOR_KEY = 0xAB;

        static byte[] XorEncrypt(byte[] data) {
            var result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++) result[i] = (byte)(data[i] ^ XOR_KEY);
            return result;
        }

        void RebuildSaveCore() {
            saveCore?.Clear();
            Func<byte[], byte[]> enc = panel.EnableEncrypt ? (Func<byte[], byte[]>)XorEncrypt : null;
            saveCore = new SaveCore(
                path: null,
                enableCrc: panel.EnableCrc,
                encryptFunc: enc,
                decryptFunc: enc
            );
            roleSaveKey = saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave");
        }

        async void Start() {
            panel.Ctor();
            panel.BtnSaveClickHandle = () => _ = SaveAsync();
            panel.BtnLoadClickHandle = () => _ = LoadAsync();
            panel.BtnRandomSkillClickHandle = () => RandomSkill();

            RebuildSaveCore();

            var (succ, iSave) = await saveCore.TryLoadAsync(roleSaveKey);
            panel.RefreshLoadButtonInteractable(succ);

            if (succ) {
                role.SetData((SampleRoleDBModel)iSave);
                panel.SetData(role);
            } else {
                role.SetData(em.roleDBModel);
                panel.SetData(role);
            }
        }

        async Task SaveAsync() {
            RebuildSaveCore();
            var model = panel.GetData();
            model.position = role.transform.position;
            model.rotation = role.transform.rotation;
            await saveCore.SaveAsync(model, roleSaveKey);
            em.Load();
        }

        async Task LoadAsync() {
            RebuildSaveCore();
            var (succ, iSave) = await saveCore.TryLoadAsync(roleSaveKey);
            if (succ) {
                var model = (SampleRoleDBModel)iSave;
                role.SetData(model);
                panel.SetData(role);
            }
            panel.RefreshLoadButtonInteractable(succ);
        }

        void RandomSkill() {
            var randomCount = UnityEngine.Random.Range(1, 5);
            var randomArr = new int[randomCount];
            for (int i = 0; i < randomCount; i++) {
                randomArr[i] = UnityEngine.Random.Range(1, 100);
            }
            var pos = role.transform.position;
            var rot = role.transform.rotation;
            var model = panel.GetData();
            model.skillTypeIDArr = randomArr;
            role.SetData(model);
            panel.SetData(role);
            role.transform.position = pos;
            role.transform.rotation = rot;
        }

        void Update() {
            var axis = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            role.transform.position += axis * Time.deltaTime * role.speed;
        }

    }

}
