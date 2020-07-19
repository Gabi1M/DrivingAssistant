using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlUserService : UserService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlUserService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public override async Task<ICollection<User>> GetAsync()
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetUsersCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var users = new List<User>();
            while (await result.ReadAsync())
            {
                users.Add(new User(result["username"].ToString(), result["password"].ToString(),
                    result["firstname"].ToString(), result["lastname"].ToString(), result["role"].ToString(),
                    Convert.ToDateTime(result["joindate"].ToString()), Convert.ToInt64(result["id"])));
            }

            await _connection.CloseAsync();
            return users;
        }

        //============================================================
        public override async Task<User> GetByIdAsync(long id)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetUserByIdCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            await result.ReadAsync();
            var user = new User(result["username"].ToString(), result["password"].ToString(),
                result["firstname"].ToString(), result["lastname"].ToString(), result["role"].ToString(),
                Convert.ToDateTime(result["joindate"].ToString()), Convert.ToInt64(result["id"]));
            await _connection.CloseAsync();
            return user;
        }

        //============================================================
        public override async Task<long> SetAsync(User user)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.AddUserCommand, _connection);
            command.Parameters.AddWithValue("username", user.Username);
            command.Parameters.AddWithValue("password", user.Password);
            command.Parameters.AddWithValue("firstname", user.FirstName);
            command.Parameters.AddWithValue("lastname", user.LastName);
            command.Parameters.AddWithValue("role", user.Role.ToString());
            command.Parameters.AddWithValue("joindate", user.JoinDate);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public override async Task UpdateAsync(User user)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.UpdateUserCommand, _connection);
            command.Parameters.AddWithValue("id", user.Id);
            command.Parameters.AddWithValue("username", user.Username);
            command.Parameters.AddWithValue("password", user.Password);
            command.Parameters.AddWithValue("firstname", user.FirstName);
            command.Parameters.AddWithValue("lastname", user.LastName);
            command.Parameters.AddWithValue("role", user.Role.ToString());
            command.Parameters.AddWithValue("joindate", user.JoinDate);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }
         
        //============================================================
        public override async Task DeleteAsync(User user)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.DeleteUserCommand, _connection);
            command.Parameters.AddWithValue("id", user.Id);
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
