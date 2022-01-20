using Kërkues_Backend.Services;
using Microsoft.ML;
using System.Text;
using WeCantSpell.Hunspell;

namespace Kërkues_Backend.Models
{
    public class Query
    {
        public string Id { get; set; }

        private string QueryString { get; set; }

        public string Words { get; set; }

        public Dictionary<string, DataTermQuery> Vector { get; set; }

        public TextTokens Tokens { get; set; }

        private int MaximumFrequency { get; set; }

        private double Norm { get; set; }

        public Query(string words)
        {
            Id = Guid.NewGuid().ToString();
            Words = words;
            QueryString = words;
            MaximumFrequency = 0;
            Vector = new Dictionary<string, DataTermQuery>();
            Tokens = new TextTokens();
            Norm = 0;
            Tokenize();
            if(Tokens.Tokens!=null)
            {
                Stemming();
                UpdateVector();
            }
        }

        private void Tokenize()
        {
            var context = new MLContext();

            var emptyData = new List<TextData>();

            var data = context.Data.LoadFromEnumerable(emptyData);

            var tokenization = context.Transforms.Text.TokenizeIntoWords("Tokens", "Text",
                separators: new char[] { '\n', ' ', ',', '.', '-' })
                .Append(context.Transforms.Text.RemoveDefaultStopWords("Tokens", "Tokens",
                Microsoft.ML.Transforms.Text.StopWordsRemovingEstimator.Language.English));

            var model = tokenization.Fit(data);

            var engine = context.Model.CreatePredictionEngine<TextData, TextTokens>(model);

            Tokens = engine.Predict(new TextData { Text = Words });
        }

        private void Stemming()
        {
            /*
            Porter2 stemmer = new Porter2();
            for (int i = 0; i < Tokens.Tokens.Length; i++)
            {
                Tokens.Tokens[i] = stemmer.stem(Tokens.Tokens[i].ToLower());
            }*/

            var stemmer = new Annytab.Stemmer.EnglishStemmer();
            for (int i = 0; i < Tokens.Tokens.Length; i++)
            {
                Tokens.Tokens[i] = stemmer.GetSteamWord(Tokens.Tokens[i].ToLower());
            }
        }

        private void UpdateVector()
        {
            foreach (var item in Tokens.Tokens)
            {
                if (!Vector.ContainsKey(item))
                {
                    Vector.Add(item, new DataTermQuery { A = 0.5, Frec = 1, IDF = 0, W = 0});
                }
                else
                    Vector[item].Frec++;
            }

            foreach (var item in Vector)
            {
                if(item.Value.Frec > MaximumFrequency)
                {
                    MaximumFrequency = item.Value.Frec;
                }
            }

            var files = Corpus.GetFiles();
            double norm = 0;
            foreach (var item1 in Vector)
            {
                var ni = 0;
                foreach (var item2 in files)
                {
                    if (item2.Vector.ContainsKey(item1.Key))
                    {
                        ni= item2.Vector[item1.Key].Ni;
                        break;
                    }
                }
                
                if (Vector.Count > 1 && ni == 0)
                {
                    Vector.Remove(item1.Key);
                }
                else
                {
                    item1.Value.IDF = Math.Log10((double)files.Count / (double)ni);
                    item1.Value.W = (item1.Value.A + ((1-item1.Value.A)*(double)item1.Value.Frec/(double)MaximumFrequency)) * item1.Value.IDF;
                    norm += Math.Pow(item1.Value.W, 2);

                    Norm += norm;
                }
                
            }

        }

        public SearchResult Ranking(bool test = false)
        {
            if (Tokens.Tokens == null)
                return new SearchResult
                {
                    SearchObjectResults = new List<SearchObjectResult>(),
                    SearchSuggestion = new List<SearchSuggestion>()
                };
            var files = Corpus.GetFiles();
            Tuple<int,double>[] sim = new Tuple<int, double>[files.Count];

            for (int i = 0; i < sim.Length; i++)
            {
                sim[i] = new Tuple<int, double>(0,0);
            }

            int index = 0;
            List<File> filesResult = new List<File>();

            foreach (var item1 in files)
            {
                foreach (var item2 in Vector)
                {
                    if(item1.Vector.ContainsKey(item2.Key))
                    {
                        sim[index] = new Tuple<int, double>(index,sim[index].Item2+item2.Value.W * item1.Vector[item2.Key].W);
                    }
                }
                sim[index] = new Tuple<int, double>(index,sim[index].Item2 / (item1.Norm * Norm));
                index++;
            }

            Array.Sort(sim, (a, b) => b.Item2.CompareTo(a.Item2));
            var objectResult = new List<SearchObjectResult>();

            if(!test)
            {
                for (int i = 0; i < sim.Length; i++)
                {
                    if (sim[i].Item2 == 0)
                        break;
                    objectResult.Add(new SearchObjectResult
                    {
                        Name = files[sim[i].Item1].Title,
                        Location = files[sim[i].Item1].Location
                    });
                }
            }
            else
            {
                for (int i = 0; i < sim.Length; i++)
                {
                    if (sim[i].Item2 == 0)
                        break;
                    objectResult.Add(new SearchObjectResult
                    {
                        Name = files[sim[i].Item1].Id.ToString()
                    });
                }
            }
            

            var res = new SearchResult
            {
                SearchObjectResults = objectResult,
                SearchSuggestion = GetSuggestions(QueryString)
            };

            return res;
        }

        private List<SearchSuggestion> GetSuggestions(string words)
        {
            var dictionary = WordList.CreateFromFiles(@"English (British).dic");
            var suggestions = dictionary.Suggest(words);
            var res = new List<SearchSuggestion>();
            foreach (var word in suggestions)
            {
                if(word!=words)
                    res.Add(new SearchSuggestion { Suggestion = word });
            }
            return res;
        }
    }
}
