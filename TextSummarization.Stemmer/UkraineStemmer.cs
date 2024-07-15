using System.Text.RegularExpressions;

namespace TextSummarization.Stemmer;

public class UkraineStemmer : IStemmer
{
    private readonly string vowel = "аеиоуюяіїє";
    private readonly string perfectiveground = @"(ив|ивши|ившись|ыв|ывши|ывшись((?<=[ая])(в|вши|вшись)))$";
    private readonly string reflexive = @"(с[яьи])$";
    private readonly string adjective = @"(ими|ій|ий|а|е|ова|ове|ів|є|їй|єє|еє|я|ім|ем|им|ім|их|іх|ою|йми|іми|у|ю|ого|ому|ої)$";
    private readonly string participle = @"(ий|ого|ому|им|ім|а|ій|у|ою|ій|і|их|йми|их)$";
    private readonly string verb = @"(сь|ся|ив|ать|ять|у|ю|ав|али|учи|ячи|вши|ши|е|ме|ати|яти|є)$";
    private readonly string noun = @"(а|ев|ов|е|ями|ами|еи|и|ей|ой|ий|й|иям|ям|ием|ем|ам|ом|о|у|ах|иях|ях|ы|ь|ию|ью|ю|ия|ья|я|і|ові|ї|ею|єю|ою|є|еві|ем|єм|ів|їв|ю)$";
    private readonly string rvre = @"[аеиоуюяіїє]";
    private readonly string derivational = @"[^аеиоуюяіїє][аеиоуюяіїє]+[^аеиоуюяіїє]+[аеиоуюяіїє].*(?<=о)сть?$";
    private string RV;

    private string UkStemmerSearchPreprocess(string word)
    {
        word = word.ToLower();
        word = word.Replace("'", "");
        word = word.Replace("ё", "е");
        word = word.Replace("ъ", "ї");
        return word;
    }

    private bool S(ref string st, string reg, string to)
    {
        var original = st;
        st = Regex.Replace(st, reg, to);
        return original != st;
    }

    public string Stem(string word)
    {
        word = UkStemmerSearchPreprocess(word);
        string stemma;
        if (!Regex.IsMatch(word, "[аеиоуюяіїє]"))
        {
            stemma = word;
        }
        else
        {
            var p = Regex.Match(word, rvre);
            var start = word.Substring(0, p.Index + p.Length);
            RV = word.Substring(p.Index + p.Length);
            
            if (!S(ref RV, perfectiveground, ""))
            {
                S(ref RV, reflexive, "");
                if (S(ref RV, adjective, ""))
                {
                    S(ref RV, participle, "");
                }
                else
                {
                    if (!S(ref RV, verb, ""))
                    {
                        S(ref RV, noun, "");
                    }
                }
            }
            
            S(ref RV, "и$", "");
            
            if (Regex.IsMatch(RV, derivational))
            {
                S(ref RV, "ость$", "");
            }
            
            if (S(ref RV, "ь$", ""))
            {
                S(ref RV, "ейше?$", "");
                S(ref RV, "нн$", "н");
            }

            stemma = start + RV;
        }
        return stemma;
    }
}