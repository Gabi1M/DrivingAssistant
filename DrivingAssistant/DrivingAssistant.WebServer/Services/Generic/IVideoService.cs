using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IVideoService : IGenericService<Video>
    {
        //============================================================
        public Task<Video> GetByProcessedId(long processedId);

        //============================================================
        public Task<IEnumerable<Video>> GetBySession(long sessionId);

        //============================================================
        public Task<IEnumerable<Video>> GetByUser(long userId);
    }
}
