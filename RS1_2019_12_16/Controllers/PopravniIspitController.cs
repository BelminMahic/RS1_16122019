using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2019_12_16.EF;
using RS1_2019_12_16.EntityModels;
using RS1_2019_12_16.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_2019_12_16.Controllers
{
    public class PopravniIspitController : Controller
    {
        private readonly MojContext db;

        public PopravniIspitController(MojContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var index = new PopravniIspitIndexVM
            {
                filterSkolskeGod=db.SkolskaGodina.Select(
                    x=>new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value=x.Id.ToString(),
                        Text=x.Naziv,
                        Selected=false
                    }
                    ).ToList(),
                filterSkole = db.Skola.Select(
                    x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Naziv,
                        Selected = false

                    }
                    ).ToList(),
                filterPredmeti = db.Predmet.Select(
                    x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Naziv,
                        Selected = false

                    }
                    ).ToList(),
            };


            return View(index);
        }


        public IActionResult Display(PopravniIspitIndexVM model)
        {
            var display = new PopravniIspitDisplayVM
            {
                SkolskaGodId = model.SkolskaGodId,
                SkolskaGodNaziv = db.SkolskaGodina.Find(model.SkolskaGodId).Naziv,
                SkolaId = model.SkolaId,
                SkolaNaziv = db.Skola.Find(model.SkolaId).Naziv,
                PredmetId = model.PredmetId,
                PredmetNaziv = db.Predmet.Find(model.PredmetId).Naziv,
                rows = db.PopravniIspit.Where(
                        x => x.PredmetId == model.PredmetId && x.SkolaId == model.SkolaId && x.SkolskaGodinaId == model.SkolskaGodId)
                        .Select(i => new PopravniIspitDisplayVM.Rows
                        {
                            PopravniIspitId=i.Id,
                            Datum=i.DatumPopravnogIspita.ToString("dd.MM.yyyy"),
                            BrojUcenikaNaPopravnom=db.PopravniIspitStavka.Where(x=>x.PopravniIspitId==i.Id).Count(),
                            BrojPolozenihNaPopravnom=db.PopravniIspitStavka.Where(x=>x.Bodovi>50 && x.PopravniIspitId==i.Id).Count()
                        }
                        ).ToList()

            };

            foreach (var predmeti in display.rows)
            {
                predmeti.ClanKomisije1 = DobaviClanaKomisije(predmeti.PopravniIspitId);
            }

            return View(display);
        }

        string DobaviClanaKomisije(int id)
        {
            var clan = db.Komisija.Where(x => x.PopravniIspitId == id).Include(x => x.Nastavnik).FirstOrDefault();

            return clan != null ? clan.Nastavnik.Ime + " " + clan.Nastavnik.Prezime : "N/A";

        }

        public IActionResult Add(int skolskaId,int skolaId,int predmetId)
        {

            var dodaj = new PopravniIspitDodajVM
            {
                SkolskaGodId = skolskaId,
                SkolskaGodNaziv = db.SkolskaGodina.Find(skolskaId).Naziv,
                SkolaId = skolaId,
                SkolaNaziv = db.Skola.Find(skolaId).Naziv,
                PredmetId = predmetId,
                PredmetNaziv = db.Predmet.Find(predmetId).Naziv,
                filterNastavnici = db.Nastavnik.Select(
                    x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Ime +" "+x.Prezime
                    }
                    ).ToList(),
                Datum=DateTime.Now.ToLocalTime()
            };



            return View(dodaj);
        }
        public IActionResult Save(PopravniIspitDodajVM model)
        {
            var popravniIspit = new PopravniIspit
            {
                SkolaId=model.SkolaId,
                SkolskaGodinaId=model.SkolskaGodId,
                PredmetId=model.PredmetId,
                DatumPopravnogIspita=model.Datum
            };
            db.Add(popravniIspit);
            db.SaveChanges();


            var prviClan = new Komisija
            {
                NastavnikId = model.ClanKomisije1Id,
                PopravniIspitId = popravniIspit.Id
            };
            db.Add(prviClan);
            db.SaveChanges();

            var drugiClan = new Komisija
            {
                NastavnikId = model.ClanKomisije2Id,
                PopravniIspitId = popravniIspit.Id
            };
            db.Add(drugiClan);
            db.SaveChanges();

            var treciClan = new Komisija
            {
                NastavnikId = model.ClanKomisije3Id,
                PopravniIspitId = popravniIspit.Id
            };
            db.Add(treciClan);
            db.SaveChanges();

            //            Prilikom Snimanja novog popravnog ispita potrebno je
            //na popravni ispit dodati sljedeće učenike:
            //            -
            //            učenici koji imaju negativnu zaključenu ocjenu na
            //kraju školske godinu za odabrani predmet
            //-
            //Učenici koji nemaju pravo na popravni ispit(imaju
            //zaključene negativne ocjene na kraju školske
            //godine iz tri ili više predmeta) se dodaju na
            //popravni ispit, ali im se evidentiraju bodovi 0.
            //Takvim učenicima nije moguće mijenjati bodove.
            //(Primjer ovoga je Učenik D na slici iz koraka 4)

            var ucenici = db.OdjeljenjeStavka.Where(x => x.Odjeljenje.Skola.Id == model.SkolaId && x.Odjeljenje.SkolskaGodina.Id == model.SkolskaGodId)
                                            .ToList();

            foreach (var u in ucenici)
            {
                var negativnaOcjena = db.DodjeljenPredmet.Where(x => x.Predmet.Id == model.PredmetId && x.OdjeljenjeStavkaId == u.Id && x.ZakljucnoKrajGodine == 1).ToList();


                var triNegativneOcjene = db.DodjeljenPredmet.Where(x => x.ZakljucnoKrajGodine == 1 && x.OdjeljenjeStavka.Id == u.Id).ToList();

                if(triNegativneOcjene.Count()>=3)
                {
                    var popravniIspitStavka = new PopravniIspitStavka
                    {
                        OdjeljenjeStavkaId=u.Id,
                        PopravniIspitId=popravniIspit.Id,
                        IsPristupio=null,
                        Bodovi=0
                    };                 

                    db.Add(popravniIspitStavka);
                    db.SaveChanges();
                }
                else if(negativnaOcjena.Any())
                {
                    var popravniIspitStavka = new PopravniIspitStavka
                    {
                        OdjeljenjeStavkaId = u.Id,
                        PopravniIspitId = popravniIspit.Id,
                        IsPristupio = false
                    };
                    db.Add(popravniIspitStavka);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index","PopravniIspit");//ne zaboravi redirect
        }


        public IActionResult Edit(int id)
        {
            var popravniIspit = db.PopravniIspit.Where(x => x.Id == id).Include(x => x.Predmet)
                .Include(x => x.Skola)
                .Include(x => x.SkolskaGodina)
                .FirstOrDefault();


            var komisija = db.Komisija.Where(x => x.PopravniIspitId == id).Include(x => x.Nastavnik);

            var uredi = new PopravniIspitUrediVM
            {
                PopravniIspitId=id,
                PredmetId=popravniIspit.PredmetId,
                PredmetNaziv=popravniIspit.Predmet.Naziv,
                SkolaId=popravniIspit.SkolaId,
                SkolaNaziv=popravniIspit.Skola.Naziv,
                SkolskaGodId=popravniIspit.SkolskaGodinaId,
                SkolskaGodNaziv=popravniIspit.SkolskaGodina.Naziv,
                Datum=popravniIspit.DatumPopravnogIspita.ToString("dd.MM.yyyy")              
                
            };

            foreach (var k in komisija)
                uredi.Komisija.Add(k.Nastavnik.Ime + " " + k.Nastavnik.Prezime);



            return View(uredi);
        }
        public IActionResult AddStudent(int id)
        {
            var ucenikNaIspitu = db.PopravniIspitStavka.Where(x => x.PopravniIspitId == id).Include(x => x.OdjeljenjeStavka).ToList();


            var dodajStudenta = new PopravniIspitUcenikDodajVM
            {
                PopravniIspitId = id,
                ucenici = db.OdjeljenjeStavka.Include(i => i.Ucenik).Where(i => !ucenikNaIspitu.Any(j => j.OdjeljenjeStavkaId == i.Id))
                        .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Value = x.Ucenik.Id.ToString(),
                            Text = x.Ucenik.ImePrezime
                        }
                        ).ToList()
            };

            return View(dodajStudenta);
        }
        public IActionResult SaveStudent(PopravniIspitUcenikDodajVM model)
        {
            var popravniIspitStavka = new PopravniIspitStavka
            {
                PopravniIspitId=model.PopravniIspitId,
                OdjeljenjeStavkaId=model.OdjeljenjeStavkaId,
                IsPristupio=false,
                Bodovi=0
            };
            db.Add(popravniIspitStavka);
            db.SaveChanges();

            return RedirectToAction("Edit", "PopravniIspit", new { id = popravniIspitStavka.PopravniIspitId });

        }
    }
}
