using BattleAxe;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bxl.Test {
    [TestFixture]
    public class XlsxWithSheetAndRowDesignated {

        [Test]
        public void TestIt() {

            bxl.Data.Connection.Value = "?";
            var template = (new ProcedureDefinition("bxl.Template_First", bxl.Data.Connection.Value)).FirstOrDefault(new bxl.Model.Template { TemplateId = 21 });
            var importer = new Import(template);
            importer.Single(@"?");
        }

    }
}
