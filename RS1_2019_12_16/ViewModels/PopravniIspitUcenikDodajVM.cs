using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_2019_12_16.ViewModels
{
    public class PopravniIspitUcenikDodajVM
    {
        public int PopravniIspitId { get; set; }
        public int OdjeljenjeStavkaId { get; set; }
        public List<SelectListItem> ucenici { get; set; }
    }
}
