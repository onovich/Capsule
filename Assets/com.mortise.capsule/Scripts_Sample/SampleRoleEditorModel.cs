using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SampleRoleEditorModel : ScriptableObject {

        public SampleRoleDBModel roleDBModel;
        SaveCore saveCore;
        [SerializeField] ushort key;

        [ContextMenu("Bake")]
        public void Bake() {
            CLog.LogHandler = Debug.Log;
            if (saveCore == null) {
                saveCore = new SaveCore();
                key = saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave");
            }
            saveCore.Save(roleDBModel, key);
        }

        [ContextMenu("Load")]
        public void Load() {
            CLog.LogHandler = Debug.Log;
            if (saveCore == null) {
                saveCore = new SaveCore();
                saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave");
            }
            var succ = saveCore.TryLoad(key, out ISave iSave);
            if (succ) {
                roleDBModel = (SampleRoleDBModel)iSave;
            }
        }

    }

}
