using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppStoreApiWithIdentityServer4.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppStoreApiWithIdentityServer4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationsHub _notificationsHub;
        public NotificationsController(NotificationsHub notificationsHub)
        {
            _notificationsHub = notificationsHub;
        }
        [HttpGet]
        public IActionResult Index()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        await _notificationsHub.SendNotification(DateTime.Now.ToShortDateString());
                        Thread.Sleep(2000);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
            return NoContent();
        }
    }
}
