using System.Threading.Tasks;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IUserSettingsService : IGenericService<UserSettings>
    {
        //============================================================
        public Task<UserSettings> GetByUser(long userId);
    }
}
