using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Services.PostgreSQL;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IUserSettingsService : IGenericService<UserSettings>
    {
        //============================================================
        public static IUserSettingsService CreateNew()
        {
            if (Constants.ServerConstants.UsePostgresql)
            {
                return new PostgresqlUserSettingsService();
            }

            return new MssqlUserSettingsService();
        }

        //============================================================
        public Task<UserSettings> GetByUser(long userId);
    }
}
