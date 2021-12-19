using Kërkues_Backend.Models;
using Microsoft.ML;

namespace Kërkues_Backend.Services
{
    public static class Corpus
    {
        private static List<Models.File> _files { get; set; }

        public static void CorpusLoad(string filesPath, bool test = false)
        {
            _files = new List<Models.File>();

            if (test)
            {
                StreamReader streamReader = new StreamReader(filesPath);
                var text = streamReader.ReadToEnd();
                ParseTest(text);

                foreach (var item1 in _files)
                {
                    foreach (var item3 in item1.Vector)
                    {
                        foreach (var item2 in _files)
                        {
                            if (item2.Vector.ContainsKey(item3.Key))
                            {
                                item3.Value.Ni++;
                            }
                        }
                    }
                }
                foreach (var item1 in _files)
                {
                    double norm = 0;
                    foreach (var item2 in item1.Vector)
                    {
                        item2.Value.IDF = Math.Log10((double)_files.Count / (double)item2.Value.Ni);
                        item2.Value.W = item2.Value.F * item2.Value.IDF;
                        norm += Math.Pow(item2.Value.W, 2);
                    }
                    item1.Norm = Math.Sqrt(norm);
                }
                streamReader.Close();
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(filesPath);
                var files = directoryInfo.GetFiles();
                foreach (var file in files)
                {
                    
                }
            }
        }

        public static List<Models.File> GetFiles()
        {
            return _files;
        }

        private static void ParseTest(string text)
        {
            int state = 0;

            var aout = "";
            var bout = "";
            var tout = "";

            var tokens = Tokenizer(text);

            tokens = Stemming(tokens);

            List<string> title = new List<string>();
            List<string> author = new List<string>();
            List<string> bookmarks = new List<string>();
            List<string> words = new List<string>();
            int id = 0;

            foreach (var token in tokens.Tokens)
            {
                if(state==0)
                {
                    if (token == ".t")
                    {
                        state++;
                    }
                    else if(token != ".i")
                        id = int.Parse(token);
                }
                else if (state == 1)
                {
                    if (token == ".a")
                    {
                        state++;
                    }
                    else if (token != ".t")
                        title.Add(token);
                }
                else if(state == 2)
                {
                    if (token == ".b")
                    {
                        state++;
                    }
                    else if (token != ".a")
                        author.Add(token);
                }
                else if (state == 3)
                {
                    if (token == ".w")
                    {
                        state++;
                    }
                    else if (token != ".b")
                        bookmarks.Add(token);
                }
                else if (state == 4)
                {
                    if (token == ".i")
                    {
                        state = 0;
                        _files.Add(new Models.File(id, title, author, bookmarks, words, null, null, null));
                        id = 0;
                        title = new List<string>();
                        author = new List<string>();
                        bookmarks = new List<string>();
                        words = new List<string>();
                    }
                    else if (token != ".w")
                        words.Add(token);
                }
            }
        }

        private static TextTokens Tokenizer(string text)
        {
            var context = new MLContext();

            var emptyData = new List<TextData>();

            var data = context.Data.LoadFromEnumerable(emptyData);

            var tokenization = context.Transforms.Text.TokenizeIntoWords("Tokens", "Text",
                separators: new char[] { '\n', ' ', ',' })
                .Append(context.Transforms.Text.RemoveDefaultStopWords("Tokens", "Tokens",
                Microsoft.ML.Transforms.Text.StopWordsRemovingEstimator.Language.English));

            var model = tokenization.Fit(data);

            var engine = context.Model.CreatePredictionEngine<TextData, TextTokens>(model);

            var tokens = engine.Predict(new TextData { Text = text });

            tokens.Tokens = tokens.Tokens.Where(x=>x!=".").Append(".I").ToArray();

            return tokens;
        }

        private static TextTokens Stemming(TextTokens text)
        {
            /*
            Porter2 stemmer = new Porter2();
            for (int i = 0; i < text.Tokens.Length; i++)
            {
                text.Tokens[i] = stemmer.stem(text.Tokens[i].ToLower());
            }*/

            var stemmer = new Annytab.Stemmer.EnglishStemmer();
            for (int i = 0; i < text.Tokens.Length; i++)
            {
                text.Tokens[i] = stemmer.GetSteamWord(text.Tokens[i].ToLower());
            }

            return text;
        }
    }
}
