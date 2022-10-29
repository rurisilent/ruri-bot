using RuriBot.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Core.Data
{
    public class BotCorePermission : BotCoreDataBase<BotCorePermissionStruct>
    {
        public BotCorePermission(string _dataPath, string _dataFilename, IRRBotCoreIO _coreIO) : base(_dataPath, _dataFilename, _coreIO) { }

        protected override void SetDefaultValue()
        {
            data = new BotCorePermissionStruct();

            data.superuser = new List<long>();
            data.admin = new List<long>();
            data.blacklist = new List<long>();
        }

        public bool IsSuperUser(long id) { return data.superuser.Contains(id); }
        public bool IsAdmin(long id) { return data.admin.Contains(id); }
        public bool IsInBlackList(long id) { return data.blacklist.Contains(id); }
    }
}
