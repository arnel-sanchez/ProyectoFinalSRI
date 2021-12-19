namespace Kërkues_Backend.Models
{
    public class DataTerm
    {
        public int Frec { get; set; }

        public double F { get; set; }

        public double IDF { get; set; }

        public double W { get; set; }

        public int Ni { get; set; }
    }  

    public class DataTermQuery
    {
        public int Frec { get; set; }

        public double W { get; set; }

        public double IDF { get; set; }

        public double A { get; set; }
    }
}
