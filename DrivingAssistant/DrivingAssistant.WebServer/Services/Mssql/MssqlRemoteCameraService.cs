using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlRemoteCameraService : IRemoteCameraService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly RemoteCameraTableAdapter _tableAdapter = new RemoteCameraTableAdapter();

        //============================================================
        public MssqlRemoteCameraService()
        {
            _tableAdapter.Connection = new SqlConnection(Constants.ServerConstants.GetMssqlConnectionString());
        }

        //============================================================
        public async Task<IEnumerable<RemoteCamera>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.RemoteCamera);
                return _dataset.RemoteCamera.AsEnumerable().Select(row => new RemoteCamera
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    DestinationSessionId = row.SessionId,
                    Host = row.Host,
                    Username = row.Username,
                    Password = Crypto.DecryptAes(row.Password),
                    VideoLength = row.VideoLength,
                    AutoProcessSession = row.AutoProcessSession,
                    AutoProcessSessionType = Enum.Parse<ProcessingAlgorithmType>(row.AutoProcessSessionType)
                });
            });
        }

        //============================================================
        public async Task<RemoteCamera> GetById(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_RemoteCamera_By_IdTableAdapter();
                tableAdapter.Fill(_dataset.Get_RemoteCamera_By_Id, id);
                return _dataset.Get_RemoteCamera_By_Id.AsEnumerable().Select(row => new RemoteCamera
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    DestinationSessionId = row.SessionId,
                    Host = row.Host,
                    Username = row.Username,
                    Password = Crypto.DecryptAes(row.Password),
                    VideoLength = row.VideoLength,
                    AutoProcessSession = row.AutoProcessSession,
                    AutoProcessSessionType = Enum.Parse<ProcessingAlgorithmType>(row.AutoProcessSessionType)
                }).First();
            });
        }

        //============================================================
        public async Task<RemoteCamera> GetByUser(long userId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_RemoteCamera_By_UserTableAdapter();
                tableAdapter.Fill(_dataset.Get_RemoteCamera_By_User, userId);
                return _dataset.Get_RemoteCamera_By_User.AsEnumerable().Select(row => new RemoteCamera
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    DestinationSessionId = row.SessionId,
                    Host = row.Host,
                    Username = row.Username,
                    Password = Crypto.DecryptAes(row.Password),
                    VideoLength = row.VideoLength,
                    AutoProcessSession = row.AutoProcessSession,
                    AutoProcessSessionType = Enum.Parse<ProcessingAlgorithmType>(row.AutoProcessSessionType)
                }).First();
            });
        }

        //============================================================
        public async Task<long> SetAsync(RemoteCamera remoteCamera)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(remoteCamera.Id, remoteCamera.UserId, remoteCamera.DestinationSessionId,
                    remoteCamera.Host, remoteCamera.Username,
                    Crypto.EncryptAes(remoteCamera.Password), remoteCamera.VideoLength, remoteCamera.AutoProcessSession,
                    remoteCamera.AutoProcessSessionType.ToString(), ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(RemoteCamera remoteCamera)
        {
            await Task.Run(() =>
            {
                _tableAdapter.Delete(remoteCamera.Id);
            });
        }

        //============================================================
        public void Dispose()
        {
            _dataset.Dispose();
        }
    }
}
