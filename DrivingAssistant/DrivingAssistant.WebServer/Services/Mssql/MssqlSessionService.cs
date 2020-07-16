using System;
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
    public class MssqlSessionService : SessionService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly string _connectionString;

        //============================================================
        public MssqlSessionService(string connectionString)
        {
            _connectionString = connectionString;
        }

        //============================================================
        public override async Task<ICollection<Session>> GetAsync()
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_SessionsTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Get_Sessions);
                using DataTable dataTable = _dataset.Get_Sessions;
                var result = from DataRow row in dataTable.AsEnumerable()
                    select new Session(row["Description"].ToString(),
                        Convert.ToDateTime(row["StartDateTime"].ToString()),
                        Convert.ToDateTime(row["EndDateTime"].ToString()),
                        new Coordinates(Convert.ToSingle(row["StartLatitude"]),
                            Convert.ToSingle(row["StartLongitude"])),
                        new Coordinates(Convert.ToSingle(row["EndLatitude"]), 
                            Convert.ToSingle(row["EndLongitude"])),
                        Convert.ToInt64(row["Id"]), Convert.ToInt64(row["UserId"]));
                return result.ToList();
            });
        }

        //============================================================
        public override async Task<Session> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Session_By_idTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Get_Session_By_id, id);
                using DataTable dataTable = _dataset.Get_Session_By_id;
                var result = from DataRow row in dataTable.AsEnumerable()
                    select new Session(row["Description"].ToString(),
                        Convert.ToDateTime(row["StartDateTime"].ToString()),
                        Convert.ToDateTime(row["EndDateTime"].ToString()),
                        new Coordinates(Convert.ToSingle(row["StartLatitude"]), 
                            Convert.ToSingle(row["StartLongitude"])),
                        new Coordinates(Convert.ToSingle(row["EndLatitude"]), 
                            Convert.ToSingle(row["EndLongitude"])),
                        Convert.ToInt64(row["Id"]), Convert.ToInt64(row["UserId"]));
                return result.First();
            });
        }

        //============================================================
        public override async Task<long> SetAsync(Session session)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Set_SessionTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                long? idOut = 0;
                tableAdapter.Fill(_dataset.Set_Session, null, session.UserId, session.Description,
                    session.StartDateTime,
                    session.EndDateTime, session.StartCoordinates.Latitude, 
                    session.StartCoordinates.Longitude,
                    session.EndCoordinates.Latitude,
                    session.EndCoordinates.Longitude, ref idOut);
                return idOut.Value;
            });
        }

        //============================================================
        public override async Task UpdateAsync(Session session)
        {
            await Task.Run(() =>
            {
                using var tableAdapter = new Set_SessionTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                long? idOut = 0;
                tableAdapter.Fill(_dataset.Set_Session, session.Id, session.UserId, session.Description,
                    session.StartDateTime,
                    session.EndDateTime, session.StartCoordinates.Latitude, 
                    session.StartCoordinates.Longitude,
                    session.EndCoordinates.Latitude,
                    session.EndCoordinates.Longitude, ref idOut);
            });
        }

        //============================================================
        public override async Task DeleteAsync(Session session)
        {
            await Task.Run(() =>
            {
                using var tableAdapter = new Delete_SessionTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Delete_Session, session.Id);
            });
        }

        //============================================================
        public override void Dispose()
        {
            _dataset.Dispose();
        }
    }
}
