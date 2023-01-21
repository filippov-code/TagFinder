namespace TagFinder
{
    public class UrlResult
    {
        public string Url { get; set; }
        public int Count { get; set; }

        public UrlResult(string url, int count)
        {
            Url = url;
            Count = count;
        }


    }
}
