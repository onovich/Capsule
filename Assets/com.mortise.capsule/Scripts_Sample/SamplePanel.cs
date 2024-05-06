using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MortiseFrame.Capsule {

    public class SamplePanel : MonoBehaviour {

        [SerializeField] InputField input_name;
        [SerializeField] Dropdown dropdown_roleType;
        [SerializeField] InputField input_atk;
        [SerializeField] Toggle toggle_canFly;
        [SerializeField] Text text_skillTypeIDArr;
        [SerializeField] Button btn_randomSkill;

        [SerializeField] Button btn_save;
        [SerializeField] Button btn_load;

        public Action BtnSaveClickHandle;
        public Action BtnLoadClickHandle;

        public void Ctor() {
            btn_save.onClick.AddListener(() => {
                BtnSaveClickHandle?.Invoke();
            });
            btn_load.onClick.AddListener(() => {
                BtnLoadClickHandle?.Invoke();
            });
        }

        public void RefreshLoadButtonInteractable(bool interactable) {
            btn_load.interactable = interactable;
        }

        public SampleRoleDBModel GetData() {
            SampleRoleDBModel model = new SampleRoleDBModel();
            model.typeName = input_name.text;
            model.roleType = (RoleType)dropdown_roleType.value;
            model.atk = float.Parse(input_atk.text);
            model.canFly = toggle_canFly.isOn;
            model.skillTypeIDArr = Array.ConvertAll(text_skillTypeIDArr.text.Split(','), int.Parse);
            return model;
        }

        public void SetData(SampleRoleEntity role) {
            input_name.text = role.typeName;
            dropdown_roleType.value = (int)role.roleType;
            input_atk.text = role.atk.ToString();
            toggle_canFly.isOn = role.canFly;
            text_skillTypeIDArr.text = string.Join(",", role.skillTypeIDArr);
        }

    }

}