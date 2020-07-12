using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlSessionService : SessionService
    {
        //============================================================
        public override async Task<ICollection<Session>> GetAsync()
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task<Session> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task<long> SetAsync(Session data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task UpdateAsync(Session data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task DeleteAsync(Session data)
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
