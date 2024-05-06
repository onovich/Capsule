using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SampleEntry : MonoBehaviour {

        [SerializeField] SampleRoleEntity role;
        [SerializeField] SamplePanel panel;
        SaveCore saveCore;
        byte roleSaveKey;

        void Start() {

            saveCore = new SaveCore();
            var roleSaveKey = saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave");

            panel.Ctor();
            panel.BtnSaveClickHandle = () => Save();
            panel.BtnLoadClickHandle = () => Load();
            var succ = saveCore.TryLoad(roleSaveKey, out ISave iSave);
            panel.RefreshLoadButtonInteractable(succ);

        }

        void Save() {
            var model = panel.GetData();
            model.position = role.transform.position;
            model.rotation = role.transform.rotation;
            saveCore.Save(model);
        }

        void Load() {
            var succ = saveCore.TryLoad(roleSaveKey, out ISave iSave);
            if (succ) {
                var model = (SampleRoleDBModel)iSave;
                role.SetData(model);
                panel.SetData(role);
            }
            panel.RefreshLoadButtonInteractable(succ);
        }

    }

}