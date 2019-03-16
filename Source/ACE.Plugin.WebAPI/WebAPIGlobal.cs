using System;
using System.Threading.Tasks;

namespace ACE.Plugin.WebAPI
{
    public sealed class WebAPIGlobal
    {
        private static readonly Lazy<WebAPIGlobal> lazy = new Lazy<WebAPIGlobal>(() => new WebAPIGlobal());
        public static WebAPIGlobal Instance => lazy.Value;
        private WebAPIGlobal() { _ResultOfInitSink = null; }
        private TaskCompletionSource<bool> _ResultOfInitSink;
        public static TaskCompletionSource<bool> ResultOfInitSink { get => Instance._ResultOfInitSink; set => Instance._ResultOfInitSink = value; }
        private TaskCompletionSource<bool> _ResultOfHostRunSink;
        public static TaskCompletionSource<bool> ResultOfHostRunSink { get => Instance._ResultOfHostRunSink; set => Instance._ResultOfHostRunSink = value; }
    }
}
