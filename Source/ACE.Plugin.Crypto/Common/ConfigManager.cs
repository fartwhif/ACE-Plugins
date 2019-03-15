using DouglasCrockford.JsMin;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ACE.Plugin.Crypto.Common
{
    public static class CryptoConfigManager
    {
        public static CryptoConfiguration Config { get; private set; }

        /// <summary>
        /// initializes from a preloaded configuration
        /// </summary>
        public static void Initialize(CryptoConfiguration configuration)
        {
            Config = configuration;
        }

        /// <summary>
        /// initializes from a crypto.js in &lt;ACE&gt;/Plugins/ACE.Plugin.Crypto/crypto.js
        /// </summary>
        public static void Initialize()
        {
            string fp = Path.Combine(Path.Combine(Path.Combine(Environment.CurrentDirectory, "Plugins"), "ACE.Plugin.Crypto"), "crypto.js");
            string fpChoice = null;
            try
            {
                if (File.Exists(fp))
                {
                    fpChoice = fp;
                }
                else
                {
                    Console.WriteLine("Configuration file crypto.js is missing.");
                    throw new Exception("missing configuration file: crypto.js");
                }
                Config = JsonConvert.DeserializeObject<CryptoConfiguration>(new JsMinifier().Minify(File.ReadAllText(fpChoice)));
            }
            catch (Exception exception)
            {
                Console.WriteLine("An exception occured while loading the crypto configuration file!");
                Console.WriteLine($"Exception: {exception.Message}");
                throw;
            }
        }
    }
}
