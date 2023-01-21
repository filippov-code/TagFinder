using TagFinder;

namespace Core.Interfaces
{
    internal interface ITagParser
    {
        public Action OnStarted { get; set; }
        public Action<float> OnProgressUpdated { get; set; }
        public Action OnFinished { get; set; }

        public Task ParseAsync(UrlResult[] urls, CancellationToken token);
    }
}
