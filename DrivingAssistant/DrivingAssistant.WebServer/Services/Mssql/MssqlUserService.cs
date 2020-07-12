using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlUserService : UserService
    {
        //============================================================
        public override async Task<ICollection<User>> GetAsync()
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task<User> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task<long> SetAsync(User data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task UpdateAsync(User data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task DeleteAsync(User data)
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
