﻿namespace Kërkues_Backend.Models
{
    public class File
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public List<string> Words { get; set; }

        public Dictionary<string, DataTerm> Vector { get; set; }

        public int MaximunFrecuency { get; set; }

        public double Norm { get; set; }

        public File(int id, string title, List<string> words, string location)
        {
            Id = id;
            Title = title;
            Words = words;
            Location = location;
            Vector = new Dictionary<string, DataTerm>();
            UpdateVectors();
            Norm = 0;
        }

        private void UpdateVectors()
        {
            var frec = 0;

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
                var temp = (double)item.Value.Frec / MaximunFrecuency;

                item.Value.F = temp;
            }
        }
    }
}
