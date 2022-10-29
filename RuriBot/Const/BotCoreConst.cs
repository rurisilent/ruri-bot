using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RuriBot.Core.Const
{
    public static class BotCoreConst
    {
        public static readonly string ModulePath = Path.Combine("modules");

        public static readonly string CoreDataPath = Path.Combine("data");
        public static readonly string ModuleDataPath = Path.Combine("data", "modules");
    }
}
