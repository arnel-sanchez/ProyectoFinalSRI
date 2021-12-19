namespace Kërkues_Backend.Models
{
    public class File
    {
        public int Id { get; set; }

        public List<string> Title { get; set; }

        public string TitleOut { get; set; }

        public List<string> Author { get; set; }

        public string AuthorOut { get; set; }

        public List<string> Bookmarks { get; set; }

        public string BookmarksOut { get; set; }

        public List<string> Words { get; set; }

        public Dictionary<string, DataTerm> Vector { get; set; }

        public int MaximunFrecuency { get; set; }

        public double Norm { get; set; }

        public File(int id, List<string> title, List<string> author, List<string> bookmarks, List<string> words, string bout, string aout, string tout)
        {
            Id = id;
            Title = title;
            Author = author;
            Bookmarks = bookmarks;
            Words = words;
            Vector = new Dictionary<string, DataTerm>();
            UpdateTerms();
            UpdateVectors();
            Norm = 0;
            AuthorOut = aout;
            BookmarksOut = bout;
            TitleOut = tout;
        }

        private void UpdateTerms()
        {
            
        }

        private void UpdateVectors()
        {
            int frec = 0;
            foreach (var item in Title)
            {
                if (Vector.ContainsKey(item))
                {
                    Vector[item].Frec++;
                }
                else
                {
                    Vector.Add(item, new DataTerm { Frec = 1, F = 0, IDF = 0, W = 0, Ni = 0 }) ;
                }
            }

            foreach (var item in Author)
            {
                if (Vector.ContainsKey(item))
                {
                    Vector[item].Frec++;
                }
                else
                {
                    Vector.Add(item, new DataTerm { Frec = 1, F = 0, IDF = 0, W = 0 });
                }
            }

            foreach (var item in Words)
            {
                if (Vector.ContainsKey(item))
                {
                    Vector[item].Frec++;
                }
                else
                {
                    Vector.Add(item, new DataTerm { Frec = 1, F = 0, IDF = 0, W = 0 });
                }
            }

            foreach (var item in Bookmarks)
            {
                if (Vector.ContainsKey(item))
                {
                    Vector[item].Frec++;
                }
                else
                {
                    Vector.Add(item, new DataTerm { Frec = 1, F = 0, IDF = 0, W = 0 });
                }
            }

            foreach (var item in Vector)
            {
                if (item.Value.Frec > frec)
                {
                    frec = item.Value.Frec;
                }
            }
            MaximunFrecuency = frec;

            foreach (var item in Vector)
            {
                double temp = (double)item.Value.Frec / ((double)MaximunFrecuency);

                item.Value.F = temp;
            }
        }
    }
}
