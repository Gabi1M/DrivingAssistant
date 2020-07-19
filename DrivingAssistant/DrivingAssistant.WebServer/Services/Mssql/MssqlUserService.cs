using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlUserService : UserService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly string _connectionString;

        //============================================================
        public MssqlUserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        //============================================================
        public override async Task<ICollection<User>> GetAsync()
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_UsersTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Get_Users);
                using DataTable dataTable = _dataset.Get_Users;
                var result = from DataRow row in dataTable.AsEnumerable()
                    select new User(row["Username"].ToString(), row["Password"].ToString(),
                        row["FirstName"].ToString(),
                        row["LastName"].ToString(), row["Email"].ToString(), row["Role"].ToString(),
                        Convert.ToDateTime(row["JoinDate"].ToString()), Convert.ToInt64(row["Id"]));
                return result.ToList();
            });
        }

        //============================================================
        public override async Task<User> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_User_By_IdTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Get_User_By_Id, id);
                using DataTable dataTable = _dataset.Get_User_By_Id;
                var result = from DataRow row in dataTable.AsEnumerable()
                    select new User(row["Username"].ToString(), row["Password"].ToString(),
                        row["FirstName"].ToString(),
                        row["LastName"].ToString(), row["Email"].ToString(), row["Role"].ToString(),
                        Convert.ToDateTime(row["JoinDate"].ToString()), Convert.ToInt64(row["Id"]));
                return result.First();
            });
        }

        //============================================================
        public override async Task<long> SetAsync(User user)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Set_UserTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                long? idOut = 0;
                tableAdapter.Fill(_dataset.Set_User, null, user.Username, user.Password, 
                    user.FirstName, user.LastName, user.Email, user.Role.ToString(), user.JoinDate, ref idOut);
                return idOut.Value;
            });
        }

        //============================================================
        public override async Task UpdateAsync(User user)
        {
            await Task.Run(() =>
            {
                using var tableAdapter = new Set_UserTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                long? idOut = 0;
                tableAdapter.Fill(_dataset.Set_User, user.Id, user.Username, user.Password, 
                    user.FirstName, user.LastName, user.Email, user.Role.ToString(), user.JoinDate, ref idOut);
            });
        }

        //============================================================
        public override async Task DeleteAsync(User user)
        {
            await Task.Run(() =>
            {
                using var tableAdapter = new Delete_UserTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Delete_User, user.Id);
            });
        }

        //============================================================
        public override void Dispose()
        {
            _dataset.Dispose();
        }
    }
}
