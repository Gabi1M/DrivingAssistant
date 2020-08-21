using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlVideoService : IVideoService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly VideoTableAdapter _tableAdapter = new VideoTableAdapter();

        //============================================================
        public MssqlVideoService()
        {
            _tableAdapter.Connection = new SqlConnection(Constants.ServerConstants.GetMssqlConnectionString());
        }

        //============================================================
        public async Task<IEnumerable<VideoRecording>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.Video);
                return _dataset.Video.AsEnumerable().Select(row => new VideoRecording
                {
                    Id = row.Id,
                    ProcessedId = row.ProcessedId,
                    SessionId = row.SessionId,
                    Filepath = row.Filepath,
                    Source = row.Source,
                    Description = row.Description,
                    DateAdded = row.DateAdded
                });
            });
        }

        //============================================================
        public async Task<VideoRecording> GetById(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Video_By_IdTableAdapter();
                tableAdapter.Fill(_dataset.Get_Video_By_Id, id);
                return _dataset.Get_Video_By_Id.AsEnumerable().Select(row => new VideoRecording
                {
                    Id = row.Id,
                    ProcessedId = row.ProcessedId,
                    SessionId = row.SessionId,
                    Filepath = row.Filepath,
                    Source = row.Source,
                    Description = row.Description,
                    DateAdded = row.DateAdded
                }).First();
            });
        }

        //============================================================
        public async Task<VideoRecording> GetByProcessedId(long processedId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Video_By_Processed_IdTableAdapter();
                tableAdapter.Fill(_dataset.Get_Video_By_Processed_Id, processedId);
                return _dataset.Get_Video_By_Processed_Id.AsEnumerable().Select(row => new VideoRecording
                {
                    Id = row.Id,
                    ProcessedId = row.ProcessedId,
                    SessionId = row.SessionId,
                    Filepath = row.Filepath,
                    Source = row.Source,
                    Description = row.Description,
                    DateAdded = row.DateAdded
                }).First();
            });
        }

        //============================================================
        public async Task<IEnumerable<VideoRecording>> GetBySession(long sessionId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Videos_By_SessionTableAdapter();
                tableAdapter.Fill(_dataset.Get_Videos_By_Session, sessionId);
                return _dataset.Get_Videos_By_Session.AsEnumerable().Select(row => new VideoRecording
                {
                    Id = row.Id,
                    ProcessedId = row.ProcessedId,
                    SessionId = row.SessionId,
                    Filepath = row.Filepath,
                    Source = row.Source,
                    Description = row.Description,
                    DateAdded = row.DateAdded
                });
            });
        }

        //============================================================
        public async Task<IEnumerable<VideoRecording>> GetByUser(long userId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Videos_By_UserTableAdapter();
                tableAdapter.Fill(_dataset.Get_Videos_By_User, userId);
                return _dataset.Get_Videos_By_User.AsEnumerable().Select(row => new VideoRecording
                {
                    Id = row.Id,
                    ProcessedId = row.ProcessedId,
                    SessionId = row.SessionId,
                    Filepath = row.Filepath,
                    Source = row.Source,
                    Description = row.Description,
                    DateAdded = row.DateAdded
                });
            });
        }

        //============================================================
        public async Task<long> SetAsync(VideoRecording videoRecording)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(videoRecording.Id, videoRecording.ProcessedId, videoRecording.SessionId,
                    videoRecording.Filepath, videoRecording.Source, videoRecording.Description, videoRecording.DateAdded, ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(VideoRecording videoRecording)
        {
            await Task.Run(async () =>
            {
                _tableAdapter.Delete(videoRecording.Id);
                await Task.Delay(1000);
                try
                {
                    File.Delete(videoRecording.Filepath);
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
