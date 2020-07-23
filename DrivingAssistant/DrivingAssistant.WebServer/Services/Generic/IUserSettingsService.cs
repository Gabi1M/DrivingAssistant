using System;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.Psql;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IUserSettingsService : IGenericService<UserSettings>
    {
        //============================================================
        public static IUserSettingsService NewInstance(Type type)
        {
            if (type == typeof(PsqlUserSettingsService))
            {
                return new PsqlUserSettingsService(Constants.ServerConstants.GetPsqlConnectionString());
            }
            else if (type == typeof(MssqlUserSettingsService))
            {
                return new MssqlUserSettingsService(Constants.ServerConstants.GetMssqlConnectionString());
            }
            else
            {
                throw new Exception("Unsupported type " + type);
            }
        }
    }
}
