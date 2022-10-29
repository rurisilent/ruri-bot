using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Module
{
    public interface IRRBotModulePermissionOperation
    {
        void SetPrivatePermissionType(RRBotModulePermissionType type, bool clearPermission = true);
        void ClearPrivatePermission();
        void SetPrivatePermission(RRBotModulePermissionOperationType operation, long obj);
        void SetPrivatePermission(RRBotModulePermissionOperationType operation, List<long> objList);
        void SetGroupPermissionType(RRBotModulePermissionType type, bool clearPermission = true);
        void ClearGroupPermission();
        void SetGroupPermission(RRBotModulePermissionOperationType operation, long obj);
        void SetGroupPermission(RRBotModulePermissionOperationType operation, List<long> objList);
        void SetAdmin(RRBotModulePermissionOperationType operation, long obj);
        void SetAdmin(RRBotModulePermissionOperationType operation, List<long> objList);
        bool IsPrivatePermission(long id);
        bool IsGroupPermission(long id);
        bool IsAdmin(long id);
    }
}
