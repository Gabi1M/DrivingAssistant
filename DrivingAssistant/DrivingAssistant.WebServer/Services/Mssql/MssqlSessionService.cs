using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlSessionService : ISessionService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly SessionTableAdapter _tableAdapter = new SessionTableAdapter();

        //============================================================
        public MssqlSessionService(string connectionString)
        {
            _tableAdapter.Connection = new SqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<Session>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.Session);
                return _dataset.Session.AsEnumerable().Select(row => new Session
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    Description = row.Description,
                    StartDateTime = row.StartDateTime,
                    EndDateTime = row.EndDateTime,
                    StartPoint = row.StartPoint.StringToPoint(),
                    EndPoint = row.EndPoint.StringToPoint(),
                    IntermediatePoints = row.IntermediatePoints.StringToPointCollection(),
                    Processed = row.Processed
                }).ToList();
            });
        }

        //============================================================
        public async Task<long> SetAsync(Session session)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(session.Id, session.UserId, session.Description, session.StartDateTime,
                    session.EndDateTime, session.StartPoint.PointToString(), session.EndPoint.PointToString(),
                    session.IntermediatePoints.PointCollectionToString(), session.Processed, ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(Session session)
        {
            await Task.Run(() =>
            {
                _tableAdapter.Delete(session.Id);
            });
        }

        //============================================================
        public void Dispose()
        {
            _tableAdapter.Dispose();
            _dataset.Dispose();
        }
    }
}
