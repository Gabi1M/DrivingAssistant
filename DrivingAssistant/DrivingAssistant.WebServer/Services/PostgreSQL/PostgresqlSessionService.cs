using DrivingAssistant.WebServer.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.PostgreSQL
{
    public class PostgresqlSessionService : ISessionService
    {
        //============================================================
        public async Task<IEnumerable<Session>> GetAsync()
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetAllSessions, connection);
                var result = await command.ExecuteReaderAsync();
                var sessions = new List<Session>();
                while (await result.ReadAsync())
                {
                    sessions.Add(new Session
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        Name = result["name"].ToString(),
                        StartDateTime = Convert.ToDateTime(result["start_date_time"]),
                        EndDateTime = Convert.ToDateTime(result["end_date_time"]),
                        StartLocation = result["start_location"].ToString().StringToPoint(),
                        EndLocation = result["end_location"].ToString().StringToPoint(),
                        Waypoints = result["waypoints"].ToString().StringToPointCollection(),
                        Status = Enum.Parse<SessionStatus>(result["status"].ToString()!),
                        DateAdded = Convert.ToDateTime(result["date_added"])
                    });
                }

                return sessions;
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
        public async Task<Session> GetById(long id)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetSessionById, connection);
                command.Parameters.AddWithValue("id", id);
                var result = await command.ExecuteReaderAsync();
                var sessions = new List<Session>();
                while (await result.ReadAsync())
                {
                    sessions.Add(new Session
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        Name = result["name"].ToString(),
                        StartDateTime = Convert.ToDateTime(result["start_date_time"]),
                        EndDateTime = Convert.ToDateTime(result["end_date_time"]),
                        StartLocation = result["start_location"].ToString().StringToPoint(),
                        EndLocation = result["end_location"].ToString().StringToPoint(),
                        Waypoints = result["waypoints"].ToString().StringToPointCollection(),
                        Status = Enum.Parse<SessionStatus>(result["status"].ToString()!),
                        DateAdded = Convert.ToDateTime(result["date_added"])
                    });
                }

                return sessions.Single();
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
        public async Task<IEnumerable<Session>> GetByUser(long userId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetSessionsByUser, connection);
                command.Parameters.AddWithValue("user_id", userId);
                var result = await command.ExecuteReaderAsync();
                var sessions = new List<Session>();
                while (await result.ReadAsync())
                {
                    sessions.Add(new Session
                    {
                        Id = Convert.ToInt64(result["id"]),
                        UserId = Convert.ToInt64(result["user_id"]),
                        Name = result["name"].ToString(),
                        StartDateTime = Convert.ToDateTime(result["start_date_time"]),
                        EndDateTime = Convert.ToDateTime(result["end_date_time"]),
                        StartLocation = result["start_location"].ToString().StringToPoint(),
                        EndLocation = result["end_location"].ToString().StringToPoint(),
                        Waypoints = result["waypoints"].ToString().StringToPointCollection(),
                        Status = Enum.Parse<SessionStatus>(result["status"].ToString()!),
                        DateAdded = Convert.ToDateTime(result["date_added"])
                    });
                }

                return sessions;
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
        public async Task<long> SetAsync(Session session)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                if (session.Id == -1)
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.SetSession, connection);
                    command.Parameters.AddWithValue("user_id", session.UserId);
                    command.Parameters.AddWithValue("name", session.Name);
                    command.Parameters.AddWithValue("start_date_time", session.StartDateTime);
                    command.Parameters.AddWithValue("end_date_time", session.EndDateTime);
                    command.Parameters.AddWithValue("start_location", session.StartLocation.PointToString());
                    command.Parameters.AddWithValue("end_location", session.EndLocation.PointToString());
                    command.Parameters.AddWithValue("waypoints", session.Waypoints.PointCollectionToString());
                    command.Parameters.AddWithValue("status", session.Status.ToString());
                    command.Parameters.AddWithValue("date_added", session.DateAdded);
                    var result = Convert.ToInt64(await command.ExecuteScalarAsync());
                    return result;
                }
                else
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.UpdateSession, connection);
                    command.Parameters.AddWithValue("id", session.Id);
                    command.Parameters.AddWithValue("user_id", session.UserId);
                    command.Parameters.AddWithValue("name", session.Name);
                    command.Parameters.AddWithValue("start_date_time", session.StartDateTime);
                    command.Parameters.AddWithValue("end_date_time", session.EndDateTime);
                    command.Parameters.AddWithValue("start_location", session.StartLocation.PointToString());
                    command.Parameters.AddWithValue("end_location", session.EndLocation.PointToString());
                    command.Parameters.AddWithValue("waypoints", session.Waypoints.PointCollectionToString());
                    command.Parameters.AddWithValue("status", session.Status.ToString());
                    command.Parameters.AddWithValue("date_added", session.DateAdded);
                    await command.ExecuteNonQueryAsync();
                    return session.Id;
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
        public async Task DeleteAsync(Session session)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.DeleteSession, connection);
                command.Parameters.AddWithValue("id", session.Id);
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
