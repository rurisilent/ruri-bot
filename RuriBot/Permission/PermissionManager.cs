using RuriBot.Core.Data;
using RuriBot.Core.IO;
using RuriBot.Library.Permission;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Core.Permission
{
    internal class PermissionManager : IRRBotPermission
    {
        BotCorePermission data;

        public PermissionManager(CoreIO io)
        {
            data = new BotCorePermission("permission", "permission", io);
        }

        ~PermissionManager()
        {

        }

        public bool IsAdmin(long id)
        {
            return data.IsAdmin(id);
        }

        public bool IsDeveloper(long id)
        {
            return data.IsSuperUser(id);
        }
    }
}
