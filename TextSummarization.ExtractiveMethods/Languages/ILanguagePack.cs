using TextSummarization.Stemmer;

namespace TextSummarization.TextRank;

public interface ILanguagePack
{
    public List<string> StopWords { get; }
    public IStemmer Stemmer { get; }
}