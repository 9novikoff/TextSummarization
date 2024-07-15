using System.Text.RegularExpressions;
using TextSummarization.Stemmer;
using TextSummarization.TextRank;

namespace TextSummarization.Metrics;

public static class Metric
{
    public static double Rouge1(string reference, string candidate, ILanguagePack languagePack)
    {
        var referencePresentation = GetWordsInSentences(reference, languagePack).SelectMany(s => s).ToList();
        var candidatePresentation = GetWordsInSentences(candidate, languagePack).SelectMany(s => s).ToList();

        var enters = referencePresentation.Intersect(candidatePresentation).Select(w =>
                Math.Min(referencePresentation.Count(r => r.Equals(w)), candidatePresentation.Count(r => r.Equals(w))))
            .Sum();
        
        var recall = (double) enters / referencePresentation.Count;
        var precision = (double)enters / candidatePresentation.Count;

        return 2 * (precision * recall) / (precision + recall);
    }
    
    
    public static double Rouge2(string reference, string candidate, ILanguagePack languagePack)
    {
        var referencePresentation = GetWordsInSentences(reference, languagePack).SelectMany(s => s).ToList();
        var candidatePresentation = GetWordsInSentences(candidate, languagePack).SelectMany(s => s).ToList();

        var referencePairs = referencePresentation.Zip(referencePresentation.Skip(1), Tuple.Create).ToList();
        var candidatePairs = candidatePresentation.Zip(candidatePresentation.Skip(1), Tuple.Create).ToList();

        var enters = referencePairs.Intersect(candidatePairs).Count();
        
        var recall = (double) enters / referencePairs.Count;
        var precision = (double) enters / candidatePairs.Count;

        return 2 * (precision * recall) / (precision + recall);
    }
    
    public static double RougeL(string reference, string candidate, ILanguagePack languagePack)
    {
        var referencePresentation = GetWordsInSentences(reference, languagePack).SelectMany(s => s).ToList();
        var candidatePresentation = GetWordsInSentences(candidate, languagePack).SelectMany(s => s).ToList();

        var common = referencePresentation.Intersect(candidatePresentation);

        var referenceCommon = referencePresentation.Where(w => common.Contains(w));
        var candidateCommon = candidatePresentation.Where(w => common.Contains(w));

        var length = GetLongestCommonSubsequenceLength(referenceCommon, candidateCommon);
        
        var recall = (double) length / referencePresentation.Count;
        var precision = (double) length / candidatePresentation.Count;

        return 2 * (precision * recall) / (precision + recall);
    }

    private static string[][] GetWordsInSentences(string text, ILanguagePack languagePack)
    {
        return Regex.Split(text, @"(?<=[\.!\?])\s+").Select(s => s.Split(' ').Select(w => w.ToLower()).Where(w => !languagePack.StopWords.Contains(w)).Select(w => new string(languagePack.Stemmer.Stem(w).Where(c => !char.IsPunctuation(c)).ToArray())).Where(w =>!string.IsNullOrWhiteSpace(w)).ToArray()).Where(l => l.Length != 0).ToArray();
    }
    
    private static int GetLongestCommonSubsequenceLength(IEnumerable<string> firstSequence, IEnumerable<string> secondSequence)
    {
        var firstList = firstSequence.ToList();
        var secondList = secondSequence.ToList();
        var length = Math.Min(firstList.Count, secondList.Count);
        while (length != 0)
        {
            var firstSub = firstList.GetSubsequences(length).Select(s => s.ToList()).ToList();
            var secondSub = secondList.GetSubsequences(length).Select(s => s.ToList()).ToList();

            foreach (var first in firstSub)
            {
                foreach (var second in secondSub)
                {
                    bool isEqual = true;
                    for (int i = 0; i < length; i++)
                    {
                        if (!first[i].Equals(second[i]))
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    if (isEqual)
                        return length;
                }
            }

            length--;
        }

        return length;

    }

    private static IEnumerable<IEnumerable<T>> GetSubsequences<T>(this IEnumerable<T> sequence, int length)
    {
        var list = sequence.ToList();
        for (int i = 0; i <= list.Count - length; i++)
        {
            yield return list.GetRange(i, length);
        }
    }
    
}