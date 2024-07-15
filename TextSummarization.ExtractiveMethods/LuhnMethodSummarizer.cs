using System.Text.RegularExpressions;
using TextSummarization.Stemmer;
using static System.Text.RegularExpressions.Regex;

namespace TextSummarization.TextRank;

public partial class LuhnMethodSummarizer : ISummarizer
{
    private const int AppearancesToBeSignificant = 2;
    private const int NonSignificantInARowMax = 4;
    
    public string Summarize(string text, double capacity, ILanguagePack languagePack)
    {
        var significantWords = NonWordRegex().Replace(text, " ").Split(' ').Where(w => w != "" && w != " ").Select(w => w.ToLower()).Where(w => !languagePack.StopWords.Contains(w)).GroupBy(w => w).Where(g => g.Count() >= AppearancesToBeSignificant).Select(g => g.Key).Select(w => languagePack.Stemmer.Stem(w)).ToList();

        var sentences = NewSentenceRegex().Split(text);
        var sentenceScores = new List<double>();
        
        foreach (var sentence in sentences)
        {
            double currentScore = 0;
            var nonSignificantWordsInARow = 0;
            double currentSegmentScore = 0;
            var segmentLength = 0;
            
            foreach (var word in NonWordRegex().Replace(sentence, " ").Split(' ').Where(w => w != "" && w != " ").Select(w => languagePack.Stemmer.Stem(w)))
            {
                if (!significantWords.Contains(word))
                {
                    nonSignificantWordsInARow++;
                    segmentLength++;
                    if (nonSignificantWordsInARow == NonSignificantInARowMax)
                    {
                        var newScore = currentSegmentScore * currentSegmentScore / segmentLength;
                        if (currentScore < newScore)
                            currentScore = newScore;
                        currentSegmentScore = 0;
                        nonSignificantWordsInARow = 0;
                        segmentLength = 0;
                    }

                }
                else
                {
                    nonSignificantWordsInARow = 0;
                    currentSegmentScore++;
                    segmentLength++;
                }
            }
            
            var finalScore = currentSegmentScore * currentSegmentScore / segmentLength;
            if (currentScore < finalScore)
                currentScore = finalScore;
            
            sentenceScores.Add(currentScore);
        }

        return string.Join("\n", sentenceScores.Select((v, i) => (v, i)).OrderByDescending(s => s.v).Take((int)Math.Floor(sentences.Length*capacity)).Select(v => sentences[v.i]));
    }
    
    [GeneratedRegex(@"(?<=[\.!\?])\s+")]
    private static partial Regex NewSentenceRegex();
    [GeneratedRegex("\\W")]
    private static partial Regex NonWordRegex();
}