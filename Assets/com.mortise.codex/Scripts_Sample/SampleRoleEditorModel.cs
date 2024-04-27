using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SampleRoleEditorModel : ScriptableObject {

        public SampleRoleDBModel roleDBModel;
        SaveCore saveCore;
        byte key;

        [ContextMenu("Bake")]
        public void Bake() {
            CLog.Log = Debug.Log;
            if (saveCore == null) {
                saveCore = new SaveCore();
                saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave");
            }
            key = saveCore.Save(roleDBModel);
        }

        [ContextMenu("Load")]
        public void Load() {
            CLog.Log = Debug.Log;
            if (saveCore == null) {
                saveCore = new SaveCore();
                saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave");
            }
            roleDBModel = (SampleRoleDBModel)saveCore.Load(key);
        }

    }

}