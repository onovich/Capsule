using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MortiseFrame.Capsule {

    public class SamplePanel : MonoBehaviour {

        [SerializeField] InputField input_name;
        [SerializeField] Dropdown dropdown_roleType;
        [SerializeField] InputField input_speed;
        [SerializeField] Toggle toggle_isMale;
        [SerializeField] Text text_skillTypeIDArr;
        [SerializeField] Button btn_randomSkill;

        [SerializeField] Button btn_save;
        [SerializeField] Button btn_load;

        public Action BtnSaveClickHandle;
        public Action BtnLoadClickHandle;
        public Action BtnRandomSkillClickHandle;

        public void Ctor() {
            btn_save.onClick.AddListener(() => {
                BtnSaveClickHandle?.Invoke();
            });
            btn_load.onClick.AddListener(() => {
                BtnLoadClickHandle?.Invoke();
            });
            btn_randomSkill.onClick.AddListener(() => {
                BtnRandomSkillClickHandle?.Invoke();
            });
            dropdown_roleType.ClearOptions();
            dropdown_roleType.AddOptions(new List<string> { "Normal", "Boss" });
        }

        public void RefreshLoadButtonInteractable(bool interactable) {
            btn_load.interactable = interactable;
        }

        public SampleRoleDBModel GetData() {
            SampleRoleDBModel model = new SampleRoleDBModel();
            model.typeName = input_name.text;
            model.roleType = (RoleType)dropdown_roleType.value;
            model.speed = float.Parse(input_speed.text);
            model.isMale = toggle_isMale.isOn;
            model.skillTypeIDArr = Array.ConvertAll(text_skillTypeIDArr.text.Split(','), int.Parse);
            return model;
        }

        public void SetData(SampleRoleEntity role) {
            input_name.text = role.typeName;
            dropdown_roleType.value = (int)role.roleType;
            input_speed.text = role.speed.ToString();
            toggle_isMale.isOn = role.isMale;
            text_skillTypeIDArr.text = string.Join(",", role.skillTypeIDArr);
        }

        private void OnDestroy() {
            btn_save.onClick.RemoveAllListeners();
            btn_load.onClick.RemoveAllListeners();
            btn_randomSkill.onClick.RemoveAllListeners();
            BtnSaveClickHandle = null;
            BtnLoadClickHandle = null;
            BtnRandomSkillClickHandle = null;
        }

    }

}