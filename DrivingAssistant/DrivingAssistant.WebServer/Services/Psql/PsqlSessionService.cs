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
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.GetSessionsCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var sessions = new List<Session>();
            while (await result.ReadAsync())
            {
                sessions.Add(new Session(result["description"].ToString(),
                    Convert.ToDateTime(result["startdatetime"]),
                    Convert.ToDateTime(result["enddatetime"]),
                    new Coordinates(Convert.ToSingle(result["startx"]), Convert.ToSingle(result["starty"])),
                new Coordinates(Convert.ToSingle(result["endx"]), Convert.ToSingle(result["endy"])),
                    Convert.ToInt64(result["id"])));
            }

            await _connection.CloseAsync();
            return sessions;
        }

        //============================================================
        public override async Task<Session> GetByIdAsync(long id)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.GetSessionByIdCommand, _connection);
            command.Parameters.AddWithValue("id", id);
            var result = await command.ExecuteReaderAsync();
            await result.ReadAsync();
            var session = new Session(result["description"].ToString(),
                Convert.ToDateTime(result["startdatetime"]),
                Convert.ToDateTime(result["enddatetime"]),
                new Coordinates(Convert.ToSingle(result["startx"]), Convert.ToSingle(result["starty"])),
                new Coordinates(Convert.ToSingle(result["endx"]), Convert.ToSingle(result["endy"])),
                Convert.ToInt64(result["id"]));
            await _connection.CloseAsync();
            return session;
        }

        //============================================================
        public override async Task<long> SetAsync(Session session)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.AddSessionCommand, _connection);
            command.Parameters.AddWithValue("user_id", session.UserId);
            command.Parameters.AddWithValue("description", session.Description);
            command.Parameters.AddWithValue("startdatetime", session.StartDateTime);
            command.Parameters.AddWithValue("enddatetime", session.EndDateTime);
            command.Parameters.AddWithValue("startx", session.StartCoordinates.Latitude);
            command.Parameters.AddWithValue("starty", session.StartCoordinates.Longitude);
            command.Parameters.AddWithValue("endx", session.EndCoordinates.Latitude);
            command.Parameters.AddWithValue("endy", session.EndCoordinates.Longitude);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public override async Task UpdateAsync(Session session)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.UpdateSessionCommand, _connection);
            command.Parameters.AddWithValue("id", session.Id);
            command.Parameters.AddWithValue("user_id", session.UserId);
            command.Parameters.AddWithValue("description", session.Description);
            command.Parameters.AddWithValue("startdatetime", session.StartDateTime);
            command.Parameters.AddWithValue("enddatetime", session.EndDateTime);
            command.Parameters.AddWithValue("startx", session.StartCoordinates.Latitude);
            command.Parameters.AddWithValue("starty", session.StartCoordinates.Longitude);
            command.Parameters.AddWithValue("endx", session.EndCoordinates.Latitude);
            command.Parameters.AddWithValue("endy", session.EndCoordinates.Longitude);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public override async Task DeleteAsync(Session session)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.DeleteSessionCommand, _connection);
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
