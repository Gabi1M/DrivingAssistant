using System;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.Psql;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IReportService : IGenericService<Report>
    {
        //============================================================
        public static IReportService NewInstance(Type type)
        {
            if (type == typeof(PsqlReportService))
            {
                return new PsqlReportService(Constants.ServerConstants.GetPsqlConnectionString());
            }
            else if (type == typeof(MssqlReportService))
            {
                return new MssqlReportService(Constants.ServerConstants.GetMssqlConnectionString());
            }
            else
            {
                throw new Exception("Unsupported type " + type);
            }
        }
    }
}
