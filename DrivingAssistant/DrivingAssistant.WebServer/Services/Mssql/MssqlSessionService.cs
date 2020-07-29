using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlSessionService : ISessionService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly SessionTableAdapter _tableAdapter = new SessionTableAdapter();

        //============================================================
        public MssqlSessionService()
        {
            _tableAdapter.Connection = new SqlConnection(Constants.ServerConstants.GetMssqlConnectionString());
        }

        //============================================================
        public async Task<IEnumerable<Session>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.Session);
                return _dataset.Session.AsEnumerable().Select(row => new Session
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    Name = row.Name,
                    StartDateTime = row.StartDateTime,
                    EndDateTime = row.EndDateTime,
                    StartLocation = row.StartLocation.StringToPoint(),
                    EndLocation = row.EndLocation.StringToPoint(),
                    Waypoints = row.Waypoints.StringToPointCollection(),
                    Processed = row.Processed,
                    DateAdded = row.DateAdded
                });
            });
        }

        //============================================================
        public async Task<Session> GetById(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Sessions_By_IdTableAdapter();
                tableAdapter.Fill(_dataset.Get_Sessions_By_Id, id);
                return _dataset.Get_Sessions_By_Id.AsEnumerable().Select(row => new Session
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    Name = row.Name,
                    StartDateTime = row.StartDateTime,
                    EndDateTime = row.EndDateTime,
                    StartLocation = row.StartLocation.StringToPoint(),
                    EndLocation = row.EndLocation.StringToPoint(),
                    Waypoints = row.Waypoints.StringToPointCollection(),
                    Processed = row.Processed,
                    DateAdded = row.DateAdded
                }).First();
            });
        }

        //============================================================
        public async Task<IEnumerable<Session>> GetByUser(long userId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Sessions_By_UserTableAdapter();
                tableAdapter.Fill(_dataset.Get_Sessions_By_User, userId);
                return _dataset.Get_Sessions_By_User.AsEnumerable().Select(row => new Session
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    Name = row.Name,
                    StartDateTime = row.StartDateTime,
                    EndDateTime = row.EndDateTime,
                    StartLocation = row.StartLocation.StringToPoint(),
                    EndLocation = row.EndLocation.StringToPoint(),
                    Waypoints = row.Waypoints.StringToPointCollection(),
                    Processed = row.Processed,
                    DateAdded = row.DateAdded
                });
            });
        }

        //============================================================
        public async Task<long> SetAsync(Session session)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(session.Id, session.UserId, session.Name, session.StartDateTime,
                    session.EndDateTime, session.StartLocation.PointToString(), session.EndLocation.PointToString(),
                    session.Waypoints.PointCollectionToString(), session.Processed, session.DateAdded, ref idOut);
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
