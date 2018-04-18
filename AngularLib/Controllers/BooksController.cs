using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LibraryAngular.Models;
using System.IO;

namespace AngularLib.Controllers
{
    public class BooksController : Controller
    {
        private ContextBooks db = new ContextBooks();
       
        // GET: Books
        public async Task<ActionResult> Index()
        {
            return View(await db.Books.ToListAsync());
        }

        public List<Author> GetOptions()
        {
            return db.Authors.ToList();

        }

        public JsonResult Genres()
        {
            return Json(db.Genres, JsonRequestBehavior.AllowGet);
        }

        public List<Genre> GenreOptions()
        {
            return db.Genres.ToList();
        }

        public async Task<JsonResult> GetGanres()
        {
            return Json(await db.Genres.ToListAsync(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteAuthors(string Prefix)
        {
            var result = (from r in db.Authors
                          where r.AuthorName.Contains(Prefix)
                          select new { r.Id, r.AuthorName });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

       
        [HttpPost]
        public async Task<JsonResult> GetAuthors(string term)
        {
            var authors = (from a in db.Authors where a.AuthorName.StartsWith(term) select a).ToListAsync();
            return Json(await authors, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> book(int id)
        {
            Book book = await db.Books.FindAsync(id);

            if (book == null)
            {
                return HttpNotFound();
            }
            if (Request.Cookies["ViewedPage"] != null)
            {
                if (Request.Cookies["ViewedPage"][string.Format("bId_{0}", book.Id)] == null)
                {
                    HttpCookie cookie = Request.Cookies["ViewedPage"];
                    cookie[string.Format("bId_{0}", book.Id)] = book.Id.ToString();
                    cookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(cookie);
                    book.ViewCount = book.ViewCount + 1;
                    db.SaveChanges();
                }
            }
            return View(book);
        }

        public async Task<JsonResult> GetSimilarBooks(string term)
        {
            var book = (from b in db.Books
                        join a in db.Authors on b.AuthorId equals a.Id
                        orderby b.Id
                        join g in db.Genres on b.GenreId equals g.GenreId
                        where g.GenreName.Contains(term)
                        orderby b.ViewCount
                        select new
                        {
                            b.Id,
                            b.Name,
                            b.Year,
                            b.Cover,
                            b.ViewCount,
                            a.AuthorName,
                            g.GenreName

                        }
                            ).Take(5).ToListAsync();
            return Json(await book, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetBooksByType(string type)
        {
            if (!String.IsNullOrEmpty(type))
            {
                var book = (from b in db.Books
                            join au in db.Authors on b.AuthorId equals au.Id
                            join a in db.FilePaths on b.Id equals a.BookID
                            orderby b.Id
                            join g in db.Genres on b.GenreId equals g.GenreId
                            where a.FileName.Contains((type))
                            orderby b.ViewCount
                            select new
                            {
                                b.Id,
                                b.Name,
                                b.Year,
                                b.Cover,
                                b.ViewCount,
                                au.AuthorName,
                                g.GenreName

                            }
                       ).ToListAsync();
                return Json(await book, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var book = (from b in db.Books
                            join au in db.Authors on b.AuthorId equals au.Id
                            orderby b.Id
                            join g in db.Genres on b.GenreId equals g.GenreId
                            orderby b.ViewCount
                            select new
                            {
                                b.Id,
                                b.Name,
                                b.Year,
                                b.Cover,
                                b.ViewCount,
                                au.AuthorName,
                                g.GenreName

                            }
                      ).ToListAsync();
                return Json(await book, JsonRequestBehavior.AllowGet);
            }
           
        }


        public async Task<JsonResult> GetBooksAsync(int? page, string term)
        {
            if (String.IsNullOrEmpty(term))
            {
                var book = (from b in db.Books
                            join a in db.Authors on b.AuthorId equals a.Id
                            orderby b.Id
                            join g in db.Genres on b.GenreId equals g.GenreId
                            orderby b.Id descending
                            select new
                            {
                                b.Id,
                                b.Name,
                                b.Year,
                                b.Cover,
                                b.Description,
                                b.ViewCount,
                                a.AuthorName,
                                g.GenreName
                            }
                            ).ToListAsync();
                return Json(await book, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var book = (from b in db.Books
                            join a in db.Authors on b.AuthorId equals a.Id
                            orderby b.Id
                            join g in db.Genres on b.GenreId equals g.GenreId
                            where g.GenreName.Contains(term)
                            select new
                            {
                                b.Id,
                                b.Name,
                                b.Year,
                                b.Cover,
                                b.Description,
                                b.ViewCount,
                                a.AuthorName,
                                g.GenreName
                            }
                            ).ToListAsync();
                return Json(await book, JsonRequestBehavior.AllowGet);
            }
        }


        [AllowAnonymous]
        public FileContentResult GetCoverImage(int id)
        {
            var cover = db.Books.FirstOrDefault(p => p.Id == id);
            if (cover.Cover != null && cover.CoverType != null)
            {
                return File(cover.Cover, cover.CoverType);
            }

            else
            {
                return null;
            }
        }

        // GET: Books/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            Book book = new Book();
            ViewBag.Authors = GetOptions();
            ViewBag.Genres = GenreOptions();

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Year, AuthorId, GenreId, Cover, CoverType, Description ")] Book book, HttpPostedFileBase file, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                         book.Cover = array;
                        book.CoverType = file.ContentType;
                    }
                }
                   db.Books.Add(book);
                   await db.SaveChangesAsync();

                Directory.CreateDirectory(Server.MapPath("~/App_Data/warehouse/book_" + book.Id));

                foreach (var f in files)
                {
                    if (f != null && f.ContentLength > 0) { 

                        var fileName = Path.GetFileName(f.FileName);
                        var path = Path.Combine(Server.MapPath("~/App_Data/warehouse/book_" + book.Id), fileName);
                        f.SaveAs(path);
                        
                        FilePath fp = new FilePath()
                        {
                            BookID = book.Id,
                            FileName = Globals.resolveVirtual(path)
                        };
                        db.FilePaths.Add(fp);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public async Task<JsonResult> GetFilePath()
        {
            var paths = (from p in db.FilePaths
                         join b in db.Books on p.BookID equals b.Id
                         select new
                         {
                             p.FilePathId,
                             p.FileName,
                             b.Id
                                }).ToListAsync();
            return Json(await paths, JsonRequestBehavior.AllowGet);
         }

            public FileResult DownloadBook(string path)
            {
                path = Path.Combine(Server.MapPath(path));
                return File(path, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(path));
            }

        public ActionResult DeleteBookFromServer(int id)
        {
            string FilePath = Path.Combine(Server.MapPath(db.FilePaths.Single(x => x.FilePathId == id).FileName));


            if (System.IO.File.Exists(FilePath))
            {
                System.IO.File.Delete(FilePath);
                FilePath fp = db.FilePaths.Find(id);
                db.FilePaths.Remove(fp);
                db.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
            

        // GET: Books/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.Authors = GetOptions();
            ViewBag.Genres = GenreOptions();
            return View(book);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Name, Year, AuthorId, GenreId, Cover, CoverType, Description")] Book book, HttpPostedFileBase file, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                        book.Cover = array;
                        book.CoverType = file.ContentType;
                    }
                    
                }

                foreach (var f in files)
                {
                    if (f != null && f.ContentLength > 0)
                    {

                        var fileName = Path.GetFileName(f.FileName);
                        var path = Path.Combine(Server.MapPath("~/App_Data/warehouse/book_" + book.Id), fileName);
                        f.SaveAs(path);

                        FilePath fp = new FilePath()
                        {
                            BookID = book.Id,
                            FileName = Globals.resolveVirtual(path)
                        };
                        db.FilePaths.Add(fp);
                        db.SaveChanges();
                    }
                }
                db.Entry(book).State = EntityState.Modified;
                if (file == null)
                {
                    db.Entry(book).Property("CoverType").IsModified = false;

                    db.Entry(book).Property("Cover").IsModified = false;
                }
                
                await db.SaveChangesAsync();
                return Redirect(Request.UrlReferrer.ToString());
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }


        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Book book = await db.Books.FindAsync(id);
            db.Books.Remove(book);
            await db.SaveChangesAsync();
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
    }
}
