using AppStoreApiWithIdentityServer4.Models;
using ClickHouse.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStoreApiWithIdentityServer4.Handlers
{
    public class LogsHandler
    {
        private readonly ClickHouseDatabase _clickDB;
        public LogsHandler(ClickHouseDatabase clickDB)
        {
            _clickDB = clickDB;
        
        }

        public void AddLog(int appId,String versionNumber)
        {
            DownloadLog log = new DownloadLog { app_id = appId, app_version = versionNumber, hits = 1, time_range =GenerateRange()};
            _clickDB.ExecuteNonQuery("insert into download_logs values("+log.app_id+",'"+log.app_version+"','"+log.time_range+"',"+log.hits+")");
        }
        internal void AddOne(int appId, string versionNumber,int hits)
        {
            _clickDB.ExecuteSelectCommand("UPDATE download_logs SET hits="+hits+1+" where app_id=" + appId + " and app_version='" + versionNumber);
        }
        public Object[][] GetLogs(int appId,String versionNumber)
        {
            return _clickDB.ExecuteSelectCommand("SELECT * FROM download_logs where app_id="+appId+" and app_version='"+ versionNumber + "'  ORDER BY time_range DESC LIMIT 1");
        }

        public bool CheckPresist(String range)
        {
            return range == GenerateRange() ? true : false;
        }
        
        private String GenerateRange()
        {
            return DateTime.Now.Year + "/" + DateTime.Now.Month + "-" + DateTime.Now.Year + "/" +((int) DateTime.Now.Month + 1);
        }

        
    }
}
