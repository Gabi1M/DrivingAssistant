using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlMediaService : IMediaService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly MediaTableAdapter _tableAdapter = new MediaTableAdapter();

        //============================================================
        public MssqlMediaService(string connectionString)
        {
            _tableAdapter.Connection = new SqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<Media>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.Media);
                return _dataset.Media.AsEnumerable().Select(row => new Media
                {
                    Id = row.Id,
                    ProcessedId = row.ProcessedId,
                    SessionId = row.SessionId,
                    UserId = row.UserId,
                    Type = (MediaType) Enum.Parse(typeof(MediaType), row.Type),
                    Filepath = row.Filepath,
                    Source = row.Source,
                    Description = row.Description,
                    DateAdded = row.DateAdded
                }).ToList();
            });
        }

        //============================================================
        public async Task<long> SetAsync(Media media)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(media.Id, media.ProcessedId, media.SessionId, media.UserId, media.Type.ToString(),
                    media.Filepath, media.Source, media.Description, media.DateAdded, ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(Media media)
        {
            await Task.Run(async () =>
            {
                _tableAdapter.Delete(media.Id);
                await Task.Delay(1000);
                try
                {
                    File.Delete(media.Filepath);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            });
        }

        //============================================================
        public void Dispose()
        {
            _tableAdapter.Dispose();
            _dataset.Dispose();
        }
    }
}
