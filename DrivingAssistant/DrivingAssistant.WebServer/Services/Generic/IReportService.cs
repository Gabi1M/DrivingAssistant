using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IReportService : IGenericService<Report>
    {
        //============================================================
        public Task<Report> GetByVideo(long videoId);

        //============================================================
        public Task<IEnumerable<Report>> GetBySession(long sessionId);

        //============================================================
        public Task<IEnumerable<Report>> GetByUser(long userId);
    }
}
