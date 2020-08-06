using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStoreApiWithIdentityServer4.Data;
using AppStoreApiWithIdentityServer4.Handlers;
using AppStoreApiWithIdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AppStoreApiWithIdentityServer4.Controllers
{
    [Authorize(Roles =("admin,dev"))]
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {

        private readonly AppStoreContext _context;
        private readonly FileHandler _fileHander;
        private readonly NotificationsHub _notificationsHub;
        private readonly IHostEnvironment _env;

        public VersionsController(AppStoreContext context, IHostEnvironment env, NotificationsHub notificationsHub)
        {
            _context = context;
            _env = env;
            _notificationsHub = notificationsHub;
            _fileHander = new FileHandler(_env.ContentRootPath, _context);
        }




        [HttpPost]
        public async Task<ActionResult<CreationResult>> CreateVersionInfo(AppVersionData newAppVersion, int id = -1)
        {
            int appId = id == -1 ? (int)newAppVersion.AppID : id;


            AppVersion toAdd = new AppVersion
            {
                AppID = appId,
                Changelog = newAppVersion.Changelog,
                VresionName = newAppVersion.VresionName,
                VresionNumber = newAppVersion.VresionNumber,
                ReleaseDate = newAppVersion.ReleaseDate,
                ChannelID = newAppVersion.ChannelID
            };

            _context.AppVersions.Add(toAdd);
            await _context.SaveChangesAsync();
            _context.Entry(toAdd).Reference(x => x.Channel).Load();
            _context.Entry(toAdd).Reference(x => x.App).Load();
            Channel channel = toAdd.Channel;

            if (channel.ChannelName.ToLower() == "release")
            {
                App app = toAdd.App;
                if (app.Published) await _notificationsHub.SendNotification("new version" + newAppVersion.VresionNumber + " of " + app.Name);
            }
            return new CreationResult { txt = "Done", ID = toAdd.ID + "" };
        }


        [Authorize(Roles = ("admin,dev"))]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppVersion(int id, AppVersion appVersion)
        {
            if (id != appVersion.ID)
            {
                return BadRequest();
            }

            _context.Entry(appVersion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppVersionExists(id))
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


        [Authorize(Roles = ("admin,dev"))]
        [HttpDelete("{id}")]
        public async Task<ActionResult<AppVersion>> DeleteAppVersion(int id)
        {
            var appVersion = await _context.AppVersions.FindAsync(id);
            if (appVersion == null)
            {
                return NotFound();
            }

            _context.AppVersions.Remove(appVersion);
            await _context.SaveChangesAsync();

            return appVersion;
        }

        private bool AppVersionExists(int id)
        {
            return _context.AppVersions.Any(e => e.ID == id);
        }
    }
}
