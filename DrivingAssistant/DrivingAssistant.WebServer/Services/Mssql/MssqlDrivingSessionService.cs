using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlDrivingSessionService : IDrivingSessionService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly DrivingSessionTableAdapter _tableAdapter = new DrivingSessionTableAdapter();

        //============================================================
        public MssqlDrivingSessionService()
        {
            _tableAdapter.Connection = new SqlConnection(Constants.ServerConstants.GetMssqlConnectionString());
        }

        //============================================================
        public async Task<IEnumerable<DrivingSession>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.DrivingSession);
                return _dataset.DrivingSession.AsEnumerable().Select(row => new DrivingSession
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    Name = row.Name,
                    StartDateTime = row.StartDateTime,
                    EndDateTime = row.EndDateTime,
                    StartLocation = row.StartLocation.StringToPoint(),
                    EndLocation = row.EndLocation.StringToPoint(),
                    Waypoints = row.Waypoints.StringToPointCollection(),
                    Status = Enum.Parse<SessionStatus>(row.Status),
                    DateAdded = row.DateAdded
                });
            });
        }

        //============================================================
        public async Task<DrivingSession> GetById(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_DrivingSession_By_IdTableAdapter();
                tableAdapter.Fill(_dataset.Get_DrivingSession_By_Id, id);
                return _dataset.Get_DrivingSession_By_Id.AsEnumerable().Select(row => new DrivingSession
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    Name = row.Name,
                    StartDateTime = row.StartDateTime,
                    EndDateTime = row.EndDateTime,
                    StartLocation = row.StartLocation.StringToPoint(),
                    EndLocation = row.EndLocation.StringToPoint(),
                    Waypoints = row.Waypoints.StringToPointCollection(),
                    Status = Enum.Parse<SessionStatus>(row.Status),
                    DateAdded = row.DateAdded
                }).First();
            });
        }

        //============================================================
        public async Task<IEnumerable<DrivingSession>> GetByUser(long userId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_DrivingSession_By_UserTableAdapter();
                tableAdapter.Fill(_dataset.Get_DrivingSession_By_User, userId);
                return _dataset.Get_DrivingSession_By_User.AsEnumerable().Select(row => new DrivingSession
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    Name = row.Name,
                    StartDateTime = row.StartDateTime,
                    EndDateTime = row.EndDateTime,
                    StartLocation = row.StartLocation.StringToPoint(),
                    EndLocation = row.EndLocation.StringToPoint(),
                    Waypoints = row.Waypoints.StringToPointCollection(),
                    Status = Enum.Parse<SessionStatus>(row.Status),
                    DateAdded = row.DateAdded
                });
            });
        }

        //============================================================
        public async Task<long> SetAsync(DrivingSession drivingSession)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(drivingSession.Id, drivingSession.UserId, drivingSession.Name, drivingSession.StartDateTime,
                    drivingSession.EndDateTime, drivingSession.StartLocation.PointToString(), drivingSession.EndLocation.PointToString(),
                    drivingSession.Waypoints.PointCollectionToString(), drivingSession.Status.ToString(), drivingSession.DateAdded,
                    ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(DrivingSession drivingSession)
        {
            await Task.Run(() =>
            {
                _tableAdapter.Delete(drivingSession.Id);
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
