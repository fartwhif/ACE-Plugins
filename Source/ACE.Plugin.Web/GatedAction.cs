namespace ACE.Plugin.Web
{
    internal class GatedAction
    {
        public Action Action { get; set; } = null;
        public ManualResetEvent CompletionToken { get; set; } = null;
    }
}
