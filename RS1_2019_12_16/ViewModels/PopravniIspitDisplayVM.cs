using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_2019_12_16.ViewModels
{
    public class PopravniIspitDisplayVM
    {
        public int PredmetId { get; set; }
        public int SkolaId { get; set; }
        public int SkolskaGodId { get; set; }
        public string PredmetNaziv { get; set; }
        public string SkolskaGodNaziv { get; set; }
        public string SkolaNaziv { get; set; }

        public List<Rows> rows { get; set; }

        public class Rows
        {
            public int PopravniIspitId { get; set; }
            public string Datum { get; set; }
            public string ClanKomisije1 { get; set; }
            public int BrojUcenikaNaPopravnom { get; set; }
            public int BrojPolozenihNaPopravnom { get; set; }

        }
    }
}
