using AppStoreApiWithIdentityServer4.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStoreApiWithIdentityServer4.Data
{
    public class AppStoreContext :
    IdentityDbContext<User, Role, string>
    {
        public AppStoreContext
        (DbContextOptions<AppStoreContext> options)
         : base(options)
        { }


        public DbSet<Enviroment> Enviroments { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<AppVersion> AppVersions { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<Channel> Channels { get; set; }
    
    }
}
