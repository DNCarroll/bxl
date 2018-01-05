using BattleAxe;

namespace bxl.Data {
    public static class Error {

        public static ProcedureDefinition Insert {
            get {
                return new ProcedureDefinition("bxl.Error_Insert", Connection.Value);
            }
        }
    }
}
