using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.PostgreSQL;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IDrivingSessionService : IGenericService<DrivingSession>
    {
        //============================================================
        public static IDrivingSessionService CreateNew()
        {
            if (Constants.ServerConstants.UsePostgresql)
            {
                return new PostgresqlDrivingSessionService();
            }

            return new MssqlDrivingSessionService();
        }

        //============================================================
        public Task<IEnumerable<DrivingSession>> GetByUser(long userId);
    }
}
