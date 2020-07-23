using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlReportService : IReportService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlReportService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<Report>> GetAsync()
        {
            throw new NotImplementedException();
        }

        //============================================================
        public async Task<long> SetAsync(Report data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public async Task DeleteAsync(Report data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
