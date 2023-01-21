using AngleSharp;
using Core.Interfaces;
using TagFinder;

namespace Core
{
    public class TagParser : ITagParser
    {
        public Action OnStarted { get; set; }
        public Action<float> OnProgressUpdated { get; set; }
        public Action OnFinished { get; set; }

        private IConfiguration config;
        private int urlCount;
        private int parsedUrlCount;

        public TagParser()
        {
            config = Configuration.Default.WithDefaultLoader();
        }

        public async Task ParseAsync(UrlResult[] urls, CancellationToken token)
        {
            OnStarted?.Invoke();

            urlCount = urls.Length;

            List<Task> tasks = new List<Task>();
            foreach (var url in urls)
            {
                tasks.Add( ParseUrl(url, token) );
            }
            await Task.WhenAll(tasks);

            OnFinished?.Invoke();
        }

        private async Task ParseUrl(UrlResult result, CancellationToken token)
        {
            try
            {
                using var context = BrowsingContext.New(config);
                using var doc = await context.OpenAsync(result.Url, token);
                if (doc != null)
                { 
                    var tags = doc.GetElementsByTagName("a");
                    result.Count = tags.Length;
                }
                ProgressTick();
                
            }
            catch (Exception e)
            {
                
            }
            
        }

        private void ProgressTick()
        {
            parsedUrlCount++;
            OnProgressUpdated?.Invoke((float)parsedUrlCount/urlCount);
        }
        
    }
}
