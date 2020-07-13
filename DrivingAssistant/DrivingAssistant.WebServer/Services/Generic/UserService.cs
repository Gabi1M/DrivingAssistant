using System;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.Psql;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public abstract class UserService : GenericService<User>
    {
        //============================================================
        public static UserService NewInstance(Type type)
        {
            if (type == typeof(PsqlUserService))
            {
                return new PsqlUserService(Constants.ServerConstants.GetPsqlConnectionString());
            }
            else if (type == typeof(MssqlUserService))
            {
                return new MssqlUserService(Constants.ServerConstants.GetMssqlConnectionString());
            }
            else
            {
                throw new Exception("Unsupported type " + type);
            }
        }
    }
}
