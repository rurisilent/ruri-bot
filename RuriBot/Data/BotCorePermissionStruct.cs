using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Core.Data
{
    public class BotCorePermissionStruct
    {
        public List<long> superuser;
        public List<long> admin;
        public List<long> blacklist;
    }
}
