using NapCatSharpLib.Data;
using RuriBot.Library.Data;
using RuriBot.Library.Module;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Event
{
    public interface IRRBotCommandRegistry
    {
        void RegisterPrivate(string cmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessagePrivate> callback);
        void RegisterPrivate(string cmd, string subCmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessagePrivate> callback);
        void RegisterGroup(string cmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessageGroup> callback);
        void RegisterGroup(string cmd, string subCmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessageGroup> callback);
        void UnregisterPrivate(string cmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessagePrivate> callback);
        void UnregisterPrivate(string cmd, string subCmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessagePrivate> callback);
        void UnregisterGroup(string cmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessageGroup> callback);
        void UnregisterGroup(string cmd, string subCmd, IRRBotModulePermissionOperation perm, Action<RRBotCommand, NapCatMessageGroup> callback);
    }
}
