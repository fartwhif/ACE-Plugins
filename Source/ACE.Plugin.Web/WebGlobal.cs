namespace ACE.Plugin.Web
{
    public sealed class WebGlobal
    {
        private static readonly Lazy<WebGlobal> lazy = new Lazy<WebGlobal>(() => new WebGlobal());
        public static WebGlobal Instance => lazy.Value;
        private WebGlobal() { _ResultOfInitSink = null; }
        private TaskCompletionSource<bool> _ResultOfInitSink;
        public static TaskCompletionSource<bool> ResultOfInitSink { get => Instance._ResultOfInitSink; set => Instance._ResultOfInitSink = value; }
        private TaskCompletionSource<bool> _ResultOfHostRunSink;
        public static TaskCompletionSource<bool> ResultOfHostRunSink { get => Instance._ResultOfHostRunSink; set => Instance._ResultOfHostRunSink = value; }
    }
}
