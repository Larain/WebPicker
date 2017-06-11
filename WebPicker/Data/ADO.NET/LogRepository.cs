using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using PickerGameModel.Interfaces;
using WebPicker.Models;

namespace WebPicker.Data.ADO.NET
{
    public class LogRepository : IRepository<Log>
    {
        public string ConnectionString { get; set; }

        public LogRepository(IConfiguration config)
        {
            ConnectionString = config.GetConnectionString("LoggerConnection");
        }

        public List<Log> Get()
        {
            var logs = new List<Log>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT * FROM LOG", connection))
                {
                    cmd.CommandType = CommandType.Text;

                    connection.Open();

                    using (IDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            logs.Add(Mapper.Map<Log>(dr));
                        }
                    }
                }
            }
            return logs;
        }

        public async Task<List<Log>> GetAsync()
        {
            var logs = new List<Log>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT * FROM LOG", connection))
                {
                    cmd.CommandType = CommandType.Text;

                    connection.Open();

                    using (IDataReader dr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            logs.Add(Mapper.Map<Log>(dr));
                        }
                    }
                }
            }
            return logs;
        }

        public Log Get(int id)
        {
            Log log = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT * FROM LOG WHERE Id = @id", connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (IDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            log = Mapper.Map<Log>(dr);
                        }
                    }
                }
            }
            return log;
        }

        public async Task<Log> GetAsync(int id)
        {
            Log log = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT * FROM LOG WHERE Id = @id", connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (IDataReader dr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            log = Mapper.Map<Log>(dr);
                        }
                    }
                }
            }
            return log;
        }

        public int Update(Log item)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                const string commandText = "UPDATE LOG " +
                                           "SET Date = @date" +
                                           ",Thread = @thread" +
                                           ",Level = @level" +
                                           ",Logger = @logger" +
                                           ",User = @user" +
                                           ",Action = @action" +
                                           ",Message = @message" +
                                           ",Exception = @exception" +
                                           "WHERE Id = @id";

                using (var cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    cmd.Parameters.AddWithValue("@date", item.Date);
                    cmd.Parameters.AddWithValue("@thread", item.Thread);
                    cmd.Parameters.AddWithValue("@level", item.Level);
                    cmd.Parameters.AddWithValue("@logger", item.Logger);
                    cmd.Parameters.AddWithValue("@user", item.User);
                    cmd.Parameters.AddWithValue("@action", item.Action);
                    cmd.Parameters.AddWithValue("@message", item.Message);
                    cmd.Parameters.AddWithValue("@exception", item.Exception);

                    connection.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public async Task<int> UpdateAsync(Log item)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                const string commandText = "UPDATE LOG " +
                                           "SET Date = @date" +
                                           ",Thread = @thread" +
                                           ",Level = @level" +
                                           ",Logger = @logger" +
                                           ",User = @user" +
                                           ",Action = @action" +
                                           ",Message = @message" +
                                           ",Exception = @exception" +
                                           "WHERE Id = @id";

                using (var cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    cmd.Parameters.AddWithValue("@date", item.Date);
                    cmd.Parameters.AddWithValue("@thread", item.Thread);
                    cmd.Parameters.AddWithValue("@level", item.Level);
                    cmd.Parameters.AddWithValue("@logger", item.Logger);
                    cmd.Parameters.AddWithValue("@user", item.User);
                    cmd.Parameters.AddWithValue("@action", item.Action);
                    cmd.Parameters.AddWithValue("@message", item.Message);
                    cmd.Parameters.AddWithValue("@exception", item.Exception);

                    connection.Open();

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public int Insert(Log item)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                const string commandText = "INSERT INTO LOG (Thread, Level, Logger, User, Action, Message, Exception)" +
                                           "VALUES (@date, @thread, @level, @logger, @user, @action, @message, @exception)";

                using (var cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@date", item.Date);
                    cmd.Parameters.AddWithValue("@thread", item.Thread);
                    cmd.Parameters.AddWithValue("@level", item.Level);
                    cmd.Parameters.AddWithValue("@logger", item.Logger);
                    cmd.Parameters.AddWithValue("@user", item.User);
                    cmd.Parameters.AddWithValue("@action", item.Action);
                    cmd.Parameters.AddWithValue("@message", item.Message);
                    cmd.Parameters.AddWithValue("@exception", item.Exception);

                    connection.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public async Task<int> InsertAsync(Log item)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                const string commandText = "INSERT INTO LOG (Thread, Level, Logger, User, Action, Message, Exception)" +
                                           "VALUES (@date, @thread, @level, @logger, @user, @action, @message, @exception)";

                using (var cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@date", item.Date);
                    cmd.Parameters.AddWithValue("@thread", item.Thread);
                    cmd.Parameters.AddWithValue("@level", item.Level);
                    cmd.Parameters.AddWithValue("@logger", item.Logger);
                    cmd.Parameters.AddWithValue("@user", item.User);
                    cmd.Parameters.AddWithValue("@action", item.Action);
                    cmd.Parameters.AddWithValue("@message", item.Message);
                    cmd.Parameters.AddWithValue("@exception", item.Exception);

                    connection.Open();

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public int Delete(int itemId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                const string commandText = "DELETE FROM LOG" +
                                           "WHERE Id = @id";

                using (var cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", itemId);

                    connection.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public async Task<int> DeleteAsync(int itemId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                const string commandText = "DELETE FROM LOG" +
                                           "WHERE Id = @id";

                using (var cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", itemId);

                    connection.Open();

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public int Delete(Log item)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                const string commandText = "DELETE FROM LOG" +
                                           "WHERE Id = @id";

                using (var cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", item.Id);

                    connection.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public async Task<int> DeleteAsync(Log item)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                const string commandText = "DELETE FROM LOG" +
                                           "WHERE Id = @id";

                using (var cmd = new SqlCommand(commandText, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", item.Id);

                    connection.Open();

                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}