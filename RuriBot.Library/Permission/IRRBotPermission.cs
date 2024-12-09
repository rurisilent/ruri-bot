using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Permission
{
    public interface IRRBotPermission
    {
        bool IsDeveloper(long id);
        bool IsAdmin(long id);
    }
}
