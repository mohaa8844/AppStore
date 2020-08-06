using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppStoreApiWithIdentityServer4.Models
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        public String Description { get; set; }
    }
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class EnviromentData
    {
        public Nullable<int> ID { get; set; }
        public String EnviromentName { get; set; }
    }

    public class ChannelData
    {
        public Nullable<int> ID { get; set; }
        public String ChannelName { get; set; }

    }

    public class AppData
    {

        public Nullable<int> ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String PackageName { get; set; }
        public Nullable<bool> Public { get; set; }
        public Nullable<bool> Published { get; set; }
        public int Downloads { get; set; }
        public String WebSite { get; set; }
        public Nullable<int> EnviromentID { get; set; }
        public String ScreenShots { get; set; }
        public String Video { get; set; }
        public String SupportedDevices { get; set; }
        public String FSiconsSources { get; set; }
        public String UserID { get; set; }

        public User User { get; set; }
        public ICollection<AppVersion> AppVersions { get; set; }
        public Enviroment Enviroment { get; set; }
    }
    public class AppVersionData
    {
        public Nullable<int> ID { get; set; }
        public String Changelog { get; set; }
        public String ReleaseDate { get; set; }
        public String VresionName { get; set; }
        public String VresionNumber { get; set; }
        public Nullable<int> AppID { get; set; }
        public int ChannelID { get; set; }
        public ICollection<Stage> Stages { get; set; }
        public App App { get; set; }
        public Channel Channel { get; set; }
    }


    public class StageData
    {
        public Nullable<int> ID { get; set; }
        public String FSsource { get; set; }
        public Nullable<int> AppVersionID { get; set; }
        public float Size { get; set; }

        public AppVersion AppVersion { get; set; }
    }

    public class CollectiveAppData
    {
        public AppData AppData { get; set; }
        public AppVersionData AppVersionData { get; set; }
        public StageData StageData { get; set; }

    }
}

