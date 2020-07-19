using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlSessionService : SessionService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlSessionService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public override async Task<ICollection<Session>> GetAsync()
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetSessionsCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var sessions = new List<Session>();
            while (await result.ReadAsync())
            {
                var session = new Session(result["description"].ToString(),
                    Convert.ToDateTime(result["start_date_time"]),
                    Convert.ToDateTime(result["end_date_time"]),
                    result["start_point"].ToString().StringToPoint(),
                    result["end_point"].ToString().StringToPoint(),
                    result["intermediate_points"].ToString().StringToPointCollection(),
                    Convert.ToInt64(result["id"]),
                    Convert.ToInt64(result["user_id"]));
                sessions.Add(session);
            }

            await _connection.CloseAsync();
            return sessions;
        }

        //============================================================
        public override async Task<Session> GetByIdAsync(long id)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetSessionByIdCommand, _connection);
            command.Parameters.AddWithValue("id", id);
            var result = await command.ExecuteReaderAsync();
            await result.ReadAsync();
            var session = new Session(result["description"].ToString(),
                Convert.ToDateTime(result["start_date_time"]),
                Convert.ToDateTime(result["end_date_time"]),
                result["start_point"].ToString().StringToPoint(),
                result["end_point"].ToString().StringToPoint(),
                result["intermediate_points"].ToString().StringToPointCollection(),
                Convert.ToInt64(result["id"]),
                Convert.ToInt64(result["user_id"]));
            await _connection.CloseAsync();
            return session;
        }

        //============================================================
        public override async Task<long> SetAsync(Session session)
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
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public override async Task UpdateAsync(Session session)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.UpdateSessionCommand, _connection);
            command.Parameters.AddWithValue("id", session.Id);
            command.Parameters.AddWithValue("user_id", session.UserId);
            command.Parameters.AddWithValue("description", session.Description);
            command.Parameters.AddWithValue("start_date_time", session.StartDateTime);
            command.Parameters.AddWithValue("end_date_time", session.EndDateTime);
            command.Parameters.AddWithValue("start_point", session.StartPoint.PointToString());
            command.Parameters.AddWithValue("end_point", session.EndPoint.PointToString());
            command.Parameters.AddWithValue("intermediate_points", session.IntermediatePoints.PointCollectionToString());
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public override async Task DeleteAsync(Session session)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.DeleteSessionCommand, _connection);
            command.Parameters.AddWithValue("id", session.Id);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public override async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}
