using ACE.Entity.Enum;
using ACE.Server.Command;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages.Messages;
using System;

namespace ACE.Plugin.Transfer.Command
{
    static partial class Commands
    {
        [CommandHandler("transfer", AccessLevel.Player, CommandHandlerFlag.None, 0, "stub")]
        public static void HandleCert(Session session, params string[] parameters)
        {
            string print = $"use the website for these character features";
            if (session != null)
            {
                session.Network.EnqueueSend(new GameMessageSystemChat(print, ChatMessageType.Broadcast));
            }
            else
            {
                Console.WriteLine(print);
            }
        }
    }
}
