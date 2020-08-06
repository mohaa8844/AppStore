using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppStoreApiWithIdentityServer4.Data;
using AppStoreApiWithIdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;

namespace AppStoreApiWithIdentityServer4.Controllers
{
    [Authorize(Roles ="admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly AppStoreContext _context;

        public ChannelsController(AppStoreContext context)
        {
            _context = context;
        }


        // PUT: api/ChannelDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChannelData(int? id, ChannelData channelData)
        {
            Channel toChange = new Channel { ChannelName = channelData.ChannelName, ID = (int)id };
            if (id != toChange.ID)
            {
                return BadRequest();
            }

            _context.Entry(toChange).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChannelDataExists(id))
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

        // POST: api/ChannelDatas
        [HttpPost]
        public async Task<ActionResult<CreationResult>> PostChannelData(ChannelData channelData)
        {
            Channel toAdd = new Channel { ChannelName = channelData.ChannelName };
            _context.Channels.Add(toAdd);
            await _context.SaveChangesAsync();

            return new CreationResult { txt = "Done", ID =toAdd.ID+""};
        }

        // DELETE: api/ChannelDatas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BasicResult>> DeleteChannelData(int? id)
        {
            var channelData = await _context.Channels.FindAsync(id);
            if (channelData == null)
            {
                return NotFound();
            }

            _context.Channels.Remove(channelData);
            await _context.SaveChangesAsync();

            return new BasicResult { txt = "Done" };
        }

        private bool ChannelDataExists(int? id)
        {
            return _context.Channels.Any(e => e.ID == id);
        }
    }
}
