using System.Text.RegularExpressions;
using TextSummarization.Stemmer;
using QuickGraph;

namespace TextSummarization.TextRank;

public partial class TextRankSummarizer : ISummarizer
{
    public string Summarize(string text, double capacity, ILanguagePack languagePack)
    {
        var sentences = NewSentenceRegex().Split(text).Select(s => s.Split(' ').Select(w => w.ToLower()).Where(w => !languagePack.StopWords.Contains(w)).Select(w => new string(languagePack.Stemmer.Stem(w).Where(c => !char.IsPunctuation(c)).ToArray())).Distinct().Where(w =>!string.IsNullOrWhiteSpace(w)).ToArray()).Where(l => l.Length != 0).ToArray();

        var n = sentences.GetLength(0);

        var adjacencyMatrix = new double[n, n];
        
        for (int i = 0; i < n; i++)
        {
            for (int j = i+1; j < n; j++)
            {
                var similarity = sentences[i].Intersect(sentences[j]).Count() / (Math.Log(sentences[i].Length) + Math.Log(sentences[j].Length));
                if (similarity == 0) continue;
                adjacencyMatrix[i, j] = similarity;
                adjacencyMatrix[j, i] = similarity;
            }
        }
        
        var pageRanks = ComputePageRank(adjacencyMatrix);
        var indices = pageRanks.Select((value, index) => new { Value = value, Index = index })
            .OrderByDescending(x => x.Value)
            .Take((int)Math.Floor(n*capacity))
            .Select(x => x.Index)
            .OrderBy(x => x);
        return string.Join('\n', indices.Select(index => NewSentenceRegex().Split(text)[index]));
    }
    
    static double[] ComputePageRank(double[,] adjacencyMatrix, double d = 0.85, double tolerance = 1e-10)
    {
        int n = adjacencyMatrix.GetLength(0);
        double[] pagerank = new double[n];
        double[] oldPagerank = new double[n];
        
        for (int i = 0; i < n; i++)
        {
            pagerank[i] = 1.0 / n;
            oldPagerank[i] = 0;
        }
        
        while (Diff(pagerank, oldPagerank) > tolerance)
        {
            Array.Copy(pagerank, oldPagerank, n);

            for (int i = 0; i < n; i++)
            {
                double sum = 0;

                for (int j = 0; j < n; j++)
                {
                    var innerSum = 0.0;
                    
                    for (int k = 0; k < n; k++)
                    {
                        innerSum += adjacencyMatrix[j, k];
                    }

                    if(innerSum != 0)
                        sum += adjacencyMatrix[i, j] * oldPagerank[j] / innerSum;
                }

                pagerank[i] = (1 - d) + d * sum;
            }
        }

        return pagerank;
    }

    static double Diff(double[] arr1, double[] arr2)
    {
        double sum = 0;

        for (int i = 0; i < arr1.Length; i++)
        {
            sum += Math.Abs(arr1[i] - arr2[i]);
        }

        return sum;
    }

    [GeneratedRegex(@"(?<=[\.!\?])\s+")]
    private static partial Regex NewSentenceRegex();
}