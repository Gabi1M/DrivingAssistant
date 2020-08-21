using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlUserSettingsService : IUserSettingsService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly UserSettingsTableAdapter _tableAdapter = new UserSettingsTableAdapter();

        //============================================================
        public MssqlUserSettingsService()
        {
            _tableAdapter.Connection = new SqlConnection(Constants.ServerConstants.GetMssqlConnectionString());
        }

        //============================================================
        public async Task<IEnumerable<UserSettings>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.UserSettings);
                return _dataset.UserSettings.AsEnumerable().Select(row => new UserSettings
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    CameraSessionId = row.CameraSessionId,
                    CameraHost = row.CameraHost,
                    CameraUsername = row.CameraUsername,
                    CameraPassword = Crypto.DecryptAes(row.CameraPassword)
                });
            });
        }

        //============================================================
        public async Task<UserSettings> GetById(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_UserSettings_By_IdTableAdapter();
                tableAdapter.Fill(_dataset.Get_UserSettings_By_Id, id);
                return _dataset.Get_UserSettings_By_Id.AsEnumerable().Select(row => new UserSettings
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    CameraSessionId = row.CameraSessionId,
                    CameraHost = row.CameraHost,
                    CameraUsername = row.CameraUsername,
                    CameraPassword = Crypto.DecryptAes(row.CameraPassword)
                }).First();
            });
        }

        //============================================================
        public async Task<UserSettings> GetByUser(long userId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_UserSettings_By_UserTableAdapter();
                tableAdapter.Fill(_dataset.Get_UserSettings_By_User, userId);
                return _dataset.Get_UserSettings_By_User.AsEnumerable().Select(row => new UserSettings
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    CameraSessionId = row.CameraSessionId,
                    CameraHost = row.CameraHost,
                    CameraUsername = row.CameraUsername,
                    CameraPassword = Crypto.DecryptAes(row.CameraPassword)
                }).First();
            });
        }

        //============================================================
        public async Task<long> SetAsync(UserSettings userSettings)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(userSettings.Id, userSettings.UserId, userSettings.CameraSessionId,
                    userSettings.CameraHost, userSettings.CameraUsername,
                    Crypto.EncryptAes(userSettings.CameraPassword), ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(UserSettings userSettings)
        {
            await Task.Run(() =>
            {
                _tableAdapter.Delete(userSettings.Id);
            });
        }

        //============================================================
        public void Dispose()
        {
            _dataset.Dispose();
        }
    }
}
