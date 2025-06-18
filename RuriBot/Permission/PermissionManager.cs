using RuriBot.Core.Data;
using RuriBot.Core.IO;
using RuriBot.Library.Event;
using RuriBot.Library.Permission;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Core.Permission
{
    internal class PermissionManager : IRRBotPermission
    {
        BotCorePermission data;
        IRRBotCommandRegistry commandRegistry;

        public PermissionManager(CoreIO io, IRRBotCommandRegistry cmdReg)
        {
            data = new BotCorePermission("permission", "permission", io);
            commandRegistry = cmdReg;
        }

        ~PermissionManager()
        {

        }

        public bool IsAdmin(long id)
        {
            return data.IsAdmin(id) || data.IsSuperUser(id);
        }

        public bool IsDeveloper(long id)
        {
            return data.IsSuperUser(id);
        }
    }
}
