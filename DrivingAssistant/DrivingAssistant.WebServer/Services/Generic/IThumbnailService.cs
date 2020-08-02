using System.Threading.Tasks;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IThumbnailService : IGenericService<Thumbnail>
    {
        //============================================================
        public Task<Thumbnail> GetByVideo(long videoId);
    }
}
