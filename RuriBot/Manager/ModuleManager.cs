using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using RuriBot.Library.Data;
using RuriBot.Library.Module;
using RuriBot.Library.Event;
using RuriBot.Core.IO;
using RuriBot.Core.Data;
using RuriBot.Library.IO;
using RuriBot.Library.Log;
using NapCatSharpLib.Event.Manager;
using NapCatSharpLib.API;
using NapCatSharpLib.Message;
using RuriBot.Library.Permission;
using NapCatSharpLib.Data;

namespace RuriBot.Core.Manager
{
    public class ModuleManager
    {
        public class ModulePermissionDummy : IRRBotModulePermissionOperation
        {
            public void ClearGroupPermission()
            {
                throw new NotImplementedException();
            }

            public void ClearPrivatePermission()
            {
                throw new NotImplementedException();
            }

            public bool IsAdmin(long id)
            {
                throw new NotImplementedException();
            }

            public bool IsGroupPermission(long id)
            {
                return true;
            }

            public bool IsPrivatePermission(long id)
            {
                return true;
            }

            public void SetAdmin(RRBotModulePermissionOperationType operation, long obj)
            {
                throw new NotImplementedException();
            }

            public void SetAdmin(RRBotModulePermissionOperationType operation, List<long> objList)
            {
                throw new NotImplementedException();
            }

            public void SetGroupPermission(RRBotModulePermissionOperationType operation, long obj)
            {
                throw new NotImplementedException();
            }

            public void SetGroupPermission(RRBotModulePermissionOperationType operation, List<long> objList)
            {
                throw new NotImplementedException();
            }

            public void SetGroupPermissionType(RRBotModulePermissionType type, bool clearPermission = true)
            {
                throw new NotImplementedException();
            }

            public void SetPrivatePermission(RRBotModulePermissionOperationType operation, long obj)
            {
                throw new NotImplementedException();
            }

            public void SetPrivatePermission(RRBotModulePermissionOperationType operation, List<long> objList)
            {
                throw new NotImplementedException();
            }

            public void SetPrivatePermissionType(RRBotModulePermissionType type, bool clearPermission = true)
            {
                throw new NotImplementedException();
            }
        }

        private Dictionary<string, RRBotModuleEntry> modules;

        //模块操作
        protected IRRBotCommandRegistry commandRegister;
        protected IRRBotEventRegistry eventRegister;
        protected NapCatAPI api;
        protected ModulePermissionDummy permissionDummy;

        //文件操作
        protected IRRBotModuleIO moduleIO;
        protected IRRBotPermission permission;

        //模块路径
        protected string modulePath;
        protected string moduleDataPath;

        //Log
        protected IRRBotLogger coreLogger;

        public int ModuleCount
        {
            get
            {
                return modules.Count;
            }
        }

        public ModuleManager(IRRBotCommandRegistry _cmdRegister, IRRBotEventRegistry _evtRegister, NapCatAPI _api, IRRBotModuleIO _moduleIO, string _modulePath, string _moduleDataPath, IRRBotPermission _permission, IRRBotLogger _logger = null)
        {
            commandRegister = _cmdRegister;
            eventRegister = _evtRegister;
            api = _api;
            moduleIO = _moduleIO;
            modulePath = _modulePath;
            moduleDataPath = _moduleDataPath;
            permission = _permission;
            permissionDummy = new ModulePermissionDummy();

            coreLogger = _logger;

            modules = new Dictionary<string, RRBotModuleEntry>();

            // Register
            _cmdRegister.RegisterGroup("rrbot", "modules", permissionDummy, GetModules);
            _cmdRegister.RegisterGroup("rrbot", "enable", permissionDummy, SetModuleGroupEnable);
            _cmdRegister.RegisterGroup("rrbot", "disable", permissionDummy, SetModuleGroupDisable);
        }

        ~ModuleManager()
        {
            commandRegister.UnregisterGroup("rrbot", "modules", permissionDummy, GetModules);
            commandRegister.UnregisterGroup("rrbot", "enable", permissionDummy, SetModuleGroupEnable);
            commandRegister.UnregisterGroup("rrbot", "disable", permissionDummy, SetModuleGroupDisable);
        }

        public void LoadModules()
        {
            if (!Directory.Exists(modulePath)) Directory.CreateDirectory(modulePath);

            string[] module_dlls = Directory.GetFiles(modulePath);
            coreLogger?.Log("[Ruri-Bot] 开始加载模块");
            coreLogger?.Log("[Ruri-Bot] ----------");

            int loadedCount = 0;

            foreach (string dll in module_dlls)
            {
                try
                {
                    Assembly mds = Assembly.LoadFrom(dll);
                    Type[] types = mds.GetTypes();
                    foreach (Type t in types)
                    {
                        if (t.IsSubclassOf(typeof(RRBotModuleEntry)))
                        {
                            object o = Activator.CreateInstance(t);
                            RRBotModuleEntry module = (RRBotModuleEntry)o;
                            module.ModuleEntryInit(commandRegister, eventRegister, api, moduleIO, permission, coreLogger, moduleDataPath);
                            if (module.ModuleFullID == "" || modules.ContainsKey(module.ModuleFullID))
                                continue; //log something when conflict
                            else
                            {
                                modules.Add(module.ModuleFullID, module);
                                coreLogger?.Log($"[Ruri-Bot] {module.ModuleName} {module.ModuleVersion} ({module.ModuleFullID}) by {module.ModuleAuthor}");
                                loadedCount++;
                            }
                            break;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            coreLogger?.Log("[Ruri-Bot] ----------");
            coreLogger?.Log($"[Ruri-Bot] 成功加载了 {loadedCount} 个模块");
        }

        protected async void GetModules(RRBotCommand command, NapCatMessageGroup source)
        {
            if (!permission.IsDeveloper(source.sender.user_id)) return;

            if (command.CommandArgsCount == 0)
            {
                CqMessageChain ret = new CqMessageChain();
                ret.Builder.AddText("[Ruri-Bot]\n");
                ret.Builder.AddText($"已加载模块个数：{ModuleCount}\n");
                ret.Builder.AddText("-------\n");
                int index = 0;
                foreach (var mod in modules)
                {
                    ret.Builder.AddText(mod.Key);
                    if (index < ModuleCount - 1) ret.Builder.AddText("\n");
                    index++;
                }

                await api.SendGroupMessage(source.group_id, ret);
            }
        }

        protected async void SetModuleGroupEnable(RRBotCommand command, NapCatMessageGroup source)
        {
            if (!permission.IsDeveloper(source.sender.user_id)) return;

            if (command.CommandArgsCount > 0)
            {
                if (command.CommandArgsCount == 1)
                {
                    var moduleId = command.CommandArgs[0];
                    /*if (long.TryParse(command.CommandArgs[3]))*/
                    if (modules.ContainsKey(moduleId))
                    {
                        if (!modules[moduleId].Permission.IsGroupPermission(source.group_id))
                            modules[moduleId].Permission.SetGroupPermission(modules[moduleId].Permission.GetGroupType() == RRBotModulePermissionType.blacklist ? RRBotModulePermissionOperationType.remove : RRBotModulePermissionOperationType.add, source.group_id);
                        CqMessageChain ret = new CqMessageChain();
                        ret.Builder.AddText("[Ruri-Bot]\n");
                        ret.Builder.AddText($"已在本群启用模块 {modules[moduleId].ModuleName} ({modules[moduleId].ModuleFullID})");
                        await api.SendGroupMessage(source.group_id, ret);
                    }
                    else
                    {
                        CqMessageChain ret = new CqMessageChain();
                        ret.Builder.AddText("[Ruri-Bot]\n");
                        ret.Builder.AddText($"模块 ID 无效，可能是未加载或输入错误");
                        await api.SendGroupMessage(source.group_id, ret);
                    }
                }
                else if (command.CommandArgsCount == 2)
                {
                    var moduleId = command.CommandArgs[0];
                    if (long.TryParse(command.CommandArgs[1], out var targetGroupId))
                    {
                        if (modules.ContainsKey(moduleId))
                        {
                            if (!modules[moduleId].Permission.IsGroupPermission(targetGroupId))
                                modules[moduleId].Permission.SetGroupPermission(modules[moduleId].Permission.GetGroupType() == RRBotModulePermissionType.blacklist ? RRBotModulePermissionOperationType.remove : RRBotModulePermissionOperationType.add, targetGroupId);
                            CqMessageChain ret = new CqMessageChain();
                            ret.Builder.AddText("[Ruri-Bot]\n");
                            ret.Builder.AddText($"已在群 {targetGroupId} 启用模块 {modules[moduleId].ModuleName} ({modules[moduleId].ModuleFullID})");
                            await api.SendGroupMessage(source.group_id, ret);
                        }
                        else
                        {
                            CqMessageChain ret = new CqMessageChain();
                            ret.Builder.AddText("[Ruri-Bot]\n");
                            ret.Builder.AddText($"模块 ID 无效，可能是未加载或输入错误");
                            await api.SendGroupMessage(source.group_id, ret);
                        }
                    }
                    else
                    {
                        CqMessageChain ret = new CqMessageChain();
                        ret.Builder.AddText("[Ruri-Bot]\n");
                        ret.Builder.AddText($"群号无效");
                        await api.SendGroupMessage(source.group_id, ret);
                    }
                }
            }
        }

        protected async void SetModuleGroupDisable(RRBotCommand command, NapCatMessageGroup source)
        {
            if (!permission.IsDeveloper(source.sender.user_id)) return;

            if (command.CommandArgsCount > 0)
            {
                if (command.CommandArgsCount == 1)
                {
                    var moduleId = command.CommandArgs[0];
                    /*if (long.TryParse(command.CommandArgs[3]))*/
                    if (modules.ContainsKey(moduleId))
                    {
                        if (modules[moduleId].Permission.IsGroupPermission(source.group_id))
                            modules[moduleId].Permission.SetGroupPermission(modules[moduleId].Permission.GetGroupType() == RRBotModulePermissionType.whitelist ? RRBotModulePermissionOperationType.remove : RRBotModulePermissionOperationType.add, source.group_id);
                        CqMessageChain ret = new CqMessageChain();
                        ret.Builder.AddText("[Ruri-Bot]\n");
                        ret.Builder.AddText($"已在本群禁用模块 {modules[moduleId].ModuleName} ({modules[moduleId].ModuleFullID})");
                        await api.SendGroupMessage(source.group_id, ret);
                    }
                    else
                    {
                        CqMessageChain ret = new CqMessageChain();
                        ret.Builder.AddText("[Ruri-Bot]\n");
                        ret.Builder.AddText($"模块 ID 无效，可能是未加载或输入错误");
                        await api.SendGroupMessage(source.group_id, ret);
                    }
                }
                else if (command.CommandArgsCount == 2)
                {
                    var moduleId = command.CommandArgs[0];
                    if (long.TryParse(command.CommandArgs[1], out var targetGroupId))
                    {
                        if (modules.ContainsKey(moduleId))
                        {
                            if (modules[moduleId].Permission.IsGroupPermission(targetGroupId))
                                modules[moduleId].Permission.SetGroupPermission(modules[moduleId].Permission.GetGroupType() == RRBotModulePermissionType.whitelist ? RRBotModulePermissionOperationType.remove : RRBotModulePermissionOperationType.add, targetGroupId);
                            CqMessageChain ret = new CqMessageChain();
                            ret.Builder.AddText("[Ruri-Bot]\n");
                            ret.Builder.AddText($"已在群 {targetGroupId} 禁用模块 {modules[moduleId].ModuleName} ({modules[moduleId].ModuleFullID})");
                            await api.SendGroupMessage(source.group_id, ret);
                        }
                        else
                        {
                            CqMessageChain ret = new CqMessageChain();
                            ret.Builder.AddText("[Ruri-Bot]\n");
                            ret.Builder.AddText($"模块 ID 无效，可能是未加载或输入错误");
                            await api.SendGroupMessage(source.group_id, ret);
                        }
                    }
                    else
                    {
                        CqMessageChain ret = new CqMessageChain();
                        ret.Builder.AddText("[Ruri-Bot]\n");
                        ret.Builder.AddText($"群号无效");
                        await api.SendGroupMessage(source.group_id, ret);
                    }
                }
            }
        }
    }
}
