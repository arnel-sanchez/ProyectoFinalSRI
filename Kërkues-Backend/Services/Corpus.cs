using Kërkues_Backend.Models;
using Microsoft.ML;

namespace Kërkues_Backend.Services
{
    public static class Corpus
    {
        public static bool Test { get; set; }

        private static List<Models.File> Files { get; set; } = null!;

        public static void CorpusLoad(string filesPath, bool test = false)
        {
            Files = new List<Models.File>();

            if (test)
            {
                var streamReader = new StreamReader(filesPath);
                var text = streamReader.ReadToEnd();
                ParseTest(text, test);

                foreach (var item1 in Files)
                {
                    foreach (var item3 in item1.Vector)
                    {
                        foreach (var item2 in Files)
                        {
                            if (item2.Vector.ContainsKey(item3.Key))
                            {
                                item3.Value.Ni++;
                            }
                        }
                    }
                }
                foreach (var item1 in Files)
                {
                    double norm = 0;
                    foreach (var item2 in item1.Vector)
                    {
                        item2.Value.IDF = Math.Log10((double)Files.Count / item2.Value.Ni);
                        item2.Value.W = item2.Value.F * item2.Value.IDF;
                        norm += Math.Pow(item2.Value.W, 2);
                    }
                    item1.Norm = Math.Sqrt(norm);
                }
                streamReader.Close();
            }
            else
            {
                var directoryInfo = new DirectoryInfo(filesPath);
                var files = directoryInfo.GetFiles();
                var i = 1;
                foreach (var file in files)
                {
                    var streamReader = new StreamReader(filesPath+"/"+file.Name);
                    var text = streamReader.ReadToEnd();
                    Parse(i,file.Name.Remove(file.Name.Length - file.Extension.Length), file.FullName, text, test);
                    i++;
                    streamReader.Close();
                }

                foreach (var item1 in Files)
                {
                    foreach (var item3 in item1.Vector)
                    {
                        foreach (var item2 in Files)
                        {
                            if (item2.Vector.ContainsKey(item3.Key))
                            {
                                item3.Value.Ni++;
                            }
                        }
                    }
                }
                foreach (var item1 in Files)
                {
                    double norm = 0;
                    foreach (var item2 in item1.Vector)
                    {
                        item2.Value.IDF = Math.Log10((double)Files.Count / item2.Value.Ni);
                        item2.Value.W = item2.Value.F * item2.Value.IDF;
                        norm += Math.Pow(item2.Value.W, 2);
                    }
                    item1.Norm = Math.Sqrt(norm);
                }

            }
        }

        public static List<Models.File> GetFiles()
        {
            return Files;
        }

        private static void ParseTest(string text, bool test)
        {
            var state = 0;

            var tokens = Tokenizer(text, test);

            tokens = Stemming(tokens);

            var words = new List<string>();
            var id = 0;

            foreach (var token in tokens.Tokens)
            {
                switch (state)
                {
                    case 0 when token == ".t":
                        state++;
                        break;
                    case 0 when token == ".w":
                        state = 4;
                        break;
                    case 0:
                    {
                        if (token != ".i")
                        {
                            id = int.Parse(token);
                        }

                        break;
                    }
                    case 1 when token == ".a":
                        state++;
                        break;
                    case 1:
                    {
                        if (token != ".t")
                        {
                            words.Add(token);
                        }

                        break;
                    }
                    case 2 when token == ".b":
                        state++;
                        break;
                    case 2 when token == ".w":
                        state = 4;
                        break;
                    case 2:
                    {
                        if (token != ".a")
                        {
                            words.Add(token);
                        }

                        break;
                    }
                    case 3 when token == ".w":
                        state++;
                        break;
                    case 3:
                    {
                        if (token != ".b")
                        {
                            words.Add(token);
                        }

                        break;
                    }
                    case 4 when token == ".i":
                        state = 0;
                        Files.Add(new Models.File(id, "", words, ""));
                        id = 0;
                        words = new List<string>();
                        break;
                    case 4:
                    {
                        if (token != ".w")
                        {
                            words.Add(token);
                        }

                        break;
                    }
                }
            }
        }

        private static void Parse(int id, string title, string location, string text, bool test)
        {
            var tokens = Tokenizer(title + " " + text, test);

            tokens = Stemming(tokens);

            Files.Add(new Models.File(id, title, tokens.Tokens.ToList(), location));
        }

        private static TextTokens Tokenizer(string text, bool test)
        {
            var context = new MLContext();

            var data = context.Data.LoadFromEnumerable(new List<TextData>());

            var tokenization = context.Transforms.Text.TokenizeIntoWords("Tokens", "Text",
                new[] { '\n', ' ', ',', '\r', ';', '?', '(', ')' })
                .Append(context.Transforms.Text.RemoveDefaultStopWords("Tokens", "Tokens"));

            var model = tokenization.Fit(data);

            var engine = context.Model.CreatePredictionEngine<TextData, TextTokens>(model);

            var tokens = engine.Predict(new TextData { Text = text });

            tokens.Tokens =
                test
                    ? tokens.Tokens.Where(x => x != ".").Append(".I").ToArray()
                    : tokens.Tokens.Where(x => x != ".").ToArray();

            return tokens;
        }

        private static TextTokens Stemming(TextTokens text)
        {
            
            var stemmer = new Porter2();
            for (var i = 0; i < text.Tokens.Length; i++)
            {
                text.Tokens[i] = stemmer.stem(text.Tokens[i].ToLower());
            }
			
            /*
            var stemmer = new Annytab.Stemmer.EnglishStemmer();
            for (int i = 0; i < text.Tokens.Length; i++)
            {
                text.Tokens[i] = stemmer.GetSteamWord(text.Tokens[i].ToLower());
            }
            */

            return text;
        }
    }
}
