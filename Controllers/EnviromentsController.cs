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
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EnviromentsController : ControllerBase
    {
        private readonly AppStoreContext _context;

        public EnviromentsController(AppStoreContext context)
        {
            _context = context;
        }

        // PUT: api/EnviromentDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnviromentData(int? id, EnviromentData enviromentData)
        {
            Enviroment tochange = new Enviroment { ID = (int)id, EnviromentName = enviromentData.EnviromentName };
            if (id != tochange.ID)
            {
                return BadRequest();
            }

            _context.Entry(tochange).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnviromentDataExists(id))
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

        // POST: api/EnviromentDatas
        [HttpPost]
        public async Task<ActionResult<Enviroment>> PostEnviromentData(EnviromentData enviromentData)
        {
            _context.Enviroments.Add(new Enviroment { EnviromentName =enviromentData.EnviromentName});
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnviromentData", new { id = enviromentData.ID }, enviromentData);
        }

        // DELETE: api/EnviromentDatas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Enviroment>> DeleteEnviromentData(int? id)
        {
            var enviromentData = await _context.Enviroments.FindAsync(id);
            if (enviromentData == null)
            {
                return NotFound();
            }

            _context.Enviroments.Remove(enviromentData);
            await _context.SaveChangesAsync();

            return enviromentData;
        }

        private bool EnviromentDataExists(int? id)
        {
            return _context.Enviroments.Any(e => e.ID == id);
        }
    }
}
