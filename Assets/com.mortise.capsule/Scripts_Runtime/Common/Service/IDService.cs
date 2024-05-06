namespace MortiseFrame.Capsule {

    public class IDService {

        public byte saveIdRecord;

        public IDService() {
            saveIdRecord = 0;
        }

        public byte PickSaveId() {
            return ++saveIdRecord;
        }

        public void Reset() {
            saveIdRecord = 0;
        }

    }

}