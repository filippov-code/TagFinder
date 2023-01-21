using Core.Interfaces;

namespace Core
{
    public class FileParser : IFileParser
    {
        public string[] Parse(string rawText)
        {
            return rawText.Replace(Environment.NewLine, "").Split(",").Select(x => x.Trim()).ToArray();
        }
    }
}
