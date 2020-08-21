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
    public class PostgresqlVideoService : IVideoService
    {
        //============================================================
        public async Task<IEnumerable<VideoRecording>> GetAsync()
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetAllVideos, connection);
                var result = await command.ExecuteReaderAsync();
                var videos = new List<VideoRecording>();
                while (await result.ReadAsync())
                {
                    videos.Add(new VideoRecording
                    {
                        Id = Convert.ToInt64(result["id"]),
                        ProcessedId = Convert.ToInt64(result["processed_id"]),
                        SessionId = Convert.ToInt64(result["session_id"]),
                        Filepath = result["filepath"].ToString(),
                        Source = result["source"].ToString(),
                        Description = result["description"].ToString(),
                        DateAdded = Convert.ToDateTime(result["date_added"])
                    });
                }

                return videos;
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
        public async Task<VideoRecording> GetById(long id)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetVideoById, connection);
                command.Parameters.AddWithValue("id", id);
                var result = await command.ExecuteReaderAsync();
                var videos = new List<VideoRecording>();
                while (await result.ReadAsync())
                {
                    videos.Add(new VideoRecording
                    {
                        Id = Convert.ToInt64(result["id"]),
                        ProcessedId = Convert.ToInt64(result["processed_id"]),
                        SessionId = Convert.ToInt64(result["session_id"]),
                        Filepath = result["filepath"].ToString(),
                        Source = result["source"].ToString(),
                        Description = result["description"].ToString(),
                        DateAdded = Convert.ToDateTime(result["date_added"])
                    });
                }

                return videos.First();
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
        public async Task<VideoRecording> GetByProcessedId(long processedId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetVideoByProcessedId, connection);
                command.Parameters.AddWithValue("processed_id", processedId);
                var result = await command.ExecuteReaderAsync();
                var videos = new List<VideoRecording>();
                while (await result.ReadAsync())
                {
                    videos.Add(new VideoRecording
                    {
                        Id = Convert.ToInt64(result["id"]),
                        ProcessedId = Convert.ToInt64(result["processed_id"]),
                        SessionId = Convert.ToInt64(result["session_id"]),
                        Filepath = result["filepath"].ToString(),
                        Source = result["source"].ToString(),
                        Description = result["description"].ToString(),
                        DateAdded = Convert.ToDateTime(result["date_added"])
                    });
                }

                return videos.First();
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
        public async Task<IEnumerable<VideoRecording>> GetBySession(long sessionId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetVideosBySession, connection);
                command.Parameters.AddWithValue("session_id", sessionId);
                var result = await command.ExecuteReaderAsync();
                var videos = new List<VideoRecording>();
                while (await result.ReadAsync())
                {
                    videos.Add(new VideoRecording
                    {
                        Id = Convert.ToInt64(result["id"]),
                        ProcessedId = Convert.ToInt64(result["processed_id"]),
                        SessionId = Convert.ToInt64(result["session_id"]),
                        Filepath = result["filepath"].ToString(),
                        Source = result["source"].ToString(),
                        Description = result["description"].ToString(),
                        DateAdded = Convert.ToDateTime(result["date_added"])
                    });
                }

                return videos;
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
        public async Task<IEnumerable<VideoRecording>> GetByUser(long userId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetVideosByUser, connection);
                command.Parameters.AddWithValue("user_id", userId);
                var result = await command.ExecuteReaderAsync();
                var videos = new List<VideoRecording>();
                while (await result.ReadAsync())
                {
                    videos.Add(new VideoRecording
                    {
                        Id = Convert.ToInt64(result["id"]),
                        ProcessedId = Convert.ToInt64(result["processed_id"]),
                        SessionId = Convert.ToInt64(result["session_id"]),
                        Filepath = result["filepath"].ToString(),
                        Source = result["source"].ToString(),
                        Description = result["description"].ToString(),
                        DateAdded = Convert.ToDateTime(result["date_added"])
                    });
                }

                return videos;
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
        public async Task<long> SetAsync(VideoRecording videoRecording)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                if (videoRecording.Id == -1)
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.SetVideo, connection);
                    command.Parameters.AddWithValue("processed_id", videoRecording.ProcessedId);
                    command.Parameters.AddWithValue("session_id", videoRecording.SessionId);
                    command.Parameters.AddWithValue("filepath", videoRecording.Filepath);
                    command.Parameters.AddWithValue("source", videoRecording.Source);
                    command.Parameters.AddWithValue("description", videoRecording.Description);
                    command.Parameters.AddWithValue("date_added", videoRecording.DateAdded);
                    var result = Convert.ToInt64(await command.ExecuteScalarAsync());
                    return result;
                }
                else
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.UpdateVideo, connection);
                    command.Parameters.AddWithValue("id", videoRecording.Id);
                    command.Parameters.AddWithValue("processed_id", videoRecording.ProcessedId);
                    command.Parameters.AddWithValue("session_id", videoRecording.SessionId);
                    command.Parameters.AddWithValue("filepath", videoRecording.Filepath);
                    command.Parameters.AddWithValue("source", videoRecording.Source);
                    command.Parameters.AddWithValue("description", videoRecording.Description);
                    command.Parameters.AddWithValue("date_added", videoRecording.DateAdded);
                    await command.ExecuteNonQueryAsync();
                    return videoRecording.Id;
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
        public async Task DeleteAsync(VideoRecording videoRecording)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.DeleteVideo, connection);
                command.Parameters.AddWithValue("id", videoRecording.Id);
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
