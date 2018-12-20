using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;

namespace StressCalculator
{
    class DBSender
    {
        public static async Task SendMeasurementToDBAsync(Measurement m, String ActivityName)
        {
            var connString = Environment.GetEnvironmentVariable("dbConnection");
            using (SqlConnection conn = new SqlConnection(connString))
            {
                int ActivityID = await GetActivityID(ActivityName);
                conn.Open();
                String query = "INSERT INTO Measurements" +
                                "(UserID, Date, ActivityID, GPSLat, GPSLng, TRI, PNN50, SDNN, SDSD, StressIndex, IsStressed) " +
                                "VALUES" +
                                "(@UserID, @Date, @ActivityID, @GPSLat, @GPSLng, @TRI, @PNN50, @SDNN, @SDSD, @StressIndex, @IsStressed) ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", m.UserID);
                cmd.Parameters.AddWithValue("@Date", m.Date);
                cmd.Parameters.AddWithValue("@ActivityID", ActivityID);
                cmd.Parameters.AddWithValue("@GPSLat", m.GPSLat);
                cmd.Parameters.AddWithValue("@GPSLng", m.GPSLng);
                cmd.Parameters.AddWithValue("@TRI", m.TRI);
                cmd.Parameters.AddWithValue("@PNN50", m.PNN50);
                cmd.Parameters.AddWithValue("@SDNN", m.SDNN);
                cmd.Parameters.AddWithValue("@SDSD", m.SDSD);
                cmd.Parameters.AddWithValue("@StressIndex", m.StressIndex);
                cmd.Parameters.AddWithValue("@IsStressed", m.IsStressed);
                // insert to Measurements
                var rows = await cmd.ExecuteNonQueryAsync();
                //update Activity counter
                cmd.CommandText = "UPDATE Activities set Counter=Counter+1 where ActivityID=@ActivityID";
                await cmd.ExecuteNonQueryAsync();
                conn.Close();
            }
        }

        /*
         * returns the ActivityID of "ActivityName" in the Activities Table,
         *inserts to the table if "ActivityName" doesn't already exist.
         */
        public static async Task<int> GetActivityID(String ActivityName)
        {
            var connString = Environment.GetEnvironmentVariable("dbConnection");
            using (SqlConnection conn = new SqlConnection(connString))
            {
                int ActivityID = 0;
                conn.Open();
                String query = "SELECT ActivityID from Activities WHERE Name=@Name";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", ActivityName);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    ActivityID = int.Parse(reader["ActivityID"].ToString());
                    conn.Close();
                    return ActivityID;
                }
                else //ActivityName doesn't exist in table
                {
                    reader.Close();
                    cmd.CommandText = "INSERT INTO Activities(Name,Counter) OUTPUT Inserted.ActivityID " +
                                      "VALUES(@Name,0)";
                    var rows = await cmd.ExecuteScalarAsync(); //returns the ID of the newly added Activity
                    ActivityID = int.Parse(rows.ToString());
                }
                conn.Close();
                return ActivityID;
            }
        }

        /*
         * returns a list of last 10 StressIndex values, when the user was relaxed.
         * i.e. when IsStressed = 0
         */
        public static async Task<List<int>> GetPrevRelaxStressIndex(String UserID)
        {
            var connString = Environment.GetEnvironmentVariable("dbConnection");
            using (SqlConnection conn = new SqlConnection(connString))
            {
                List<int> StressIndexes = new List<int>();
                //dt.AddHours(-2); //get data from the past 2 hours
                //DateTime date = new DateTime(dt.Year, dt.Month, dt.Day, dt., 0, 0); //at midnight
                String query = "SELECT TOP 10 StressIndex from Measurements " +
                               "WHERE UserID=@UserID and IsStressed=0";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                //cmd.Parameters.AddWithValue("@Date", dt);

                conn.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    StressIndexes.Add(int.Parse(reader["StressIndex"].ToString()));
                }
                conn.Close();
                return StressIndexes;
            }
        }
        /*
        * returns the last StressIndex value of when the user was relaxed,
        * i.e. when IsStressed = 1
        */
        public static async Task<int> GetPrevStressedStressIndex(String UserID)
        {
            var connString = Environment.GetEnvironmentVariable("dbConnection");
            using (SqlConnection conn = new SqlConnection(connString))
            {
                int StressIndex = -1;
                String query = "SELECT StressIndex from Measurements " +
                               "WHERE UserID=@UserID and IsStressed=1 ORDER BY ActivityID DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", UserID);

                conn.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    StressIndex = int.Parse(reader["StressIndex"].ToString());
                }
                conn.Close();
                return StressIndex;
            }
        }
        public static async Task SendTestToDBAsync()
        {
            var connString = Environment.GetEnvironmentVariable("dbConnection");
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                String query = "INSERT INTO Testing" +
                                "(id, SomeField, Date) " +
                                "VALUES" +
                                "(@id, @SomeField, @Date) ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", "1234");
                cmd.Parameters.AddWithValue("@SomeField", "bar");
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                // Execute the command
                var rows = await cmd.ExecuteNonQueryAsync();
                conn.Close();
            }
        }
        public static async Task SendUserToDBAsync()
        {
            var connString = Environment.GetEnvironmentVariable("dbConnection");
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                String query = "INSERT INTO Users" +
                                "(UserID, FirstName, LastName, DateOfBirth, UserType) " +
                                "VALUES" +
                                "(@UserID, @FirstName, @LastName, @DateOfBirth, @UserType) ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", "1234");
                cmd.Parameters.AddWithValue("@FirstName", "bar");
                cmd.Parameters.AddWithValue("@LastName", "test");
                cmd.Parameters.AddWithValue("@DateOfBirth", new DateTime(1992,6,5));
                cmd.Parameters.AddWithValue("@UserType", 0);
                // Execute the command
                var rows = await cmd.ExecuteNonQueryAsync();
                conn.Close();
            }
        }
        public static String insertQueryString(Measurement m)
        {
            String query = "INSERT INTO Measurements (";
            String values = "(";
            foreach (FieldInfo field in m.GetType().GetFields())
            {
                query += field.Name + ",";
                var val = field.GetValue(m);
                if (val is DateTime)
                    values += ((DateTime)val).ToUniversalTime();
                else
                    values += field.GetValue(m) + ",";
            }
            query = query.Substring(0, query.Length - 1) + ")";
            values = values.Substring(0, values.Length - 1) + ")";
            return query + " VALUES " + values;
            
        }
    }
}
