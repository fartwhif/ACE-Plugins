using log4net;
using System;
using System.IO;

namespace ACE.Plugin.Transfer
{
    public static class TransferManagerConstants
    {
        public const int CookieLength = 8;
        public const int NonceLength = 16;
        public const string CookieChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string NonceChars = CookieChars;
    }
}
