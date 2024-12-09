using System;
using System.Collections.Generic;
using System.Text;
using NapCatSharpLib.Data;
using RuriBot.Library.Data;
using RuriBot.Library.Module;

namespace RuriBot.Library.Event
{
    public class CommandManager : IRRBotCommandRegistry
    {
        Dictionary<string, Dictionary<string, HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessagePrivate>)>>> m_privateCommandRegistry
            = new Dictionary<string, Dictionary<string, HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessagePrivate>)>>>();
        Dictionary<string, Dictionary<string, HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessageGroup>)>>> m_groupCommandRegistry
            = new Dictionary<string, Dictionary<string, HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessageGroup>)>>>();

        public void RegisterPrivate(string cmd, string subCmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessagePrivate> callback)
        {
            if (!m_privateCommandRegistry.ContainsKey(cmd))
            {
                m_privateCommandRegistry.Add(cmd, new Dictionary<string, HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessagePrivate>)>>()
                {
                    { subCmd, new HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessagePrivate>)>() { (perm, callback) } }
                });
            }
            else
            {
                if (!m_privateCommandRegistry[cmd].ContainsKey(subCmd))
                {
                    m_privateCommandRegistry[cmd].Add(subCmd, new HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessagePrivate>)>() { (perm, callback) });
                }
                else
                {
                    if (!m_privateCommandRegistry[cmd][subCmd].Contains((perm, callback))) m_privateCommandRegistry[cmd][subCmd].Add((perm, callback));
                }
            }
        }
        public void RegisterGroup(string cmd, string subCmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessageGroup> callback)
        {
            if (!m_groupCommandRegistry.ContainsKey(cmd))
            {
                m_groupCommandRegistry.Add(cmd, new Dictionary<string, HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessageGroup>)>>()
                {
                    { subCmd, new HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessageGroup>)>() { (perm, callback) } }
                });
            }
            else
            {
                if (!m_groupCommandRegistry[cmd].ContainsKey(subCmd))
                {
                    m_groupCommandRegistry[cmd].Add(subCmd, new HashSet<(IRRBotModulePermissionOperation, Action<RRBotCommand, NapCatMessageGroup>)>() { (perm, callback) });
                }
                else
                {
                    if (!m_groupCommandRegistry[cmd][subCmd].Contains((perm, callback))) m_groupCommandRegistry[cmd][subCmd].Add((perm, callback));
                }
            }
        }

        public void UnregisterPrivate(string cmd, string subCmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessagePrivate> callback)
        {
            if (m_privateCommandRegistry.TryGetValue(cmd, out var subReg))
            {
                if (subReg.TryGetValue(subCmd, out var cbCollection))
                {
                    if (cbCollection.Contains((perm, callback))) cbCollection.Remove((perm, callback));
                }
            }
        }

        public void UnregisterGroup(string cmd, string subCmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessageGroup> callback)
        {
            if (m_groupCommandRegistry.TryGetValue(cmd, out var subReg))
            {
                if (subReg.TryGetValue(subCmd, out var cbCollection))
                {
                    if (cbCollection.Contains((perm, callback))) cbCollection.Remove((perm, callback));
                }
            }
        }

        public bool ReactPrivate(RRBotCommand cmd, NapCatMessagePrivate msg, out string ret)
        {
            if (m_privateCommandRegistry.TryGetValue(cmd.CommandType, out var subReg))
            {
                if (subReg.TryGetValue(cmd.CommandSubType, out var cbCollection))
                {
                    try
                    {
                        foreach (var cb in cbCollection)
                        {
                            if (cb.Item1.IsPrivatePermission(msg.user_id)) cb.Item2?.Invoke(cmd, msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        ret = $"{ex.Message}\n\n{ex.StackTrace}";
                        return false;
                    }
                }
            }
            ret = "success";
            return true;
        }

        public bool ReactGroup(RRBotCommand cmd, NapCatMessageGroup msg, out string ret)
        {
            if (m_groupCommandRegistry.TryGetValue(cmd.CommandType, out var subReg))
            {
                if (subReg.TryGetValue(cmd.CommandSubType, out var cbCollection))
                {
                    try
                    {
                        foreach (var cb in cbCollection)
                        {
                            if (cb.Item1.IsGroupPermission(msg.group_id)) cb.Item2?.Invoke(cmd, msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        ret = $"{ex.Message}\n\n{ex.StackTrace}";
                        return false;
                    }
                }
            }
            ret = "success";
            return true;
        }

        public void RegisterPrivate(string cmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessagePrivate> callback)
        {
            RegisterPrivate(cmd, "", perm, callback);
        }

        public void RegisterGroup(string cmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessageGroup> callback)
        {
            RegisterGroup(cmd, "", perm, callback);
        }

        public void UnregisterPrivate(string cmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessagePrivate> callback)
        {
            UnregisterPrivate(cmd, "", perm, callback);
        }

        public void UnregisterGroup(string cmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessageGroup> callback)
        {
            UnregisterGroup(cmd, "", perm, callback);
        }
    }
}
