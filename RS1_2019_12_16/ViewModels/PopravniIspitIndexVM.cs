using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_2019_12_16.ViewModels
{
    public class PopravniIspitIndexVM
    {
        public List<SelectListItem> filterSkolskeGod { get; set; }
        public int SkolskaGodId { get; set; }

        public List<SelectListItem> filterSkole { get; set; }
        public int SkolaId { get; set; }

        public List<SelectListItem> filterPredmeti { get; set; }
        public int PredmetId { get; set; }

    }
}
