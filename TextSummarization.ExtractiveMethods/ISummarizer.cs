namespace TextSummarization.TextRank;

public interface ISummarizer
{
    public string Summarize(string text, double capacity, ILanguagePack languagePack);
}