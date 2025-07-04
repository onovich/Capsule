using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SampleEntry : MonoBehaviour {

        [SerializeField] SampleRoleEntity role;
        [SerializeField] SamplePanel panel;
        [SerializeField] SampleRoleEditorModel em;
        SaveCore saveCore;
        byte roleSaveKey;

        void Start() {

            saveCore = new SaveCore();
            roleSaveKey = saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave");

            panel.Ctor();
            panel.BtnSaveClickHandle = () => Save();
            panel.BtnLoadClickHandle = () => Load();
            panel.BtnRandomSkillClickHandle = () => RandomSkill();
            var succ = saveCore.TryLoad(roleSaveKey, out ISave iSave);
            panel.RefreshLoadButtonInteractable(succ);

            if (succ) {
                role.SetData((SampleRoleDBModel)iSave);
                panel.SetData(role);
            } else {
                role.SetData(em.roleDBModel);
                panel.SetData(role);
            }

        }

        void Save() {
            var model = panel.GetData();
            model.position = role.transform.position;
            model.rotation = role.transform.rotation;
            saveCore.Save(model,roleSaveKey);
            em.Load();
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

        void RandomSkill() {
            var randomCount = Random.Range(1, 5);
            var randomArr = new int[randomCount];
            for (int i = 0; i < randomCount; i++) {
                randomArr[i] = Random.Range(1, 100);
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