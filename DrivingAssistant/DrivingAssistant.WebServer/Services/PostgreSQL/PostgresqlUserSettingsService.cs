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
    public class PostgresqlUserSettingsService : IUserSettingsService
    {
        //============================================================
        public async Task<IEnumerable<UserSettings>> GetAsync()
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetAllUserSettings, connection);
                var result = await command.ExecuteReaderAsync();
                var userSettings = new List<UserSettings>();
                while (await result.ReadAsync())
                {
                    userSettings.Add(new UserSettings
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        CameraSessionId = Convert.ToInt64(result["camera_session_id"]),
                        CameraHost = result["camera_host"].ToString(),
                        CameraUsername = result["camera_username"].ToString(),
                        CameraPassword = result["camera_password"].ToString()
                    });
                }

                return userSettings;
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
        public async Task<UserSettings> GetById(long id)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetUserSettingsById, connection);
                command.Parameters.AddWithValue("id", id);
                var result = await command.ExecuteReaderAsync();
                var userSettings = new List<UserSettings>();
                while (await result.ReadAsync())
                {
                    userSettings.Add(new UserSettings
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        CameraSessionId = Convert.ToInt64(result["camera_session_id"]),
                        CameraHost = result["camera_host"].ToString(),
                        CameraUsername = result["camera_username"].ToString(),
                        CameraPassword = result["camera_password"].ToString()
                    });
                }

                return userSettings.Single();
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
        public async Task<UserSettings> GetByUser(long userId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetUserSettingsByUser, connection);
                command.Parameters.AddWithValue("user_id", userId);
                var result = await command.ExecuteReaderAsync();
                var userSettings = new List<UserSettings>();
                while (await result.ReadAsync())
                {
                    userSettings.Add(new UserSettings
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        CameraSessionId = Convert.ToInt64(result["camera_session_id"]),
                        CameraHost = result["camera_host"].ToString(),
                        CameraUsername = result["camera_username"].ToString(),
                        CameraPassword = result["camera_password"].ToString()
                    });
                }

                return userSettings.Single();
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
        public async Task<long> SetAsync(UserSettings userSettings)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                if (userSettings.Id == -1)
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.SetUserSettings, connection);
                    command.Parameters.AddWithValue("user_id", userSettings.UserId);
                    command.Parameters.AddWithValue("camera_session_id", userSettings.CameraSessionId);
                    command.Parameters.AddWithValue("camera_host", userSettings.CameraHost);
                    command.Parameters.AddWithValue("camera_username", userSettings.CameraUsername);
                    command.Parameters.AddWithValue("camera_password", userSettings.CameraPassword);
                    var result = Convert.ToInt64(await command.ExecuteScalarAsync());
                    return result;
                }
                else
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.UpdateUserSettings, connection);
                    command.Parameters.AddWithValue("id", userSettings.Id);
                    command.Parameters.AddWithValue("user_id", userSettings.UserId);
                    command.Parameters.AddWithValue("camera_session_id", userSettings.CameraSessionId);
                    command.Parameters.AddWithValue("camera_host", userSettings.CameraHost);
                    command.Parameters.AddWithValue("camera_username", userSettings.CameraUsername);
                    command.Parameters.AddWithValue("camera_password", userSettings.CameraPassword);
                    await command.ExecuteNonQueryAsync();
                    return userSettings.Id;
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
        public async Task DeleteAsync(UserSettings userSettings)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.DeleteUserSettings, connection);
                command.Parameters.AddWithValue("id", userSettings.Id);
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
