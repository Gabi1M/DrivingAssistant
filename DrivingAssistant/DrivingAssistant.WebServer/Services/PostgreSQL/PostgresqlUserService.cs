using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.PostgreSQL
{
    public class PostgresqlUserService : IUserService
    {
        //============================================================
        public async Task<IEnumerable<User>> GetAsync()
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetAllUsers, connection);
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
                        JoinDate = Convert.ToDateTime(result["join_date"])
                    });
                }

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task<User> GetById(long id)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetUserById, connection);
                command.Parameters.AddWithValue("id", id);
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
                        JoinDate = Convert.ToDateTime(result["join_date"])
                    });
                }

                return users.Single();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task<long> SetAsync(User user)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                if (user.Id == -1)
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.SetUser, connection);
                    command.Parameters.AddWithValue("username", user.Username);
                    command.Parameters.AddWithValue("password", user.Password);
                    command.Parameters.AddWithValue("first_name", user.FirstName);
                    command.Parameters.AddWithValue("last_name", user.LastName);
                    command.Parameters.AddWithValue("email", user.Email);
                    command.Parameters.AddWithValue("join_date", user.JoinDate);
                    var result = Convert.ToInt64(await command.ExecuteScalarAsync());
                    return result;
                }
                else
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.UpdateUser, connection);
                    command.Parameters.AddWithValue("id", user.Id);
                    command.Parameters.AddWithValue("username", user.Username);
                    command.Parameters.AddWithValue("password", user.Password);
                    command.Parameters.AddWithValue("first_name", user.FirstName);
                    command.Parameters.AddWithValue("last_name", user.LastName);
                    command.Parameters.AddWithValue("email", user.Email);
                    command.Parameters.AddWithValue("join_date", user.JoinDate);
                    await command.ExecuteNonQueryAsync();
                    return user.Id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task DeleteAsync(User user)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.DeleteUser, connection);
                command.Parameters.AddWithValue("id", user.Id);
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public void Dispose()
        {
            
        }
    }
}
