using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotaTrial.Models;
using System.IO;
using System.Web.Hosting;
using DotaTrial.ViewModel;
using System.Data.Entity.Infrastructure;
using System.Security.Cryptography;

namespace DotaTrial.Controllers
{
    public class HeroController : Controller
    {
        private DotaTrialEntities1 db = new DotaTrialEntities1();

        // GET: Hero
        public ActionResult Index()
        {
            //return Redirect("https://202.60.8.156:8443/transaction/verify?terminalID=46&referenceCode=31_16-04677_20170115023422&amount=1541.9400&serviceType=Business Tax Payment&securityToken=6d54389b185795a5643cc159547982bcf183e0cd");
            return View(db.Heroes.ToList());
        }

        // GET: Hero/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hero hero = db.Heroes.Find(id);
            if (hero == null)
            {
                return HttpNotFound();
            }
            return View(hero);
        }

        // GET: Hero/Create
        public ActionResult Create()
        {
            var hero = new Hero();
            PopulateAssignItems(hero);
            return View();
        }

        // POST: Hero/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Heroname,HeroNickname")] Hero hero, HttpPostedFileBase heroImage, string[] selectedItems)
        {
            if (ModelState.IsValid)
            {
                string newFileName = null;
                if (heroImage != null && heroImage.ContentLength > 0)
                {
                    var fileExt = Path.GetExtension(heroImage.FileName);
                    if (fileExt.ToLower().EndsWith(".png") || fileExt.ToLower().EndsWith(".jpg"))
                    {
                        newFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(heroImage.FileName);
                        var filePath = HostingEnvironment.MapPath("~/Images/") + newFileName;
                        var directory = new DirectoryInfo(HostingEnvironment.MapPath("~/Images/"));
                        if (directory.Exists == false)
                        {
                            directory.Create();
                        }
                        heroImage.SaveAs(filePath);
                    }
                }
                hero.HeroImages = newFileName;
                db.Heroes.Add(hero);
                
                updateHeroItems(selectedItems, hero);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hero);
        }

        // GET: Hero/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hero = db.Heroes
                .Include(h => h.HeroItems)
                .Where(h => h.HeroID == id)
                .Single();
            PopulateAssignItems(hero);
            if (hero == null)
            {
                return HttpNotFound();
            }
            return View(hero);
        }



        // POST: Hero/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, HttpPostedFileBase heroImage, string oldImage, string[] selectedItems)
        {
            if(id== null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
      
            var heroToUpdate = db.Heroes
                    .Include(h => h.HeroItems)
                    .Where(h => h.HeroID == id)
                    .Single();
            
            if (TryUpdateModel(heroToUpdate, "", new string[] { "Heroname", "HeroNickname"})) {
                
                try
                {
                    string newFileName = null;
                    if (heroImage != null && heroImage.ContentLength > 0)
                    {
                        string getPath = HostingEnvironment.MapPath("~/Images/") + oldImage;
                        FileInfo file = new FileInfo(getPath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }

                        var fileExt = Path.GetExtension(heroImage.FileName);
                        if (fileExt.ToLower().EndsWith(".png") || fileExt.ToLower().EndsWith(".jpg"))
                        {
                            newFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(heroImage.FileName);
                            var filePath = HostingEnvironment.MapPath("~/Images/") + newFileName;
                            var directory = new DirectoryInfo(HostingEnvironment.MapPath("~/Images/"));
                            if (directory.Exists == false)
                            {
                                directory.Create();
                            }
                            heroImage.SaveAs(filePath);
                        }
                        heroToUpdate.HeroImages = newFileName;
                    }
                    
                    updateHeroItems(selectedItems, heroToUpdate);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                } catch (RetryLimitExceededException) {
                    ModelState.AddModelError("", "Unable to save Changes. Please try again.");
                }
            }
            PopulateAssignItems(heroToUpdate);
            return View(heroToUpdate);
        }

        // GET: Hero/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError=false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault()) {
                ViewBag.ErrorMessage = "Delete failed.Try again.";
            }
            Hero hero = db.Heroes
                    .Include(h => h.HeroItems)
                    .Where(h => h.HeroID == id)
                    .Single();
            PopulateAssignItems(hero);
            if (hero == null)
            {
                return HttpNotFound();
            }
            return View(hero);
        }

        // POST: Hero/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hero hero = db.Heroes.Find(id);
            string Path = HostingEnvironment.MapPath("~/Images/") + hero.HeroImages;
            FileInfo file = new FileInfo(Path);
            if (file.Exists)
            {
                file.Delete();
            }
            var heroItems = db.HeroItems.Where(hi => hi.HeroID == id);

            foreach (var heroItem in heroItems) {
                db.HeroItems.Remove(heroItem);
            }
            
            db.Heroes.Remove(hero);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private void updateHeroItems(string[] selectedItems, Hero heroToUpdate) {
            if (selectedItems == null)
            {
                heroToUpdate.HeroItems = new List<HeroItem>();
                return;
            }

            var selectedHeroItems = new HashSet<string>(selectedItems);
            var heroItems = new HashSet<int?>(heroToUpdate.HeroItems.Select(h => h.ItemID));

            foreach (var item in db.Items) {
                if (selectedItems.Contains(item.ItemID.ToString())) {
                    if (!heroItems.Contains(item.ItemID)) {
                        db.HeroItems.Add(new HeroItem
                        {
                            ItemID =item.ItemID,
                            HeroID = heroToUpdate.HeroID
                        });
                    }
                } else {
                    if (heroItems.Contains(item.ItemID))
                    {
                        var heroess = db.HeroItems.Where(hi => hi.ItemID== item.ItemID && hi.HeroID ==heroToUpdate.HeroID).Single();
                        heroToUpdate.HeroItems.Remove(heroess);
                    }
                }
            }
        }
        private void PopulateAssignItems(Hero hero)
        {
            var allItems = db.Items;
            var heroItems = new HashSet<int?>(hero.HeroItems.Select(h => h.ItemID));
            var viewModel = new List<AssignedItem>();
            foreach (var items in allItems)
            {
                viewModel.Add(new AssignedItem
                {
                    ItemID = items.ItemID,
                    ItemName = items.ItemName+bbbb(),
                    ItemStrenght = items.Strenght,
                    ItemAgility = items.Agility,
                    ItemIntelligent = items.Intelligent,
                    IsAssigned = heroItems.Contains(items.ItemID)
                });
            }
            ViewBag.Items = viewModel;
        }
        public string bbbb() {
            return "mbmmnbn";
        }
    }
}
