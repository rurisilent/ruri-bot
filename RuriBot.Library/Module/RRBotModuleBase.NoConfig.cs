﻿using System;
using System.Collections.Generic;
using System.Text;
using CqHttpSharp.Event.Manager;
using CqHttpSharp.API;
using RuriBot.Library.Event;
using RuriBot.Library.IO;

namespace RuriBot.Library.Module
{
    public abstract class RRBotModuleBase : RRBotModuleEntry
    {
        protected override void ConfigurationInit()
        {
            Permission = new RRBotModulePermission();
            Permission.Init(ModuleFullID, fileIO);
        }
    }
}
