using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services
{
    public class VideoService : GenericService<Video>
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public VideoService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public override async Task<ICollection<Video>> GetAsync()
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.GetVideosCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var videos = new List<Video>();
            while (await result.ReadAsync())
            {
                videos.Add(new Video(
                    result["filepath"].ToString(),
                    Convert.ToInt32(result["width"]),
                    Convert.ToInt32(result["height"]),
                    Convert.ToInt32(result["fps"]),
                    result["format"].ToString(),
                    result["source"].ToString(),
                    Convert.ToDateTime(result["datetime"]),
                    Convert.ToInt64(result["processed_id"]),
                    Convert.ToInt64(result["id"])));
            }

            await _connection.CloseAsync();
            return videos;
        }

        //============================================================
        public override async Task<long> SetAsync(Video video)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.AddVideoCommand, _connection);
            command.Parameters.AddWithValue("processed_id", video.ProcessedId);
            command.Parameters.AddWithValue("filepath", video.Filepath);
            command.Parameters.AddWithValue("width", video.Width);
            command.Parameters.AddWithValue("height", video.Height);
            command.Parameters.AddWithValue("fps", video.Fps);
            command.Parameters.AddWithValue("format", video.Format);
            command.Parameters.AddWithValue("source", video.Source);
            command.Parameters.AddWithValue("datetime", DateTime.Now);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public override async Task UpdateAsync(Video video)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.UpdateVideoCommand, _connection);
            command.Parameters.AddWithValue("id", video.Id);
            command.Parameters.AddWithValue("processed_id", video.ProcessedId);
            command.Parameters.AddWithValue("filepath", video.Filepath);
            command.Parameters.AddWithValue("width", video.Width);
            command.Parameters.AddWithValue("height", video.Height);
            command.Parameters.AddWithValue("fps", video.Fps);
            command.Parameters.AddWithValue("format", video.Format);
            command.Parameters.AddWithValue("source", video.Source);
            command.Parameters.AddWithValue("datetime", DateTime.Now);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public override async Task DeleteAsync(long id)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.DeleteVideoCommand, _connection);
            command.Parameters.AddWithValue("id", id);
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
