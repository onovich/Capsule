using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SampleRoleEntity : MonoBehaviour {

        public int typeID;
        public string typeName;
        public RoleType roleType;
        public float speed;
        public bool isMale;
        public int[] skillTypeIDArr;

        public void SetData(SampleRoleDBModel model) {
            typeID = model.typeID;
            typeName = model.typeName;
            roleType = model.roleType;
            speed = model.speed;
            transform.position = model.position;
            transform.rotation = model.rotation;
            isMale = model.isMale;
            skillTypeIDArr = model.skillTypeIDArr;
        }

        public SampleRoleDBModel GetData() {
            SampleRoleDBModel model = new SampleRoleDBModel();
            model.typeID = typeID;
            model.typeName = typeName;
            model.roleType = roleType;
            model.speed = speed;
            model.position = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
            model.rotation = transform.rotation;
            model.isMale = isMale;
            model.skillTypeIDArr = skillTypeIDArr;
            return model;
        }

    }

}