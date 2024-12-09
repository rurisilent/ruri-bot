using NapCatSharpLib.Data;
using NapCatSharpLib.Event;
using NapCatSharpLib.Event.Manager;
using RuriBot.Library.Event;
using RuriBot.Library.Module;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Core.Manager
{
    public class EventManager : IRRBotEventRegistry
    {
        Dictionary<string, HashSet<(IRRBotModulePermissionOperation, Delegate)>> handlers = new Dictionary<string, HashSet<(IRRBotModulePermissionOperation, Delegate)>>();
        NapCatEventManager rawEventManager;

        public EventManager(NapCatEventManager source)
        {
            rawEventManager = source;

            // Reg All Events
            rawEventManager.OnEventMessagePrivate += (x) => React(x);
            rawEventManager.OnEventMessageGroup += (x) => React(x);
            rawEventManager.OnEventNoticeGroupFileUpload += (x) => React(x);
            rawEventManager.OnEventNoticeGroupAdminChange += (x) => React(x);
            rawEventManager.OnEventNoticeGroupDecrease += (x) => React(x);
            rawEventManager.OnEventNoticeGroupIncrease += (x) => React(x);
            rawEventManager.OnEventNoticeGroupBan += (x) => React(x);
            rawEventManager.OnEventNoticeAddFriend += (x) => React(x);
            rawEventManager.OnEventNoticeGroupRecall += (x) => React(x);
            rawEventManager.OnEventNoticePrivateRecall += (x) => React(x);
            rawEventManager.OnEventNoticePrivatePoke += (x) => React(x);
            rawEventManager.OnEventNoticeGroupPoke += (x) => React(x);
            rawEventManager.OnEventNoticeGroupLuckyKing += (x) => React(x);
            rawEventManager.OnEventNoticeGroupHonor += (x) => React(x);
            rawEventManager.OnEventNoticeGroupCardChange += (x) => React(x);
            rawEventManager.OnEventNoticeOfflineFileReceive += (x) => React(x);
            rawEventManager.OnEventNoticeClientStatusChange += (x) => React(x);
            rawEventManager.OnEventNoticeGroupEssence += (x) => React(x);
            rawEventManager.OnEventRequestFriend += (x) => React(x);
            rawEventManager.OnEventRequestGroup += (x) => React(x);
        }


        public void Register<T>(IRRBotModulePermissionOperation perm, Action<T> callback)
        {
            if (handlers.TryGetValue(nameof(T), out var cbs))
            {
                if (!cbs.Contains((perm, callback)))
                {
                    cbs.Add((perm, callback));
                }
            }
            else
            {
                handlers.Add(nameof(T), new HashSet<(IRRBotModulePermissionOperation, Delegate)>() { (perm, callback) });
            }
        }

        public void Unregister<T>(IRRBotModulePermissionOperation perm, Action<T> callback)
        {
            if (handlers.TryGetValue(nameof(T), out var cbs))
            {
                if (cbs.Contains((perm, callback)))
                {
                    cbs.Remove((perm, callback));
                }
            }
        }

        public void React<T>(T data)
        {
            bool isPrivate = IsPrivateEvent<T>();
            long id;
            if (isPrivate) id = GetEventUserId(data);
            else id = GetEventGroupId(data);
            if (id == -1) return;

            if (handlers.TryGetValue(nameof(T), out var cbs))
            {
                foreach (var value in cbs)
                {
                    if (isPrivate && value.Item1.IsPrivatePermission(id) && value.Item2 is Action<T> act) act?.Invoke(data);
                    else if (!isPrivate && value.Item1.IsGroupPermission(id) && value.Item2 is Action<T> act2) act2?.Invoke(data);
                }
            }
        }

        long GetEventUserId<T>(T data)
        {
            if (data is NapCatMessagePrivate msgPrivate) return msgPrivate.user_id;
            if (data is NapCatNoticeAddFriend ntcAddFriend) return ntcAddFriend.user_id;
            if (data is NapCatNoticePrivateRecall ntcPrivateRecall) return ntcPrivateRecall.user_id;
            if (data is NapCatNoticePrivatePoke ntcPrivatePoke) return ntcPrivatePoke.user_id;
            if (data is NapCatNoticeOfflineFileReceive ntcOfflineFile) return ntcOfflineFile.user_id;
            if (data is NapCatRequestFriend reqFriend) return reqFriend.user_id;

            return -1;
        }

        long GetEventGroupId<T>(T data)
        {
            if (data is NapCatMessageGroup msgGroup) return msgGroup.group_id;
            if (data is NapCatNoticeGroupFileUpload ntcGroupFileUpload) return ntcGroupFileUpload.group_id;
            if (data is NapCatNoticeGroupAdminChange ntcGroupAdminChange) return ntcGroupAdminChange.group_id;
            if (data is NapCatNoticeGroupDecrease ntcGroupDecrease) return ntcGroupDecrease.group_id;
            if (data is NapCatNoticeGroupIncrease ntcGroupIncrease) return ntcGroupIncrease.group_id;
            if (data is NapCatNoticeGroupBan ntcGroupBan) return ntcGroupBan.group_id;
            if (data is NapCatNoticeGroupRecall ntcGroupRecall) return ntcGroupRecall.group_id;
            if (data is NapCatNoticeGroupPoke ntcGroupPoke) return ntcGroupPoke.group_id;
            if (data is NapCatNoticeGroupLuckyKing ntcGroupLuckyKing) return ntcGroupLuckyKing.group_id;
            if (data is NapCatNoticeGroupHonor ntcGroupHonor) return ntcGroupHonor.group_id;
            if (data is NapCatNoticeGroupCardChange ntcGroupCardChange) return ntcGroupCardChange.group_id;
            if (data is NapCatNoticeGroupEssence ntcGroupEssence) return ntcGroupEssence.group_id;
            if (data is NapCatRequestGroup reqGroup) return reqGroup.group_id;

            return -1;
        }

        bool IsPrivateEvent<T>()
        {
            if (typeof(T) == typeof(NapCatMessagePrivate)) return true;
            if (typeof(T) == typeof(NapCatNoticePrivateRecall)) return true;
            if (typeof(T) == typeof(NapCatNoticePrivatePoke)) return true;
            if (typeof(T) == typeof(NapCatRequestFriend)) return true;
            if (typeof(T) == typeof(NapCatNoticeAddFriend)) return true;
            if (typeof(T) == typeof(NapCatNoticeOfflineFileReceive)) return true;
            return false;
        }
    }
}
