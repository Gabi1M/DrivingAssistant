using System;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.Psql;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public abstract class MediaService : GenericService<Media>
    {
        //============================================================
        public static MediaService NewInstance(Type type)
        {
            if (type == typeof(PsqlMediaService))
            {
                return new PsqlMediaService(Constants.ServerConstants.GetPsqlConnectionString());
            }
            else if (type == typeof(MssqlMediaService))
            {
                return new MssqlMediaService(Constants.ServerConstants.GetMssqlConnectionString());
            }
            else
            {
                throw new Exception("Unsupported type " + type);
            }
        }
    }
}
