using BattleAxe;

namespace bxl.Data {
    internal static class Field {
        internal static ProcedureDefinition List =>
            new ProcedureDefinition("bxl.TemplateField_List", Connection.Value);

        internal static ProcedureDefinition Update =>
            new ProcedureDefinition("bxl.TemplateField_Update", Connection.Value);
    }
}
