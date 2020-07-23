using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlSessionService : ISessionService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlSessionService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<Session>> GetAsync()
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetSessionsCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var sessions = new List<Session>();
            while (await result.ReadAsync())
            {
                sessions.Add(new Session
                {
                    Id = Convert.ToInt64(result["id"]),
                    UserId = Convert.ToInt64(result["user_id"]),
                    StartDateTime = Convert.ToDateTime(result["start_date_time"]),
                    EndDateTime = Convert.ToDateTime(result["end_date_time"]),
                    StartPoint = result["start_point"].ToString().StringToPoint(),
                    EndPoint = result["end_point"].ToString().StringToPoint(),
                    IntermediatePoints = result["intermediate_points"].ToString().StringToPointCollection(),
                    Processed = Convert.ToBoolean(result["processed"])
                });
            }

            await _connection.CloseAsync();
            return sessions;
        }

        //============================================================
        public async Task<long> SetAsync(Session session)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.AddSessionCommand, _connection);
            command.Parameters.AddWithValue("user_id", session.UserId);
            command.Parameters.AddWithValue("description", session.Description);
            command.Parameters.AddWithValue("start_date_time", session.StartDateTime);
            command.Parameters.AddWithValue("end_date_time", session.EndDateTime);
            command.Parameters.AddWithValue("start_point", session.StartPoint.PointToString());
            command.Parameters.AddWithValue("end_point", session.EndPoint.PointToString());
            command.Parameters.AddWithValue("intermediate_points", session.IntermediatePoints.PointCollectionToString());
            command.Parameters.AddWithValue("processed", session.Processed);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public async Task DeleteAsync(Session session)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.DeleteSessionCommand, _connection);
            command.Parameters.AddWithValue("id", session.Id);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}
