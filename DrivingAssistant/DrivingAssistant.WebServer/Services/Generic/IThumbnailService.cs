using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.PostgreSQL;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IThumbnailService : IGenericService<Thumbnail>
    {
        //============================================================
        public static IThumbnailService CreateNew()
        {
            if (Constants.ServerConstants.UsePostgresql)
            {
                return new PostgresqlThumbnailService();
            }

            return new MssqlThumbnailService();
        }

        //============================================================
        public Task<Thumbnail> GetByVideo(long videoId);
    }
}
