using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppStoreApiWithIdentityServer4.Data;
using AppStoreApiWithIdentityServer4.Handlers;
using AppStoreApiWithIdentityServer4.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AppStoreApiWithIdentityServer4.Controllers
{   
    [Authorize(Roles ="admin,dev")]
    [Route("api/[controller]")]
    [ApiController]
    public class StagesController : ControllerBase
    {

        private readonly AppStoreContext _context;
        private readonly FileHandler _fileHander;
        private readonly IHostEnvironment _env;

        public StagesController(AppStoreContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
            _fileHander = new FileHandler(_env.ContentRootPath, _context);
        }
        [HttpPost]
        public async Task<ActionResult<CreationResult>> CreateStageInfo(StageData newStage, int id = -1)
        {
            int versionID = id == -1 ? (int)newStage.AppVersionID : id;

            AppVersion appvr = _context.AppVersions.Where(x => x.ID == versionID).SingleOrDefault();
            _context.Entry(appvr).Reference(c => c.App).Load();

            App app = appvr.App;

            String fileSrc = "";
            float fileSize = 0.0f;
            if (app.Public)
            {
                fileSrc = newStage.FSsource; ;
                fileSize = 0.0f; //GetFileSize(fileSrc);

            }
            else
            {
                _context.Entry(app).Reference(c => c.Enviroment).Load();

                String iconsSrc = app.FSiconsSources;

                if (app.Enviroment.EnviromentName.ToLower() == "android")
                {
                    fileSrc = _fileHander.acceptableName("Apk", newStage.FSsource);
                    _fileHander.saveFile("Apk", newStage.FSsource, fileSrc);
                    fileSize = _fileHander.FileSize("Apk", fileSrc);
                }
                else if (app.Enviroment.EnviromentName.ToLower() == "ios")
                {
                    String tmpIpaSrc = _fileHander.acceptableName("Ipa", newStage.FSsource);
                    _fileHander.saveFile("Ipa", newStage.FSsource, tmpIpaSrc);
                    fileSrc = _fileHander.GenerateManifest(tmpIpaSrc, iconsSrc, versionID);
                }
            }
            Stage toAdd = new Stage
            {
                AppVersionID = versionID,
                FSsource = fileSrc,
                Size = fileSize,
            };
            _context.Stages.Add(toAdd);
            await _context.SaveChangesAsync();

            return new CreationResult { txt = "Done", ID = toAdd.ID + "" };
        }


        [Authorize(Roles = "admin,dev")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStage(int id, Stage stage)
        {
            if (id != stage.ID)
            {
                return BadRequest();
            }

            _context.Entry(stage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Stage>> DeleteStage(int id)
        {
            var stage = await _context.Stages.FindAsync(id);
            if (stage == null)
            {
                return NotFound();
            }

            _context.Stages.Remove(stage);
            await _context.SaveChangesAsync();

            return stage;
        }

        private bool StageExists(int id)
        {
            return _context.Stages.Any(e => e.ID == id);
        }

    }
}
