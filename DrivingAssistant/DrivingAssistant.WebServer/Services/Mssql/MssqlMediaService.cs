using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlMediaService : MediaService
    {
        //============================================================
        public override async Task<ICollection<Media>> GetAsync()
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task<Media> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task<long> SetAsync(Media data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task UpdateAsync(Media data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task DeleteAsync(Media data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
