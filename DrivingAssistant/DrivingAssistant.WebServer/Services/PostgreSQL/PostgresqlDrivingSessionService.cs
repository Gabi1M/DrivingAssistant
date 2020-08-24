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
    public class PostgresqlDrivingSessionService : IDrivingSessionService
    {
        //============================================================
        public async Task<IEnumerable<DrivingSession>> GetAsync()
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetAllSessions, connection);
                var result = await command.ExecuteReaderAsync();
                var sessions = new List<DrivingSession>();
                while (await result.ReadAsync())
                {
                    sessions.Add(new DrivingSession
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
        public async Task<DrivingSession> GetById(long id)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetSessionById, connection);
                command.Parameters.AddWithValue("id", id);
                var result = await command.ExecuteReaderAsync();
                var sessions = new List<DrivingSession>();
                while (await result.ReadAsync())
                {
                    sessions.Add(new DrivingSession
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
        public async Task<IEnumerable<DrivingSession>> GetByUser(long userId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetSessionsByUser, connection);
                command.Parameters.AddWithValue("user_id", userId);
                var result = await command.ExecuteReaderAsync();
                var sessions = new List<DrivingSession>();
                while (await result.ReadAsync())
                {
                    sessions.Add(new DrivingSession
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
        public async Task<long> SetAsync(DrivingSession drivingSession)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                if (drivingSession.Id == -1)
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.SetSession, connection);
                    command.Parameters.AddWithValue("user_id", drivingSession.UserId);
                    command.Parameters.AddWithValue("name", drivingSession.Name);
                    command.Parameters.AddWithValue("start_date_time", drivingSession.StartDateTime);
                    command.Parameters.AddWithValue("end_date_time", drivingSession.EndDateTime);
                    command.Parameters.AddWithValue("start_location", drivingSession.StartLocation.PointToString());
                    command.Parameters.AddWithValue("end_location", drivingSession.EndLocation.PointToString());
                    command.Parameters.AddWithValue("waypoints", drivingSession.Waypoints.PointCollectionToString());
                    command.Parameters.AddWithValue("status", drivingSession.Status.ToString());
                    command.Parameters.AddWithValue("date_added", drivingSession.DateAdded);
                    var result = Convert.ToInt64(await command.ExecuteScalarAsync());
                    return result;
                }
                else
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.UpdateSession, connection);
                    command.Parameters.AddWithValue("id", drivingSession.Id);
                    command.Parameters.AddWithValue("user_id", drivingSession.UserId);
                    command.Parameters.AddWithValue("name", drivingSession.Name);
                    command.Parameters.AddWithValue("start_date_time", drivingSession.StartDateTime);
                    command.Parameters.AddWithValue("end_date_time", drivingSession.EndDateTime);
                    command.Parameters.AddWithValue("start_location", drivingSession.StartLocation.PointToString());
                    command.Parameters.AddWithValue("end_location", drivingSession.EndLocation.PointToString());
                    command.Parameters.AddWithValue("waypoints", drivingSession.Waypoints.PointCollectionToString());
                    command.Parameters.AddWithValue("status", drivingSession.Status.ToString());
                    command.Parameters.AddWithValue("date_added", drivingSession.DateAdded);
                    await command.ExecuteNonQueryAsync();
                    return drivingSession.Id;
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
        public async Task DeleteAsync(DrivingSession drivingSession)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.DeleteSession, connection);
                command.Parameters.AddWithValue("id", drivingSession.Id);
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
