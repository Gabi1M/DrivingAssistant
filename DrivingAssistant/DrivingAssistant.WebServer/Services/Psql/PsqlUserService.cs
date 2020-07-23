using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlUserService : IUserService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlUserService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<User>> GetAsync()
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetUsersCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var users = new List<User>();
            while (await result.ReadAsync())
            {
                users.Add(new User
                {
                    Id = Convert.ToInt64(result["id"]),
                    Username = result["username"].ToString(),
                    Password = result["password"].ToString(),
                    FirstName = result["first_name"].ToString(),
                    LastName = result["last_name"].ToString(),
                    Email = result["email"].ToString(),
                    Role = (UserRole) Enum.Parse(typeof(UserRole), result["role"].ToString()!),
                    JoinDate = Convert.ToDateTime(result["join_date"])
                });
            }

            await _connection.CloseAsync();
            return users;
        }

        //============================================================
        public async Task<long> SetAsync(User user)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.AddUserCommand, _connection);
            command.Parameters.AddWithValue("username", user.Username);
            command.Parameters.AddWithValue("password", user.Password);
            command.Parameters.AddWithValue("firstname", user.FirstName);
            command.Parameters.AddWithValue("lastname", user.LastName);
            command.Parameters.AddWithValue("email", user.Email);
            command.Parameters.AddWithValue("role", user.Role.ToString());
            command.Parameters.AddWithValue("joindate", user.JoinDate);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public async Task DeleteAsync(User user)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.DeleteUserCommand, _connection);
            command.Parameters.AddWithValue("id", user.Id);
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
