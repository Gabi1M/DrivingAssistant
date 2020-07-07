using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services
{
    public class UserService : IDisposable
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public UserService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        //============================================================
        public async Task<ICollection<User>> GetAsync()
        {
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.GetUsersCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var users = new List<User>();
            while (await result.ReadAsync())
            {
                users.Add(new User(
                    result["username"].ToString(),
                    result["password"].ToString(),
                    result["firstname"].ToString(),
                    result["lastname"].ToString(),
                    Convert.ToDateTime(result["datetime"].ToString()),
                    Convert.ToInt64(result["id"])));
            }

            return users;
        }

        //============================================================
        public async Task<long> SetAsync(User user)
        {
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.AddUserCommand, _connection);
            command.Parameters.AddWithValue("username", user.Username);
            command.Parameters.AddWithValue("password", user.Password);
            command.Parameters.AddWithValue("firstname", user.FirstName);
            command.Parameters.AddWithValue("lastname", user.LastName);
            command.Parameters.AddWithValue("datetime", user.JoinDate);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            return result;
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.DeleteUserCommand, _connection);
            command.Parameters.AddWithValue("id", id);
            await command.ExecuteNonQueryAsync();
        }

        //============================================================
        public async void Dispose()
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
