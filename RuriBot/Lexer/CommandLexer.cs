using System;
using System.Collections.Generic;
using System.Text;
using NapCatSharpLib.Message;
using RuriBot.Library.Data;
using RuriBot.Library.Event;

namespace RuriBot.Core.Lexer
{
    public class CommandLexer
    {
        public RRBotCommand MessageLexer(string message)
        {
            return MessageLexer(new CqMessageChain(message));
        }

        public RRBotCommand MessageLexer(CqMessageChain messageChain)
        {
            RRBotCommand ret = new RRBotCommand();

            if (messageChain.Count <= 0) return null;
            else
            {
                //先去除所有非 Text 类型信息
                StringBuilder commandLine = new StringBuilder();
                foreach (var msg in messageChain.Messages)
                {
                    if (msg.type == CqCode.text)
                    {
                        CqMessageDataText textMsg = new CqMessageDataText(msg);
                        if (textMsg.text != null) commandLine.Append(textMsg.text);
                    }
                }

                string commandLineString = commandLine.ToString();
                int length = commandLineString.Length;
                int index = 0;
                int splitCount = 0; //已分割内容数
                bool isCommand = false;

                commandLine.Clear();
                
                while (index < length)
                {
                    if (!isCommand)
                    {
                        //忽略前导空格和换行
                        if (commandLineString[index] != ' ' && commandLineString[index] != '\n')
                        {
                            if (commandLineString[index] == '/') isCommand = true;
                            else return null;
                        }
                    }
                    else //确认这个是命令
                    {
                        //空格分割
                        if (commandLineString[index] == ' ')
                        {
                            if (commandLine.Length > 0)
                            {
                                if (splitCount == 0)
                                {
                                    ret.SetType(commandLine.ToString().ToLower());
                                }
                                else if (splitCount == 1)
                                {
                                    ret.SetSubType(commandLine.ToString().ToLower());
                                }
                                else
                                {
                                    ret.AddArgument(commandLine.ToString());
                                }

                                splitCount++;
                                commandLine.Clear();
                            }
                            else
                            {
                                //忽略过多的空格
                            }
                        }
                        else
                        {
                            if (commandLineString[index] != '\n') //忽略换行
                            {
                                commandLine.Append(commandLineString[index]);
                            }
                        }
                    }

                    index++;
                }

                //残余数据处理
                if (commandLine.Length > 0)
                {
                    if (splitCount == 0)
                    {
                        ret.SetType(commandLine.ToString().ToLower());
                    }
                    else if (splitCount == 1)
                    {
                        ret.SetSubType(commandLine.ToString().ToLower());
                    }
                    else
                    {
                        ret.AddArgument(commandLine.ToString());
                    }

                    splitCount++;
                }

                if (splitCount == 0) return null;
                else return ret;
            }
        }
    }
}
