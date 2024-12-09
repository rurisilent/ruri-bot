using System;
using System.Collections.Generic;
using System.Text;
using RuriBot.Library.Event;
using RuriBot.Library.IO;

namespace RuriBot.Library.Module
{
    public abstract class RRBotModuleBase<TConfig, TConfigData> : RRBotModuleEntry
        where TConfig : RRBotModuleConfigBase<TConfigData>
    {
        public TConfig Config { get; protected set; }

        protected override void ConfigurationInit()
        {
            Permission = new RRBotModulePermission();
            Permission.Init(ModuleFullID, fileIO);

            Config = Activator.CreateInstance<TConfig>();
            Config.Init(ModuleFullID, fileIO);
        }
    }
}
