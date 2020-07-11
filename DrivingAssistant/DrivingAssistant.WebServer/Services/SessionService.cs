using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services
{
    public class SessionService : GenericService<Session>
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public SessionService(string connectionString)
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
                sessions.Add(new Session(Convert.ToDateTime(result["startdatetime"]),
                    Convert.ToDateTime(result["enddatetime"]),
                    new Coordinates
                    {
                        X = Convert.ToDouble(result["startx"]),
                        Y = Convert.ToDouble(result["starty"])
                    }, 
                    new Coordinates
                    {
                        X = Convert.ToDouble(result["endx"]),
                        Y = Convert.ToDouble(result["endy"])
                    },
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
            var session = new Session(Convert.ToDateTime(result["startdatetime"]),
                Convert.ToDateTime(result["enddatetime"]),
                new Coordinates
                {
                    X = Convert.ToDouble(result["startx"]),
                    Y = Convert.ToDouble(result["starty"])
                },
                new Coordinates
                {
                    X = Convert.ToDouble(result["endx"]),
                    Y = Convert.ToDouble(result["endy"])
                },
                Convert.ToInt64(result["id"]));
            await _connection.CloseAsync();
            return session;
        }

        //============================================================
        public override async Task<long> SetAsync(Session session)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.AddSessionCommand, _connection);
            command.Parameters.AddWithValue("startdatetime", session.StartDateTime);
            command.Parameters.AddWithValue("enddatetime", session.EndDateTime);
            command.Parameters.AddWithValue("startx", session.StartCoordinates.X);
            command.Parameters.AddWithValue("starty", session.StartCoordinates.Y);
            command.Parameters.AddWithValue("endx", session.EndCoordinates.X);
            command.Parameters.AddWithValue("endy", session.EndCoordinates.Y);
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
            command.Parameters.AddWithValue("startdatetime", session.StartDateTime);
            command.Parameters.AddWithValue("enddatetime", session.EndDateTime);
            command.Parameters.AddWithValue("startx", session.StartCoordinates.X);
            command.Parameters.AddWithValue("starty", session.StartCoordinates.Y);
            command.Parameters.AddWithValue("endx", session.EndCoordinates.X);
            command.Parameters.AddWithValue("endy", session.EndCoordinates.Y);
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
