using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TextSummarization.TextRank;

public partial class LsaSummarizer : ISummarizer
{
    [Benchmark(Description = "LSA")]
    public string Summarize(string text, double capacity, ILanguagePack languagePack)
    {
        var sentences = NewSentenceRegex().Split(text).Select(s => s.Split(' ').Select(w => w.ToLower()).Where(w => !languagePack.StopWords.Contains(w)).Select(w => new string(languagePack.Stemmer.Stem(w).Where(c => !char.IsPunctuation(c)).ToArray())).Where(w =>!string.IsNullOrWhiteSpace(w)).ToArray()).Where(l => l.Length != 0).ToArray();

        var words = sentences.SelectMany(s => s).Distinct().ToList();

        var A = new double[words.Count][];

        for (int i = 0; i < words.Count; i++)
        {
            A[i] = new double[sentences.Length];
            
            for (int j = 0; j < sentences.Length; j++)
            {
                A[i][j] = sentences[j].Count(w => w.Equals(words[i]));
            }
        }
        
        var a = SparseMatrix.OfRowArrays(A);
        var res = a.Svd();

        var indices = res.VT.EnumerateColumns().Select(v => v.Select((x, i) => Math.Pow(x, 2) + Math.Pow(res.S[i], 2)).Sum()).Select((v, i) => (v, i)).OrderByDescending(t => t.v).Take((int)Math.Floor(sentences.Length*capacity)).Select(t => t.i).OrderBy(i => i);
        
        return string.Join('\n', indices.Select(index => NewSentenceRegex().Split(text)[index]));
    }

    [GeneratedRegex(@"(?<=[\.!\?])\s+")]
    private static partial Regex NewSentenceRegex();
    
    
}