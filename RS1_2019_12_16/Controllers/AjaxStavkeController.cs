using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2019_12_16.EF;
using RS1_2019_12_16.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_2019_12_16.Controllers
{
    public class AjaxStavkeController : Controller
    {
        private readonly MojContext db;

        public AjaxStavkeController(MojContext db)
        {
            this.db = db;
        }

        public IActionResult Index(int id)
        {
            var ispit = db.PopravniIspit.Find(id);

            var index = new AjaxStavkeIndexVM
            {
                PopravniIspitId=id,
                SkolaId=ispit.SkolaId,
                PredmetId=ispit.PredmetId,
                SkolskaGodId=ispit.SkolskaGodinaId,
                rows=db.PopravniIspitStavka.Where(x=>x.PopravniIspitId==id)
                    .Select(x=> new AjaxStavkeIndexVM.Rows { 
                        PopravniIspitStavkaId=x.Id,
                        Odjeljenje=x.OdjeljenjeStavka.Odjeljenje.Oznaka,
                        BrojUDnevniku=x.OdjeljenjeStavka.BrojUDnevniku,
                        UcenikIme=x.OdjeljenjeStavka.Ucenik.ImePrezime,
                        Bodovi=x.Bodovi,
                        IsPristupio=x.IsPristupio                   
                    
                    }).ToList()



            };
            return PartialView(index);
        }

        public IActionResult Edit(int id)
        {

            var popravniIspitStavka = db.PopravniIspitStavka.Where(x => x.Id == id)
                                    .Include(x => x.OdjeljenjeStavka)
                                    .Include(x=>x.OdjeljenjeStavka.Ucenik)
                                    .FirstOrDefault();

            var uredi = new AjaxStavkeUrediVM
            {
                PopravniIspitStavkaId = id,
                Ucenik = popravniIspitStavka.OdjeljenjeStavka.Ucenik.ImePrezime,
                Bodovi = popravniIspitStavka.Bodovi
            };


            return PartialView(uredi);
        }

        public IActionResult Save(AjaxStavkeUrediVM model)
        {
            var popravniIspitStavka = db.PopravniIspitStavka.Find(model.PopravniIspitStavkaId);
            popravniIspitStavka.Bodovi = model.Bodovi < 0 ? 0 : model.Bodovi > 100 ? 100 : model.Bodovi;
            db.SaveChanges();
            return RedirectToAction("Index", "AjaxStavke", new { id = popravniIspitStavka.PopravniIspitId });
        }
    }
}
