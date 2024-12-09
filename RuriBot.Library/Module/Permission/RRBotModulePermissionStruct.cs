using System.Collections.Generic;

namespace RuriBot.Library.Module
{
    public class RRBotModulePermissionStruct
    {
        public RRBotModulePermissionType private_type = RRBotModulePermissionType.blacklist;
        public RRBotModulePermissionType group_type = RRBotModulePermissionType.whitelist;
        public List<long> private_permission = new List<long>();
        public List<long> group_permission = new List<long>();
        public List<long> admin = new List<long>();
        public List<long> blacklist_user = new List<long>();
    }
}
