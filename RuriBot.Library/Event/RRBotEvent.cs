using System;
using System.Collections.Generic;
using System.Text;
using CqHttpSharp.Data;
using RuriBot.Library.Data;

namespace RuriBot.Library.Event
{
    public class RRBotEvent
    {
        public delegate void BotPrivateCommandReceive(RRBotDataCommand command, CqHttpMessagePrivate source);
        public delegate void BotGroupCommandReceive(RRBotDataCommand command, CqHttpMessageGroup source);
    }
}
