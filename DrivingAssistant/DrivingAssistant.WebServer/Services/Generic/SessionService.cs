using System;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.Psql;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public abstract class SessionService : GenericService<Session>
    {
        //============================================================
        public static SessionService NewInstance(Type type)
        {
            if (type == typeof(PsqlSessionService))
            {
                return new PsqlSessionService(Constants.ServerConstants.GetPsqlConnectionString());
            }
            else if (type == typeof(MssqlSessionService))
            {
                return new MssqlSessionService(Constants.ServerConstants.GetMssqlConnectionString());
            }
            else
            {
                throw new Exception("Unsupported type " + type);
            }
        }
    }
}
