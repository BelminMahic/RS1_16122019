using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_2019_12_16.ViewModels
{
    public class PopravniIspitDodajVM
    {
        public int PredmetId { get; set; }
        public int SkolaId { get; set; }
        public int SkolskaGodId { get; set; }
        public string PredmetNaziv { get; set; }
        public string SkolskaGodNaziv { get; set; }
        public string SkolaNaziv { get; set; }
        public DateTime Datum { get; set; }
        public int ClanKomisije1Id { get; set; }
        public int ClanKomisije2Id { get; set; }
        public int ClanKomisije3Id { get; set; }

        public List<SelectListItem> filterNastavnici { get; set; }
        public int NastavnikId { get; set; }

    }
}
