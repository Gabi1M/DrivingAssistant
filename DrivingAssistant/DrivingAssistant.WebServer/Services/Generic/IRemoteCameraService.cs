using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.PostgreSQL;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IRemoteCameraService : IGenericService<RemoteCamera>
    {
        //============================================================
        public static IRemoteCameraService CreateNew()
        {
            if (Constants.ServerConstants.UsePostgresql)
            {
                return new PostgresqlRemoteCameraService();
            }

            return new MssqlRemoteCameraService();
        }

        //============================================================
        public Task<RemoteCamera> GetByUser(long userId);
    }
}
