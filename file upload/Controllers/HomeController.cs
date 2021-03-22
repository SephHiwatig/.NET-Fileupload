using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using file_upload.Models;
using file_upload.ViewModel;
using System.Threading;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;

namespace file_upload.Controllers
{
    public class HomeController : Controller
    {
        private readonly FileContext _fileContext;
        public HomeController(FileContext fileContext)
        {
            _fileContext = fileContext;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var files = await _fileContext.FileForm.ToListAsync(ct);

            ViewBag.Files = files;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ActionResult> Upload(FileViewModel fileViewModel, CancellationToken ct)
        {
            FileForm fileForm = null;

            if (fileViewModel.File.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    fileViewModel.File.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    fileForm = new FileForm
                    {
                        Name = fileViewModel.File.FileName,
                        ContentType = fileViewModel.File.ContentType,
                        File = fileBytes
                    };
                }
            }

            if(fileForm != null)
            {
                await _fileContext.FileForm.AddAsync(fileForm, ct);
                await _fileContext.SaveChangesAsync(ct);
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Download(int id, CancellationToken ct)
        {
            var fileForm = await _fileContext.FileForm.FindAsync(new object[] { id }, ct);

            if (fileForm == null)
            {
                return RedirectToAction("Index");
            }

            return File(fileForm.File, "application/octet-stream", fileForm.Name);
        }
    }
}
