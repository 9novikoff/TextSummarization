using System.Text.RegularExpressions;
using TextSummarization.Stemmer;

namespace TextSummarization.TextRank;

public partial class LexRankSummarizer : ISummarizer
{
    private const string IdfPath = "C:\\Users\\Novikov\\RiderProjects\\TextSummarization\\TextSummarization.ConsoleApp\\idf.txt";
    private const string TrainPath = "C:\\Users\\Novikov\\RiderProjects\\TextSummarization\\TextSummarization.ConsoleApp\\train.txt";
    public const int Size = 50000;
    public string Summarize(string text, double capacity, ILanguagePack languagePack)
    {
        var sentences = NewSentenceRegex().Split(text).Select(s => s.Split(' ').Select(w => w.ToLower()).Where(w => !languagePack.StopWords.Contains(w)).Select(w => new string(languagePack.Stemmer.Stem(w).Where(c => !char.IsPunctuation(c)).ToArray())).Where(w =>!string.IsNullOrWhiteSpace(w)).ToArray()).Where(l => l.Length != 0).ToArray();

        var n = sentences.GetLength(0);
        
        var adjacencyMatrix = new double[n, n];
        
        var idfs = File.ReadAllLines(IdfPath).Select(r =>
        {
            var pair = r.Split(":");
            return (pair[0], double.Parse(pair[1]));
        }).ToDictionary();

        var frequency = new Dictionary<string, int[]>();

        var words = sentences.SelectMany(s => s).Distinct();

        foreach (var word in words)
        {
            var tf = sentences.Select(s => s.Count(w => w.Equals(word))).ToArray();

            var enters = tf.Count(s => s != 0);

            frequency[word] = tf;
        }
        
        for (int i = 0; i < n; i++)
        {
            for (int j = i+1; j < n; j++)
            {
                var numeratorSum = 0.0;
                var iSum = 0.0;
                var jSum = 0.0;
                
                foreach (var iWord in sentences[i].Distinct())
                {
                    var idf = GetIdf(idfs, iWord);
                    iSum += Math.Pow(frequency[iWord][i] * idf, 2);
                }
                
                foreach (var jWord in sentences[j].Distinct())
                {
                    var idf = GetIdf(idfs, jWord);
                    jSum += Math.Pow(frequency[jWord][j] * idf, 2);
                }

                foreach (var word in sentences[i].Concat(sentences[j]).Distinct())
                {
                    var idf = GetIdf(idfs, word);
                    numeratorSum += frequency[word][i] * frequency[word][j] * idf *
                                    idf;
                }
                
                var similarity = numeratorSum / Math.Sqrt(iSum) / Math.Sqrt(jSum);
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

    private double GetIdf(Dictionary<string, double> idfs, string word)
    {
        return idfs.TryGetValue(word, out var idf) ? idf : Math.Log(Size);
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
    
    private void TrainIdf(string path, ILanguagePack languagePack)
    {
        var dictionary = new Dictionary<string, double>();
        
        var lines = File.ReadLines(path);
        var count = 0;
        
        foreach(var line in lines)
        {
            var words = Regex.Split(line, @"(?<=[\.!\?])\s+").Select(w => w.ToLower()).Select(s => s.Split(' ')).Select(s => s.Select(w => w)
                .Where(w => !languagePack.StopWords.Contains(w))
                .Select(w =>
                    new string(languagePack.Stemmer.Stem(w).Where(c => !char.IsPunctuation(c)).ToArray())).Distinct().Where(w => !string.IsNullOrWhiteSpace(w) && !string.IsNullOrEmpty(w)).ToList()).Where(s => s.Any()).ToList();
        
            foreach (var sentence in words)
            {
                foreach (var word in sentence)
                {
                    if(dictionary.ContainsKey(word))
                        dictionary[word]++;
                    else
                    {
                        dictionary[word] = 1;
                    }
                }
            }
        
            count += words.Count;
        
            if (count == 50000)
                break;
        }

        var keys = dictionary.Keys.ToList();
        keys.Sort();
        
        foreach (var key in keys)
        {
            File.AppendAllText("idf.txt", key + ":" + Math.Log(count / dictionary[key]) + "\n");
        }
    }

    [GeneratedRegex(@"(?<=[\.!\?])\s+")]
    private static partial Regex NewSentenceRegex();
}