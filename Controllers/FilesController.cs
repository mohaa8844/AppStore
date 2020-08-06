using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppStoreApiWithIdentityServer4.Data;
using AppStoreApiWithIdentityServer4.Handlers;
using AppStoreApiWithIdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace AppStoreApiWithIdentityServer4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppStoreContext _context;
        private readonly FileHandler _fileHandeler;
        private readonly IHostEnvironment _env;
        public FilesController(AppStoreContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
            _fileHandeler = new FileHandler(_env.ContentRootPath,_context);
        }

        [HttpGet("{fileName}")]
        public  ActionResult DownloadFile(String fileName)
        {
            String filePath = _fileHandeler.HandleURL(fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            if (System.IO.File.Exists(filePath))
            {
                return File(fileBytes, "application/octet-stream", fileName);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<CreationResult>> UploadFile([FromForm] IFormFile file)
        {
            string fName = GenerateName(file.FileName);
            string path = Path.Combine(_env.ContentRootPath, "Files/tmp/" + fName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return new CreationResult { txt ="Done",ID= fName };
        }

        private String GenerateName(String fileName)
        {
            string[] fileNameFragments = fileName.Split(".");
            int length = fileNameFragments.Length;
            return DateTime.Now.Millisecond.ToString() + Path.GetRandomFileName().Split('.')[0] + (length > 1 ? "." + fileNameFragments[length - 1] : "");
        }
    }
}
