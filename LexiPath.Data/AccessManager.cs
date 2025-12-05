using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace LexiPath.Data
{
    public class AccessManager
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["LexiPathDB"].ConnectionString;
        }

        public void StartCourse(int userId, int courseId)
        {
            string sqlCheck = "SELECT COUNT(*) FROM AccessRecord WHERE UserID = @UserID AND CourseID = @CourseID AND is_completed = 0";
            string sqlInsert = "INSERT INTO AccessRecord (UserID, CourseID, StartAt, is_completed) VALUES (@UserID, @CourseID, @StartAt, 0)";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                int activeRecords = 0;
                using (SqlCommand checkCmd = new SqlCommand(sqlCheck, conn))
                {
                    checkCmd.Parameters.AddWithValue("@UserID", userId);
                    checkCmd.Parameters.AddWithValue("@CourseID", courseId);
                    activeRecords = (int)checkCmd.ExecuteScalar();
                }

                if (activeRecords == 0) // Only insert if no active, incomplete record is found
                {
                    using (SqlCommand insertCmd = new SqlCommand(sqlInsert, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@UserID", userId);
                        insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                        insertCmd.Parameters.AddWithValue("@StartAt", DateTime.Now);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void EndCourse(int userId, int courseId)
        {
            string sqlUpdate = @"
                UPDATE AccessRecord 
                SET EndAt = @EndAt, is_completed = 1 
                WHERE RecordID = (
                    SELECT TOP 1 RecordID 
                    FROM AccessRecord 
                    WHERE UserID = @UserID AND CourseID = @CourseID AND is_completed = 0 
                    ORDER BY StartAt DESC
                )";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand updateCmd = new SqlCommand(sqlUpdate, conn))
                {
                    updateCmd.Parameters.AddWithValue("@UserID", userId);
                    updateCmd.Parameters.AddWithValue("@CourseID", courseId);
                    updateCmd.Parameters.AddWithValue("@EndAt", DateTime.Now);
                    conn.Open();
                    updateCmd.ExecuteNonQuery();
                }
            }
        }

        /**
         * Checks if the Practice module should be unlocked.
         */
        public bool IsCourseComplete(int userId, int courseId)
        {
            string sql = "SELECT COUNT(*) FROM AccessRecord WHERE UserID = @UserID AND CourseID = @CourseID AND is_completed = 1";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public HashSet<int> GetCompletedCourseIds(int userId)
        {
            HashSet<int> courseIds = new HashSet<int>();
            // We only select courses that are marked as complete
            string sql = "SELECT DISTINCT CourseID FROM AccessRecord WHERE UserID = @UserID AND is_completed = 1";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        courseIds.Add((int)reader["CourseID"]);
                    }
                }
            }
            return courseIds;
        }

        public int GetCompletedCourseCount(int userId)
        {
            string sql = "SELECT COUNT(DISTINCT CourseID) FROM AccessRecord WHERE UserID = @UserID AND is_completed = 1";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    // ExecuteScalar is perfect for getting a single value (like a count)
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public double GetTotalLearningMinutes(int userId)
        {
            string sql = @"
                WITH SessionDurations AS (
                    SELECT 
                        DATEDIFF(MINUTE, StartAt, EndAt) AS DurationMinutes
                    FROM 
                        AccessRecord
                    WHERE 
                        UserID = @UserID 
                        AND is_completed = 1 
                        AND EndAt IS NOT NULL
                )
                SELECT 
                    SUM(CASE 
                        WHEN DurationMinutes > 10 THEN 10.0  -- Limit active time to 10 minutes per record
                        ELSE DurationMinutes 
                    END)
                FROM 
                    SessionDurations";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDouble(result);
                    }
                    return 0.0;
                }
            }
        }
    }
}