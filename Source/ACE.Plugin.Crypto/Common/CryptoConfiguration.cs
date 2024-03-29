﻿namespace ACE.Plugin.Crypto.Common
{
    public class CryptoConfigurationOuter
    {
        public CryptoConfiguration CryptoConfiguration { get; set; }
    }
    public class CryptoConfiguration
    {
        /// <summary>
        /// For hosting the web services.<para/>
        /// Specify the full path to a PKCS #12 file containing a private key with its X.509 certificate.<para/>
        /// Leave this null to automatically generate a new self-signed one for this server upon first server start with web services enabled.<para/>
        /// Automatic creation will save and use the file at:<para/>
        /// Linux:  &lt;ACE&gt;/certificates/web.pfx<para/>
        /// Windows:  &lt;ACE&gt;\certificates\web.pfx
        /// </summary>
        public string FilePathKeyAndCertBundleWeb { get; set; }

        /// <summary>
        /// If supplying an encrypted PKCS #12 file for web services and want to manually enter the password via the console during server startup.
        /// </summary>
        public bool PasswordStartupPromptKeyAndCertBundleWeb { get; set; }

        /// <summary>
        /// If supplying an encrypted PKCS #12 file for web services and not using password startup prompt mode.  Disables password startup prompt mode for web services if supplied.  Default empty/no password, example: "mypassword"
        /// </summary>
        public string PasswordKeyAndCertBundleWeb { get; set; }

        /// <summary>
        /// For signing character data.<para/>
        /// Applicable when using character transfer features.<para/>
        /// Specify the full path to a PKCS #12 file containing a private key with its X.509 certificate.<para/>
        /// Leave this null to automatically generate a new self-signed one for this server upon first server start with one or more character transfer features enabled.<para/>
        /// Automatic creation will save and use the file at:<para/>
        /// Linux: &lt;ACE&gt;/certificates/datasigner.pfx<para/>
        /// Windows: &lt;ACE&gt;\certificates\datasigner.pfx<para/>
        /// After creation keep the file private and it's a good idea to backup the file to a private location.<para/>
        /// If an adversary obtains it they could use it maliciously against servers that trust its certificate/server thumbprint!<para/>
        /// If this file is lost all servers that trust its certificate/server thumbprint will need to be reconfigured and all local pending migrations cancelled.<para/>
        /// If the world name is changed then this file should be moved to the new location.<para/>
        /// Note: &lt;worldname&gt; is a version of the configured world name of only the characters a-z, 0-9, no spaces, no specials, converted to lowercase.
        /// </summary>
        public string FilePathKeyAndCertBundleDataSigner { get; set; }

        /// <summary>
        /// If supplying an encrypted PKCS #12 file for DataSigner and want to manually enter the password via the console during server startup.
        /// </summary>
        public bool PasswordStartupPromptKeyAndCertBundleDataSigner { get; set; }

        /// <summary>
        /// If supplying an encrypted PKCS #12 file for DataSigner and not using password startup prompt mode.  Disables password startup prompt mode for DataSigner if supplied.  Default empty/no password, example: "mypassword"
        /// </summary>
        public string PasswordKeyAndCertBundleDataSigner { get; set; }
    }
}
