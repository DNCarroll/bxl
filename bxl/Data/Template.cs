using BattleAxe;

namespace bxl.Data {
    internal static class Template {
        internal static ProcedureDefinition List =>
            new ProcedureDefinition("bxl.Template_List", Connection.Value);

        internal static ProcedureDefinition Update =>
            new ProcedureDefinition("bxl.Template_Update", Connection.Value);

        public static ProcedureDefinition EmailOnError =>
            new ProcedureDefinition("bxl.EmailOnError", Connection.Value);
    }
}
