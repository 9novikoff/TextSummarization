using Microsoft.AspNetCore.Mvc;
using TextSummarization.TextRank;

namespace TextSummarization.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
                });
        });

        
        builder.Services.AddSingleton<LuhnMethodSummarizer>();
        builder.Services.AddSingleton<TextRankSummarizer>();
        builder.Services.AddSingleton<LexRankSummarizer>();
        builder.Services.AddSingleton<LsaSummarizer>();
        
        var app = builder.Build();
        
        app.MapPost("/summary/textrank", (SummarizationRequest request, [FromServices]TextRankSummarizer summarizer) => 
            summarizer.Summarize(request.Text, request.Capacity, new UkrainianLanguagePack()));
        
        app.MapPost("/summary/lexrank", (SummarizationRequest request, [FromServices]LexRankSummarizer summarizer) => 
            summarizer.Summarize(request.Text, request.Capacity, new UkrainianLanguagePack()));
        
        app.MapPost("/summary/luhn", (SummarizationRequest request, [FromServices]LuhnMethodSummarizer summarizer) => 
            summarizer.Summarize(request.Text, request.Capacity, new UkrainianLanguagePack()));
        
        app.MapPost("/summary/lsa", (SummarizationRequest request, [FromServices]LsaSummarizer summarizer) => 
            summarizer.Summarize(request.Text, request.Capacity, new UkrainianLanguagePack()));
        
        app.UseCors();
        
        app.Run();
    }
}