using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlUserSettingsService : UserSettingsService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlUserSettingsService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public override async Task<ICollection<UserSettings>> GetAsync()
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task<UserSettings> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task<long> SetAsync(UserSettings data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task UpdateAsync(UserSettings data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async Task DeleteAsync(UserSettings data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public override async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}
