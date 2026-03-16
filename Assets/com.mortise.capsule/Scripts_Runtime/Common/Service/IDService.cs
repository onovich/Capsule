namespace MortiseFrame.Capsule {

    public class IDService {

        public ushort saveIdRecord;

        public IDService() {
            saveIdRecord = 0;
        }

        public ushort PickSaveId() {
            return ++saveIdRecord;
        }

        public void Reset() {
            saveIdRecord = 0;
        }

    }

}
