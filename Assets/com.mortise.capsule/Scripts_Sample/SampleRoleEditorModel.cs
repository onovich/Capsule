using UnityEngine;

namespace MortiseFrame.Capsule {

    public class SampleRoleEditorModel : ScriptableObject {

        public SampleRoleDBModel roleDBModel;
        SaveCore saveCore;
        [SerializeField] byte key;

        [ContextMenu("Bake")]
        public void Bake() {
            CLog.Log = Debug.Log;
            if (saveCore == null) {
                saveCore = new SaveCore();
                key = saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave");
            }
            saveCore.Save(roleDBModel);
        }

        [ContextMenu("Load")]
        public void Load() {
            CLog.Log = Debug.Log;
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