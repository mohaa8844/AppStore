using AppStoreApiWithIdentityServer4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppStoreApiWithIdentityServer4.Handlers
{
    public class FileHandler
    {
        private readonly String _PREFIX;
        private readonly String _WEBSITE;
        private readonly AppStoreContext _context;
        public FileHandler(String prefix,AppStoreContext context)
        {
            _WEBSITE = "https://localhost:44374/api/Files/";
            _PREFIX = prefix;
            _context = context;
        }

        public String HandleFile(String name)
        {
            return _WEBSITE + name;
        }
        public String HandleURL(String name) {
            return _PREFIX + "/Files/Data/" + GetType(name) + "s/" + name;
        }

        public String GetType(String name)
        {
            String ext = name.Split('.').Last().ToLower();
            String type="";
            if (ext.Contains("apk"))
            {
                type = "Apk";
            }else if (ext == "ico")
            {
                type = "Icon";
            }else if (ext == "png" || ext == "jpg" || ext == "jpeg" || ext == "bmp")
            {
                type = "Image";
            }else if (ext == "plist")
            {
                type = "Manifest";
            }else if (ext == "mp4" || ext == "avi" || ext == "webm"||ext=="ogg"||ext== "wmv"||ext=="3gp"||ext=="flv")
            {
                type = "Video";
            }else if (ext == "ipa")
            {
                type = "Ipa";
            }


            return type;
        }


        public String GenerateName(String fileName)
        {
            string[] fileNameFragments = fileName.Split(".");
            int length = fileNameFragments.Length;
            return DateTime.Now.Millisecond.ToString() + Path.GetRandomFileName().Split('.')[0] + (length > 1 ? "." + fileNameFragments[length - 1] : "");

        }
        public String acceptableName(String path, String filename)
        {
            if (filename == "") return "";
            String[] names = filename.Split(",");
            String[] newNames = new string[names.Length];
            String name;
            for (int i = 0; i < names.Length; i++)
            {
                name = names[i];
                int count = 0;
                while (System.IO.File.Exists(_PREFIX + "/Files/Data/" + path + "s/" + name))
                {
                    name = GenerateName(name);
                    if (count > 3) name = "media253" + name;
                }
                newNames[i] = name;

            }
            return String.Join(",", newNames);
        }

        public string GenerateManifest(string Ipa, string icons, int id)
        {
            var info = (from q in _context.AppVersions
                        where q.ID == id
                        select new
                        {
                            appVersion = q.VresionNumber,
                            appName = q.App.Name,
                            packageName = q.App.PackageName
                        }).SingleOrDefault();

            String tamplateManifest = System.IO.File.ReadAllText(_PREFIX + "/Files/Data/Manifests/tamplate.plist");
            tamplateManifest = tamplateManifest
                .Replace("%ipaurl%", HandleFile(Ipa))
                .Replace("%iconurl%", HandleFile(icons.Split(',')[0]))
                .Replace("%version%", info.appVersion).Replace("%appname%", info.appName)
                .Replace("%packagename%", info.packageName);

            String fileName = acceptableName("Manifest", "AppManifest.plist");
            System.IO.File.WriteAllText(_PREFIX + "/Files/Data/Manifests/" + fileName, tamplateManifest);
            return fileName;

        }
        public void saveFile(String type, String srcName, String dstName)
        {
            String pathPrefix = _PREFIX;
            String[] srcs = srcName.Split(',');
            String[] dsts = dstName.Split(',');
            for (int i = 0; i < srcs.Length; i++)
            {
                System.IO.File.Move(pathPrefix + "/Files/tmp/" + srcs[i], pathPrefix + "/Files/Data/" + type + "s/" + dsts[i]);
            }
        }

        public float FileSize(String type, String src)
        {
            String pathPrefix = _PREFIX;
            return new FileInfo(pathPrefix + "/Files/Data/" + type + "s/" + src).Length / (1024 * 1024);
        }
        public float GetFileSize(String url)
        {
            float result = -1;

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            req.Method = "HEAD";
            using (System.Net.WebResponse resp = req.GetResponse())
            {
                if (long.TryParse(resp.Headers.Get("Content-Length"), out long ContentLength))
                {
                    result = ContentLength;
                }
            }

            return result / (1024 * 1024);
        }


    }
}
