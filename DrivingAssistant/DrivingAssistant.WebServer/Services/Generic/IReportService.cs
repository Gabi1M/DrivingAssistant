using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models.Reports;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.PostgreSQL;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IReportService : IGenericService<LaneDepartureWarningReport>
    {
        //============================================================
        public static IReportService CreateNew()
        {
            if (Constants.ServerConstants.UsePostgresql)
            {
                return new PostgresqlReportService();
            }

            return new MssqlReportService();
        }

        //============================================================
        public Task<LaneDepartureWarningReport> GetByVideo(long videoId);

        //============================================================
        public Task<IEnumerable<LaneDepartureWarningReport>> GetBySession(long sessionId);

        //============================================================
        public Task<IEnumerable<LaneDepartureWarningReport>> GetByUser(long userId);
    }
}
