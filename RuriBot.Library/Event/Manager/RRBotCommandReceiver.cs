using RuriBot.Library.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Event
{
    public class RRBotCommandReceiver
    {
        public RRBotEvent.BotPrivateCommandReceive OnReceivePrivateBotCommand;
        public RRBotEvent.BotGroupCommandReceive OnReceiveGroupBotCommand;
    }
}
