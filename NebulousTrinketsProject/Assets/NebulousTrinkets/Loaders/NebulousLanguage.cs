using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm.Loaders;

namespace NebulousTrinkets
{
    public class NebulousLanguage : LanguageLoader<NebulousLanguage>
    {
        public override string AssemblyDir => NebulousMain.InstalledDirectory;

        public override string LanguagesFolderName => "NebulousLanguage";

        internal void Init()
        {
            LoadLanguages();
        }
    }
}