using BattleAxe;

namespace bxl.Data {
    internal static class Log {
        internal static ProcedureDefinition List =>
            new ProcedureDefinition("bxl.Log_List", Connection.Value);

        internal static ProcedureDefinition Update =>
            new ProcedureDefinition("bxl.Log_Update", Connection.Value);
    }
}
