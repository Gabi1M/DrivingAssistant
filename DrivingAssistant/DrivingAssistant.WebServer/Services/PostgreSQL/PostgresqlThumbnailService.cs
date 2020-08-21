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
    public class PostgresqlThumbnailService : IThumbnailService
    {
        //============================================================
        public async Task<IEnumerable<Thumbnail>> GetAsync()
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetAllThumbnails, connection);
                var result = await command.ExecuteReaderAsync();
                var thumbnails = new List<Thumbnail>();
                while (await result.ReadAsync())
                {
                    thumbnails.Add(new Thumbnail
                    {
                        Id = Convert.ToInt64(result["id"]),
                        VideoId = Convert.ToInt64(result["video_id"]),
                        Filepath = result["filepath"].ToString()
                    });
                }

                return thumbnails;
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
        public async Task<Thumbnail> GetById(long id)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetThumbnailById, connection);
                command.Parameters.AddWithValue("id", id);
                var result = await command.ExecuteReaderAsync();
                var thumbnails = new List<Thumbnail>();
                while (await result.ReadAsync())
                {
                    thumbnails.Add(new Thumbnail
                    {
                        Id = Convert.ToInt64(result["id"]),
                        VideoId = Convert.ToInt64(result["video_id"]),
                        Filepath = result["filepath"].ToString()
                    });
                }

                return thumbnails.Single();
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
        public async Task<Thumbnail> GetByVideo(long videoId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetThumbnailByVideo, connection);
                command.Parameters.AddWithValue("video_id", videoId);
                var result = await command.ExecuteReaderAsync();
                var thumbnails = new List<Thumbnail>();
                while (await result.ReadAsync())
                {
                    thumbnails.Add(new Thumbnail
                    {
                        Id = Convert.ToInt64(result["id"]),
                        VideoId = Convert.ToInt64(result["video_id"]),
                        Filepath = result["filepath"].ToString()
                    });
                }

                return thumbnails.Single();
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
        public async Task<long> SetAsync(Thumbnail thumbnail)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                if (thumbnail.Id == -1)
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.SetThumbnail, connection);
                    command.Parameters.AddWithValue("video_id", thumbnail.VideoId);
                    command.Parameters.AddWithValue("filepath", thumbnail.Filepath);
                    var result = Convert.ToInt64(await command.ExecuteScalarAsync());
                    await connection.CloseAsync();
                    return result;
                }
                else
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.UpdateThumbnail, connection);
                    command.Parameters.AddWithValue("id", thumbnail.Id);
                    command.Parameters.AddWithValue("video_id", thumbnail.VideoId);
                    command.Parameters.AddWithValue("filepath", thumbnail.Filepath);
                    await command.ExecuteNonQueryAsync();
                    return thumbnail.Id;
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
        public async Task DeleteAsync(Thumbnail thumbnail)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.DeleteThumbnail, connection);
                command.Parameters.AddWithValue("id", thumbnail.Id);
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
