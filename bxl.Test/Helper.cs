using System.IO;
using BattleAxe;
using System.Reflection;

namespace bxl {
    public static class Helper {
        public static string ExecuteablePath() {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            string folder = Path.GetDirectoryName(thisAssembly.Location);
            folder = folder.Substring(0, folder.IndexOf("bxl.Test"));
            return folder;
        }

        public static void SetConnectionString() {
            Data.Connection.Value = "?";
        }
        
    }
}
