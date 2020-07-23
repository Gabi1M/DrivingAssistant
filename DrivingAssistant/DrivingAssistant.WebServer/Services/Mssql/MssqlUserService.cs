using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlUserService : IUserService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly UserTableAdapter _tableAdapter = new UserTableAdapter();

        //============================================================
        public MssqlUserService(string connectionString)
        {
            _tableAdapter.Connection = new SqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<User>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.User);
                return _dataset.User.AsEnumerable().Select(row => new User
                {
                    Id = row.Id,
                    Username = row.Username,
                    Password = row.Password,
                    FirstName = row.FirstName,
                    LastName = row.LastName,
                    Email = row.Email,
                    Role = (UserRole) Enum.Parse(typeof(UserRole), row.Role),
                    JoinDate = row.JoinDate
                }).ToList();
            });
        }

        //============================================================
        public async Task<long> SetAsync(User user)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(user.Id, user.Username, user.Password, user.FirstName, user.LastName, user.Email,
                    user.Role.ToString(), user.JoinDate, ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(User user)
        {
            await Task.Run(() =>
            {
                _tableAdapter.Delete(user.Id);
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
