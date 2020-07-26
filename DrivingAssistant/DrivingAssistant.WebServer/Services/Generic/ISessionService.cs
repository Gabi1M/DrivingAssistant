using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface ISessionService : IGenericService<Session>
    {
        //============================================================
        public Task<IEnumerable<Session>> GetByUser(long userId);
    }
}
