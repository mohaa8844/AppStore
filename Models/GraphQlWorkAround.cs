using AppStoreApiWithIdentityServer4.Data;
using AppStoreApiWithIdentityServer4.Models;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AppStoreApiWithIdentityServer4.Models
{
    public class EnviromentType : ObjectGraphType<Enviroment>
    {
        public EnviromentType()
        {
            Field(x => x.ID);
            Field(x => x.EnviromentName);
        }
    }

    public class ChannelType : ObjectGraphType<Channel>
    {
        public ChannelType()
        {
            Field(x => x.ID);
            Field(x => x.ChannelName);
        }

    }

    public class StageType : ObjectGraphType<Stage>
    {
        public StageType()
        {
            Field(x => x.ID);
            Field(x => x.FSsource);
            Field(x => x.AppVersionID);
            Field(x => x.Size);
        }
    }

    public class AppVersionType : ObjectGraphType<AppVersion>
    {
        public AppVersionType()
        {
            Field(x => x.ID);
            Field(x => x.Changelog);
            Field(x => x.VresionName);
            Field(x => x.VresionNumber);
            Field(x => x.ReleaseDate);
            Field(x => x.AppID);
            Field(x => x.ChannelID);
        }
    }

    public class AppType : ObjectGraphType<ToseeApp>
    {
        public AppType()
        {
            Field(x => x.ID);
            Field(x => x.Name);
            Field(x => x.Description);
            Field(x => x.PackageName);
            Field(x => x.ScreenShots);
            Field(x => x.Video);
            Field(x => x.SupportedDevices);
            Field(x => x.FSiconsSources);
            Field(x => x.Public);
            Field(x => x.Published);
            Field(x => x.Downloads);
            Field(x => x.WebSite);
            Field(x => x.Enviroment);
            Field(x => x.Developer);
            Field(x => x.FSsource);
            Field(x => x.Size);
            Field(x => x.Changelog);
            Field(x => x.ReleaseDate);
            Field(x => x.VresionName);
            Field(x => x.VresionNumber);
            Field(x => x.Channel);
        }
    }
  

    public class RootQuery : ObjectGraphType
    {
        private readonly AppStoreContext _context;
        public RootQuery(AppStoreContext context)
        {
            _context = context;
            Field<ListGraphType<ChannelType>>("channels", resolve: context => GetChannels());
            Field<ChannelType>("channel", arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "iD" }), resolve: context =>
            {
                var id = context.GetArgument<int>("iD");
                return GetChannels().FirstOrDefault(x => x.ID == id);
            });

            Field<ListGraphType<StageType>>("stages", resolve: context => GetStages());
            Field<StageType>("stage", arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "iD" }), resolve: context =>
            {
                var id = context.GetArgument<int>("iD");
                return GetStages().FirstOrDefault(x => x.ID == id);
            });

            Field<ListGraphType<AppVersionType>>("appversions", resolve: context => GetAppVersions());
            Field<AppVersionType>("appversion", arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "iD" }), resolve: context =>
            {
                var id = context.GetArgument<int>("iD");
                return GetAppVersions().FirstOrDefault(x => x.ID == id);
            });

            Field<ListGraphType<AppType>>("apps", resolve: context => GetToseeApps());
            Field<AppType>("app", arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "iD" }), resolve: context =>
            {
                var id = context.GetArgument<int>("iD");
                return GetToseeApps().FirstOrDefault(x => x.ID == id);
            });


            Field<ListGraphType<EnviromentType>>("envs", resolve: context => GetEnvs());
            Field<EnviromentType>("env",arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = "iD" }), resolve: context =>
              {
                  var id = context.GetArgument<int>("iD");
                  return GetEnvs().FirstOrDefault(x => x.ID == id);
              });

        }
        List<Channel> GetChannels()
        {
            return _context.Channels.ToList();
        }
        List<Stage> GetStages()
        {
            return _context.Stages.ToList();
        }
        List<AppVersion> GetAppVersions()
        {
            return _context.AppVersions.ToList();
        }
       
        List<ToseeApp> GetToseeApps()
        {
            var apps = _context.Apps.ToList();
            List<ToseeApp> aa=  apps.ConvertAll(x=> {
                _context.Entry(x).Reference(c => c.Enviroment).Load();
                _context.Entry(x).Reference(c => c.User).Load();
                var appvs = (from q in _context.AppVersions where q.AppID == x.ID select q).AsEnumerable();
                AppVersion appv;
                Stage stage;
                String channel;
                if (appvs.Count() > 0)
                {
                    appv = appvs.Last();
                    _context.Entry(appv).Reference(c => c.Channel).Load();
                    channel = appv.Channel.ChannelName;

                    var stages = (from q in _context.Stages where q.AppVersionID == appv.ID select q).AsEnumerable();
                    if (stages.Count()>0) stage = stages.Last();
                    else stage = new Stage { FSsource = "", Size = 0.0f };
                }
                else
                {
                    appv = new AppVersion { Changelog = "", ReleaseDate = "", VresionName = "", VresionNumber = "" };
                    channel = "";
                    stage = new Stage { FSsource = "", Size = 0.0f };
                }
                
                return new ToseeApp
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                    PackageName = x.PackageName,
                    Downloads = x.Downloads,
                    WebSite = x.WebSite,
                    Published = x.Published,
                    Public = x.Public,
                    Enviroment = x.Enviroment.EnviromentName,
                    FSiconsSources = x.FSiconsSources,
                    ScreenShots = x.ScreenShots,
                    Video = x.Video,
                    SupportedDevices = x.SupportedDevices,
                    Changelog = appv.Changelog,
                    VresionName = appv.VresionName,
                    VresionNumber = appv.VresionNumber,
                    ReleaseDate = appv.ReleaseDate,
                    FSsource = stage.FSsource,
                    Size = stage.Size,
                    Developer=x.User.FullName,
                    Channel=channel
                    
                };

            });
    
            return aa;
        }
        List<Enviroment> GetEnvs()
        {
            return _context.Enviroments.ToList();
        }
    }

    public class GraphSchema : Schema
    {
        public GraphSchema(IDependencyResolver resolver) :
           base(resolver)
        {
            Query = resolver.Resolve<RootQuery>();
        }
    }

    public class ToseeApp
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String PackageName { get; set; }
        public bool Public { get; set; }
        public bool Published { get; set; }
        public int Downloads { get; set; }
        public String WebSite { get; set; }
        public String Enviroment { get; set; }
        public String ScreenShots { get; set; }
        public String Video { get; set; }
        public String SupportedDevices { get; set; }
        public String FSiconsSources { get; set; }
        public String Developer { get; set; }
        public String FSsource { get; set; }
        public String Channel { get; set; }
        public float Size { get; set; }
        public String Changelog { get; set; }
        public String ReleaseDate { get; set; }
        public String VresionName { get; set; }
        public String VresionNumber { get; set; }
    }

}
