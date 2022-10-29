using System;
using System.Collections.Generic;
using RuriBot.Library.IO;

namespace RuriBot.Library.Module
{
    public abstract class RRBotModulePermissionBase<T> : IRRBotModulePermissionOperation
        where T : RRBotModulePermissionStruct
    {
        public T data;

        protected bool loaded = false;
        protected string module_id;
        private IRRBotModuleIO moduleIO;

        public RRBotModulePermissionBase() { loaded = false; }

        public void Init(string _moduleId, IRRBotModuleIO _moduleIO)
        {
            module_id = _moduleId;
            moduleIO = _moduleIO;

            data = Activator.CreateInstance<T>();

            ReadData();
            SaveData();

            loaded = true;
        }

        public void SetPrivatePermissionType(RRBotModulePermissionType type, bool clearPermission = true)
        {
            if (!loaded) return;

            if (data.private_type != type)
            {
                data.private_type = type;
                if (clearPermission) data.private_permission.Clear();
            }

            SaveData();
        }

        public void ClearPrivatePermission()
        {
            if (!loaded) return;

            data.private_permission.Clear();

            SaveData();
        }

        public void SetPrivatePermission(RRBotModulePermissionOperationType operation, long obj)
        {
            if (!loaded) return;

            if (operation == RRBotModulePermissionOperationType.add)
                data.private_permission.Add(obj);
            else if (operation == RRBotModulePermissionOperationType.remove)
                data.private_permission.Remove(obj);

            SaveData();
        }

        public void SetPrivatePermission(RRBotModulePermissionOperationType operation, List<long> objList)
        {
            if (!loaded) return;

            if (operation == RRBotModulePermissionOperationType.add)
                foreach (var obj in objList) data.private_permission.Add(obj);
            else if (operation == RRBotModulePermissionOperationType.remove)
                foreach (var obj in objList) data.private_permission.Remove(obj);

            SaveData();
        }

        public void SetGroupPermissionType(RRBotModulePermissionType type, bool clearPermission = true)
        {
            if (!loaded) return;

            if (data.group_type != type)
            {
                data.group_type = type;
                if (clearPermission) data.group_permission.Clear();
            }

            SaveData();
        }

        public void ClearGroupPermission()
        {
            if (!loaded) return;

            data.group_permission.Clear();

            SaveData();
        }

        public void SetGroupPermission(RRBotModulePermissionOperationType operation, long obj)
        {
            if (!loaded) return;

            if (operation == RRBotModulePermissionOperationType.add)
                data.group_permission.Add(obj);
            else if (operation == RRBotModulePermissionOperationType.remove)
                data.group_permission.Remove(obj);

            SaveData();
        }

        public void SetGroupPermission(RRBotModulePermissionOperationType operation, List<long> objList)
        {
            if (!loaded) return;

            if (operation == RRBotModulePermissionOperationType.add)
                foreach (var obj in objList) data.group_permission.Add(obj);
            else if (operation == RRBotModulePermissionOperationType.remove)
                foreach (var obj in objList) data.group_permission.Remove(obj);

            SaveData();
        }

        public void SetAdmin(RRBotModulePermissionOperationType operation, long obj)
        {
            if (!loaded) return;

            if (operation == RRBotModulePermissionOperationType.add)
                data.admin.Add(obj);
            else if (operation == RRBotModulePermissionOperationType.remove)
                data.admin.Remove(obj);

            SaveData();
        }

        public void SetAdmin(RRBotModulePermissionOperationType operation, List<long> objList)
        {
            if (!loaded) return;

            if (operation == RRBotModulePermissionOperationType.add)
                foreach (var obj in objList) data.admin.Add(obj);
            else if (operation == RRBotModulePermissionOperationType.remove)
                foreach (var obj in objList) data.admin.Remove(obj);

            SaveData();
        }

        public bool IsPrivatePermission(long id)
        {
            if (data.private_permission.Contains(id))
            {
                if (data.private_type == RRBotModulePermissionType.blacklist) return false;
                else if (data.private_type == RRBotModulePermissionType.whitelist) return true;
                return false;
            }
            else
            {
                if (data.private_type == RRBotModulePermissionType.blacklist) return true;
                else if (data.private_type == RRBotModulePermissionType.whitelist) return false;
                return true;
            }
        }

        public bool IsGroupPermission(long id)
        {
            if (data.group_permission.Contains(id))
            {
                if (data.group_type == RRBotModulePermissionType.blacklist) return false;
                else if (data.group_type == RRBotModulePermissionType.whitelist) return true;
                return false;
            }
            else
            {
                if (data.group_type == RRBotModulePermissionType.blacklist) return true;
                else if (data.group_type == RRBotModulePermissionType.whitelist) return false;
                return true;
            }
        }

        public bool IsAdmin(long id)
        {
            if (data.admin.Contains(id)) return true;
            else return false;
        }

        public RRBotModulePermissionType GetPrivateType() { return data.private_type; }
        public RRBotModulePermissionType GetGroupType() { return data.group_type; }

        protected void ReadData()
        {
            var tempData = moduleIO.ReadJson<T>(module_id, "", "permission");
            if (tempData != null) data = tempData;

            loaded = true;
        }

        protected void SaveData()
        {
            moduleIO.SaveJson(module_id, "", "permission", data);
        }
    }
}
