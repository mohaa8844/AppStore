using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStoreApiWithIdentityServer4.Data;
using AppStoreApiWithIdentityServer4.Handlers;
using AppStoreApiWithIdentityServer4.Models;
using ClickHouse.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AppStoreApiWithIdentityServer4.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AppsController : ControllerBase
    {
        private readonly AppStoreContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHostEnvironment _env;
        private readonly FileHandler _fileHandler;
        private readonly NotificationsHub _notificationsHub;
        private readonly LogsHandler _logsHandler;

        public AppsController(AppStoreContext context,ClickHouseDatabase clickDB, UserManager<User> userManager, IHostEnvironment env, NotificationsHub notificationsHub)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _fileHandler = new FileHandler(_env.ContentRootPath,_context);
            _notificationsHub = notificationsHub;
            _logsHandler = new LogsHandler(clickDB);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BasicResult>> Download(int id)
        {
            App app =await _context.Apps.Where(x => x.ID == id).SingleOrDefaultAsync();
            int appId = app.ID;
            List<AppVersion> appVersions = await _context.AppVersions.Where(x => x.AppID == id).ToListAsync();
            String versionNumber = appVersions.LastOrDefault().VresionNumber;
            //var logs = _logsHandler.GetLogs(app.ID, versionNumber);
            //if (logs.Count() > 0 && _logsHandler.CheckPresist(logs[0][2].ToString()))
            //{
            //    _logsHandler.AddOne(appId, versionNumber,(int)logs[0][3]);
            //}
            //else
            //{
            //    _logsHandler.AddLog(appId, versionNumber);
            //}
            _logsHandler.AddLog(appId, versionNumber);

            return new BasicResult { txt = "Done" };
        }


        [Authorize(Roles = "admin,dev")]
        [HttpPost]
        public async Task<ActionResult<CreationResult>> CreateAppInfo(AppData newApp)
        {
            String txt = "";
            User user = _userManager.GetUserAsync(HttpContext.User).Result;

            String videoSrc = _fileHandler.acceptableName("Video", newApp.Video);
            _fileHandler.saveFile("Video", newApp.Video, videoSrc);

            String screenShotsSrc = _fileHandler.acceptableName("Image", newApp.ScreenShots);
            _fileHandler.saveFile("Image", newApp.ScreenShots, screenShotsSrc);

            String iconsSrc = _fileHandler.acceptableName("Icon", newApp.FSiconsSources);
            _fileHandler.saveFile("Icon", newApp.FSiconsSources, iconsSrc);

            App toAdd = new App
            {
                Name = newApp.Name,
                Description = newApp.Description,
                PackageName = newApp.PackageName,
                Downloads = 0,
                WebSite = newApp.WebSite,
                Published = (bool)newApp.Published,
                Public = (bool)newApp.Public,
                EnviromentID = (int)newApp.EnviromentID,
                UserID = user.Id,
                FSiconsSources = iconsSrc,
                ScreenShots = screenShotsSrc,
                Video = videoSrc,
                SupportedDevices = newApp.SupportedDevices
            };
            _context.Apps.Add(toAdd);
            await _context.SaveChangesAsync();
            return new CreationResult { txt = "Done", ID = toAdd.ID + "" };
        }

        [Authorize(Roles = "admin,dev")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApp(int id, App app)
        {
            if (id != app.ID)
            {
                return BadRequest();
            }

            _context.Entry(app).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppExists(id))
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


        [Authorize(Roles = "admin,dev")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<App>> DeleteApp(int id)
        {
            var app = await _context.Apps.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            _context.Apps.Remove(app);
            await _context.SaveChangesAsync();

            return app;
        }


        [Authorize(Roles = "admin,dev")]
        [HttpPost("allinfo")]
        public async Task<ActionResult<CreationResult>> CreateAllInfo(CollectiveAppData newAll)
        {

            StagesController stage = new StagesController(_context, _env);
            VersionsController version = new VersionsController(_context, _env, _notificationsHub);

            int appId = int.Parse(await Task.FromResult<string>(CreateAppInfo(newAll.AppData).Result.Value.ID));
            int versionId = int.Parse(await Task.FromResult<string>(version.CreateVersionInfo(newAll.AppVersionData, appId).Result.Value.ID));
            int stageId = int.Parse(await Task.FromResult<string>(stage.CreateStageInfo(newAll.StageData, versionId).Result.Value.ID));
            return new CreationResult { txt = "Done", ID = appId + "" };
        }

        private bool AppExists(int id)
        {
            return _context.Apps.Any(e => e.ID == id);
        }
    }
}
