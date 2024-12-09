using NapCatSharpLib.Data;
using RuriBot.Library.Module;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Event
{
    public interface IRRBotEventRegistry
    {
        void Register<T>(IRRBotModulePermissionOperation perm, Action<T> callback);
        void Unregister<T>(IRRBotModulePermissionOperation perm, Action<T> callback);
    }
}
