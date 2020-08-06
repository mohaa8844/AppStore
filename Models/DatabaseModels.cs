using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppStoreApiWithIdentityServer4.Models
{

public class User : IdentityUser
    {
        [MaxLength(45)]
        public string FullName { get; set; }
        [MaxLength(150)]
        public String Description { get; set; }

        public  ICollection<App> Apps { get; set; }
    }

    public class Role : IdentityRole
    {
        //public int ID { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }
    }
    public class Enviroment
    {
        public int ID { get; set; }
        [MaxLength(20)]
        public String EnviromentName { get; set; }
        public ICollection<App> Apps { get; set; }
    }
    public class Channel
    {
        public int ID { get; set; }
        [MaxLength(20)]
        public String ChannelName { get; set; }
        public ICollection<AppVersion> AppVersions { get; set; }
    }
    public class Stage {
        public int ID { get; set; }
        [MaxLength(200)]
        public String FSsource { get; set; }
        public int AppVersionID { get; set; }
        public float Size { get; set; }

        public  AppVersion AppVersion { get; set; }

    }
    public class AppVersion
    {
        public int ID { get; set; }
        [MaxLength(450)]
        public String Changelog { get; set; }
        [MaxLength(20)]
        public String VresionName { get; set; }
        [MaxLength(15)]
        public String VresionNumber { get; set; }
        [MaxLength(20)]
        public String ReleaseDate { get; set; }
        public int AppID { get; set; }
        public int ChannelID { get; set; }

        public  ICollection<Stage> Stages { get; set; }
        public  App App { get; set; }
        public Channel Channel { get; set; }

    }
    public class App
    {
        public int ID { get; set; }
        [MaxLength(45)]
        public String Name { get; set; }
        [MaxLength(450)]
        public String Description { get; set; }
        [MaxLength(35)]
        public String  PackageName { get; set; }
        [MaxLength(270)]
        public String ScreenShots { get; set; }
        [MaxLength(270)]
        public String Video { get; set; }
        [MaxLength(200)]
        public String SupportedDevices { get; set; }
        [MaxLength(270)]
        public String FSiconsSources { get; set; }
        public bool Public { get; set; }
        public bool Published { get; set; }
        public int Downloads { get; set; }
        [MaxLength(45)]
        public String WebSite { get; set; }
        public int EnviromentID { get; set; }
        public String UserID { get; set; }

        public  User User { get; set; }
        public  ICollection<AppVersion> AppVersions { get; set; }
        public  Enviroment Enviroment { get; set; }
    }


}
