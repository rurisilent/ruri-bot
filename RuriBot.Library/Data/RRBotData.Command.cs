using System;
using System.Collections.Generic;
using System.Text;

namespace RuriBot.Library.Data
{
    public class RRBotDataCommand
    {
        public string CommandType { get; private set; }
        public string CommandSubType { get; private set; }
        public ushort CommandArgsCount { get; private set; }
        public List<string> CommandArgs { get; private set; }

        public RRBotDataCommand()
        {
            CommandType = "";
            CommandSubType = "";
            CommandArgsCount = 0;
            CommandArgs = new List<string>();
        }

        public void SetType(string type)
        {
            CommandType = type;
        }

        public void SetSubType(string subType)
        {
            CommandSubType = subType;
        }

        public void AddArgument(string arg)
        {
            CommandArgsCount++;
            CommandArgs.Add(arg);
        }

        public void ClearArgument(string arg)
        {
            CommandArgsCount = 0;
            CommandArgs.Clear();
        }
    }
}
