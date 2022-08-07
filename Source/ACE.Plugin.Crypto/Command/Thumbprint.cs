using ACE.Entity.Enum;
using ACE.Plugin.Crypto.Managers;
using ACE.Server.Command;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages.Messages;
using System;

namespace ACE.Plugin.Crypto.Command
{
    static partial class Commands
    {
        [CommandHandler("thumbprint", AccessLevel.Admin, CommandHandlerFlag.None, 0, "Reveals this server's certificate thumbprint.")]
        public static void Thumbprint(Session session, params string[] parameters)
        {
            string print = $"Server thumbprint: {CertificateManager.Thumbprint}";
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
