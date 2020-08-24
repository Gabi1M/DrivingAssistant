using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.PostgreSQL
{
    public class PostgresqlRemoteCameraService : IRemoteCameraService
    {
        //============================================================
        public async Task<IEnumerable<RemoteCamera>> GetAsync()
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetAllUserSettings, connection);
                var result = await command.ExecuteReaderAsync();
                var userSettings = new List<RemoteCamera>();
                while (await result.ReadAsync())
                {
                    userSettings.Add(new RemoteCamera
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        DestinationSessionId = Convert.ToInt64(result["session_id"]),
                        Host = result["host"].ToString(),
                        Username = result["username"].ToString(),
                        Password = result["password"].ToString(),
                        VideoLength = Convert.ToInt32(result["video_length"]),
                        AutoProcessSession = Convert.ToBoolean(result["auto_process_session"]),
                        AutoProcessSessionType = Enum.Parse<ProcessingAlgorithmType>(result["auto_process_session_type"].ToString()!)
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
        public async Task<RemoteCamera> GetById(long id)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetUserSettingsById, connection);
                command.Parameters.AddWithValue("id", id);
                var result = await command.ExecuteReaderAsync();
                var userSettings = new List<RemoteCamera>();
                while (await result.ReadAsync())
                {
                    userSettings.Add(new RemoteCamera
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        DestinationSessionId = Convert.ToInt64(result["session_id"]),
                        Host = result["host"].ToString(),
                        Username = result["username"].ToString(),
                        Password = result["password"].ToString(),
                        VideoLength = Convert.ToInt32(result["video_length"]),
                        AutoProcessSession = Convert.ToBoolean(result["auto_process_session"]),
                        AutoProcessSessionType = Enum.Parse<ProcessingAlgorithmType>(result["auto_process_session_type"].ToString()!)
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
        public async Task<RemoteCamera> GetByUser(long userId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetUserSettingsByUser, connection);
                command.Parameters.AddWithValue("user_id", userId);
                var result = await command.ExecuteReaderAsync();
                var userSettings = new List<RemoteCamera>();
                while (await result.ReadAsync())
                {
                    userSettings.Add(new RemoteCamera
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        DestinationSessionId = Convert.ToInt64(result["session_id"]),
                        Host = result["host"].ToString(),
                        Username = result["username"].ToString(),
                        Password = result["password"].ToString(),
                        VideoLength = Convert.ToInt32(result["video_length"]),
                        AutoProcessSession = Convert.ToBoolean(result["auto_process_session"]),
                        AutoProcessSessionType = Enum.Parse<ProcessingAlgorithmType>(result["auto_process_session_type"].ToString()!)
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
        public async Task<long> SetAsync(RemoteCamera remoteCamera)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                if (remoteCamera.Id == -1)
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.SetUserSettings, connection);
                    command.Parameters.AddWithValue("user_id", remoteCamera.UserId);
                    command.Parameters.AddWithValue("session_id", remoteCamera.DestinationSessionId);
                    command.Parameters.AddWithValue("host", remoteCamera.Host);
                    command.Parameters.AddWithValue("username", remoteCamera.Username);
                    command.Parameters.AddWithValue("password", remoteCamera.Password);
                    command.Parameters.AddWithValue("video_length", remoteCamera.VideoLength);
                    command.Parameters.AddWithValue("auto_process_session", remoteCamera.AutoProcessSession);
                    command.Parameters.AddWithValue("auto_process_session_type", remoteCamera.AutoProcessSessionType.ToString());
                    var result = Convert.ToInt64(await command.ExecuteScalarAsync());
                    return result;
                }
                else
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.UpdateUserSettings, connection);
                    command.Parameters.AddWithValue("id", remoteCamera.Id);
                    command.Parameters.AddWithValue("user_id", remoteCamera.UserId);
                    command.Parameters.AddWithValue("session_id", remoteCamera.DestinationSessionId);
                    command.Parameters.AddWithValue("host", remoteCamera.Host);
                    command.Parameters.AddWithValue("username", remoteCamera.Username);
                    command.Parameters.AddWithValue("password", remoteCamera.Password);
                    command.Parameters.AddWithValue("video_length", remoteCamera.VideoLength);
                    command.Parameters.AddWithValue("auto_process_session", remoteCamera.AutoProcessSession);
                    command.Parameters.AddWithValue("auto_process_session_type", remoteCamera.AutoProcessSessionType.ToString());
                    await command.ExecuteNonQueryAsync();
                    return remoteCamera.Id;
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
        public async Task DeleteAsync(RemoteCamera remoteCamera)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.DeleteUserSettings, connection);
                command.Parameters.AddWithValue("id", remoteCamera.Id);
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
