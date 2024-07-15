using System.Diagnostics;
using TextSummarization.TextRank;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Running;
using TextSummarization.Metrics;

namespace TextSummarization.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        var ukrainianLanguagePack = new UkrainianLanguagePack();
        
        var text = File.ReadAllText("text.txt");
        var summarizer = new LuhnMethodSummarizer();
        var timer = new Stopwatch();
        timer.Start();
        var res = summarizer.Summarize(text, 0.5, ukrainianLanguagePack);
        timer.Stop();
        Console.WriteLine($"Excecution time: {timer.ElapsedMilliseconds} ms");
        Console.WriteLine($"Rouge1: {Metric.Rouge1(text, res, ukrainianLanguagePack)}");
        Console.WriteLine($"Rouge2: {Metric.Rouge2(text, res, ukrainianLanguagePack)}");
        Console.WriteLine($"RougeL: {Metric.RougeL(text, res, ukrainianLanguagePack)}");
        Console.WriteLine(res);

        //BenchmarkRunner.Run<SummarizationBenchmark>();

    }
    
}