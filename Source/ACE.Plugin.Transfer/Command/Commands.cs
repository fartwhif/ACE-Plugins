using ACE.Server.Command;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ACE.Plugin.Transfer.Command
{
    static partial class Commands
    {
        public static void RegisterCommands(Dictionary<string, CommandHandlerInfo> commandHandlers)
        {
            var type = typeof(Commands);
            foreach (var method in type.GetMethods())
            {
                foreach (var attribute in method.GetCustomAttributes<CommandHandlerAttribute>())
                {
                    var commandHandler = new CommandHandlerInfo()
                    {
                        Handler = (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), method),
                        Attribute = attribute
                    };
                    commandHandlers[attribute.Command] = commandHandler;
                }
            }
        }
    }
}
