using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.PostgreSQL;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IVideoService : IGenericService<VideoRecording>
    {
        //============================================================
        public static IVideoService CreateNew()
        {
            if (Constants.ServerConstants.UsePostgresql)
            {
                return new PostgresqlVideoService();
            }

            return new MssqlVideoService();
        }

        //============================================================
        public Task<VideoRecording> GetByProcessedId(long processedId);

        //============================================================
        public Task<IEnumerable<VideoRecording>> GetBySession(long sessionId);

        //============================================================
        public Task<IEnumerable<VideoRecording>> GetByUser(long userId);
    }
}
