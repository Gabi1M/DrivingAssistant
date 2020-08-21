using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.PostgreSQL;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface ISessionService : IGenericService<Session>
    {
        //============================================================
        public static ISessionService CreateNew()
        {
            if (Constants.ServerConstants.UsePostgresql)
            {
                return new PostgresqlSessionService();
            }

            return new MssqlSessionService();
        }

        //============================================================
        public Task<IEnumerable<Session>> GetByUser(long userId);
    }
}
