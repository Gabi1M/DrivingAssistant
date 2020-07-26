using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IMediaService : IGenericService<Media>
    {
        //============================================================
        public Task<Media> GetByProcessedId(long processedId);

        //============================================================
        public Task<IEnumerable<Media>> GetBySession(long sessionId);

        //============================================================
        public Task<IEnumerable<Media>> GetByUser(long userId);
    }
}
