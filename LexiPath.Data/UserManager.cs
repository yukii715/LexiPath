using System;
using System.Collections.Generic;
using System.Configuration; // Add this
using System.Data;
using System.Data.SqlClient; // Add this

namespace LexiPath.Data
{
    public class UserManager
    {
        // Helper method to get the connection string from Web.config
        private string GetConnectionString()
        {
            // You will need to add a reference to System.Configuration
            return ConfigurationManager.ConnectionStrings["LexiPathDB"].ConnectionString;
        }

        /**
         * REGISTRATION (INSERT)
         * This fulfills the "Insert new records" requirement.
         */
        public bool RegisterUser(string username, string email, string passwordHash)
        {
            // Use 'using' blocks to ensure connections are closed
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sql = "INSERT INTO Users (Username, Email, PasswordHash, IsAdmin) VALUES (@Username, @Email, @PasswordHash, 0)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Add parameters to prevent SQL Injection
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    try
                    {
                        conn.Open();
                        // ExecuteNonQuery returns the number of rows affected.
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return (rowsAffected == 1);
                    }
                    catch (SqlException)
                    {
                        // Will fail if username or email is not unique
                        return false;
                    }
                }
            }
        }

        /**
         * LOGIN (READ)
         * This fulfills the "Display records" and "Authentication" requirements.
         */
        public User AuthenticateUser(string username, string passwordHash)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sql = "SELECT UserID, Username, Email, IsAdmin, ProfilePicPath FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash AND Status = 'Active'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // If a matching user was found
                        {
                            // Create a User object with the data
                            User user = new User
                            {
                                UserID = (int)reader["UserID"],
                                Username = (string)reader["Username"],
                                Email = (string)reader["Email"],
                                IsAdmin = (bool)reader["IsAdmin"],
                                ProfilePicPath = reader["ProfilePicPath"] != DBNull.Value ? (string)reader["ProfilePicPath"] : null
                            };
                            return user; // Return the populated User object
                        }
                    }
                }
            }

            return null; // No user found
        }

        public bool IsUsernameTaken(string username)
        {
            return IsUsernameTaken(username, -1); // -1 means check all rows
        }

        public bool IsUsernameTaken(string username, int excludeUserId)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND UserID != @ExcludeID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@ExcludeID", excludeUserId);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool IsEmailTaken(string email)
        {
            return IsEmailTaken(email, -1);
        }

        public bool IsEmailTaken(string email, int excludeUserId)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND UserID != @ExcludeID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@ExcludeID", excludeUserId);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }



        /**
         * READ: Gets a user's full profile info
         */
        public User GetUserProfile(int userId)
        {
            User user = null;
            string sql = "SELECT UserID, Username, Email, IsAdmin, ProfilePicPath FROM Users WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        user = new User
                        {
                            UserID = (int)reader["UserID"],
                            Username = (string)reader["Username"],
                            Email = (string)reader["Email"],
                            IsAdmin = (bool)reader["IsAdmin"],
                            ProfilePicPath = reader["ProfilePicPath"] != DBNull.Value ? (string)reader["ProfilePicPath"] : null
                        };
                    }
                }
            }
            return user;
        }

        /**
         * UPDATE: Updates a user's username and email
         */
        public bool UpdateUserProfile(int userId, string username, string email)
        {
            string sql = "UPDATE Users SET Username = @Username, Email = @Email WHERE UserID = @UserID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /**
         * UPDATE: Updates a user's password
         */
        public bool UpdatePassword(int userId, string newPasswordHash)
        {
            string sql = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /**
         * UPDATE: Updates a user's profile picture path
         */
        public bool UpdateProfilePicture(int userId, string imagePath)
        {
            string sql = "UPDATE Users SET ProfilePicPath = @ImagePath WHERE UserID = @UserID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /**
         * NEW: Gets all users as a DataTable (easy to bind to a GridView)
         */
        public DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();
            // We don't select PasswordHash for security
            string sql = "SELECT UserID, Username, Email, CreatedAt, Status, ProfilePicPath FROM Users WHERE IsAdmin = 0";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    // Use SqlDataAdapter to fill the DataTable
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        /**
         * NEW: Toggles a user's status between "Active" and "Blocked"
         */
        public void ToggleUserStatus(int userId)
        {
            string currentStatus = "";
            string getStatusSql = "SELECT Status FROM Users WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                // 1. Get the user's current status
                using (SqlCommand getCmd = new SqlCommand(getStatusSql, conn))
                {
                    getCmd.Parameters.AddWithValue("@UserID", userId);
                    object result = getCmd.ExecuteScalar();
                    if (result != null)
                    {
                        currentStatus = result.ToString();
                    }
                }

                // 2. Determine the new status
                string newStatus = (currentStatus == "Active") ? "Blocked" : "Active";

                // 3. Update the status
                string updateSql = "UPDATE Users SET Status = @NewStatus WHERE UserID = @UserID";
                using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                {
                    updateCmd.Parameters.AddWithValue("@NewStatus", newStatus);
                    updateCmd.Parameters.AddWithValue("@UserID", userId);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }




        // --- CORE CRUD OPERATIONS (Collection & Liking) ---
        /**
         * Toggles the IsCollected status of a course for a user (bookmark).
         */
        public bool ToggleCollection(int userId, int courseId)
        {
            string sqlCheck = "SELECT IsCollected FROM UserCourseInteraction WHERE UserID = @UserID AND CourseID = @CourseID";
            object result;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand checkCmd = new SqlCommand(sqlCheck, conn))
                {
                    checkCmd.Parameters.AddWithValue("@UserID", userId);
                    checkCmd.Parameters.AddWithValue("@CourseID", courseId);
                    result = checkCmd.ExecuteScalar();
                }

                bool newStatus = true;
                bool exists = (result != null);

                if (exists)
                {
                    // Existing record found: determine the new status (toggle)
                    newStatus = !(bool)result;
                    string sqlUpdate = "UPDATE UserCourseInteraction SET IsCollected = @NewStatus, CollectedAt = @CollectedAt WHERE UserID = @UserID AND CourseID = @CourseID";
                    using (SqlCommand updateCmd = new SqlCommand(sqlUpdate, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@UserID", userId);
                        updateCmd.Parameters.AddWithValue("@CourseID", courseId);
                        updateCmd.Parameters.AddWithValue("@NewStatus", newStatus);
                        updateCmd.Parameters.AddWithValue("@CollectedAt", newStatus ? (object)DateTime.Now : DBNull.Value);
                        return updateCmd.ExecuteNonQuery() > 0;
                    }
                }
                else
                {
                    // No record found: insert a new one, setting IsCollected = 1 (true) and IsLiked = 0 (false)
                    string sqlInsert = "INSERT INTO UserCourseInteraction (UserID, CourseID, IsCollected, CollectedAt, IsLiked) VALUES (@UserID, @CourseID, 1, @CollectedAt, 0)";
                    using (SqlCommand insertCmd = new SqlCommand(sqlInsert, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@UserID", userId);
                        insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                        insertCmd.Parameters.AddWithValue("@CollectedAt", DateTime.Now);
                        return insertCmd.ExecuteNonQuery() > 0;
                    }
                }
            }
        }

        /**
         * Toggles the IsLiked status of a course for a user.
         */
        public bool ToggleLike(int userId, int courseId)
        {
            string sqlCheck = "SELECT IsLiked FROM UserCourseInteraction WHERE UserID = @UserID AND CourseID = @CourseID";
            object result;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand checkCmd = new SqlCommand(sqlCheck, conn))
                {
                    checkCmd.Parameters.AddWithValue("@UserID", userId);
                    checkCmd.Parameters.AddWithValue("@CourseID", courseId);
                    result = checkCmd.ExecuteScalar();
                }

                bool newStatus = true;
                bool exists = (result != null);

                if (exists)
                {
                    newStatus = !(bool)result;
                    string sqlUpdate = "UPDATE UserCourseInteraction SET IsLiked = @NewStatus, LikedAt = @LikedAt WHERE UserID = @UserID AND CourseID = @CourseID";
                    using (SqlCommand updateCmd = new SqlCommand(sqlUpdate, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@UserID", userId);
                        updateCmd.Parameters.AddWithValue("@CourseID", courseId);
                        updateCmd.Parameters.AddWithValue("@NewStatus", newStatus);
                        updateCmd.Parameters.AddWithValue("@LikedAt", newStatus ? (object)DateTime.Now : DBNull.Value);
                        return updateCmd.ExecuteNonQuery() > 0;
                    }
                }
                else
                {
                    // Insert a new record, setting IsLiked = 1 (true) and IsCollected = 0 (false)
                    string sqlInsert = "INSERT INTO UserCourseInteraction (UserID, CourseID, IsCollected, CollectedAt, IsLiked, LikedAt) VALUES (@UserID, @CourseID, 0, NULL, 1, @LikedAt)";
                    using (SqlCommand insertCmd = new SqlCommand(sqlInsert, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@UserID", userId);
                        insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                        insertCmd.Parameters.AddWithValue("@LikedAt", DateTime.Now);
                        return insertCmd.ExecuteNonQuery() > 0;
                    }
                }
            }
        }

        /**
         * Reads the current status of interactions for a course.
         */
        public Dictionary<string, bool> GetInteractionStatus(int userId, int courseId)
        {
            var status = new Dictionary<string, bool> { { "IsCollected", false }, { "IsLiked", false } };
            string sql = "SELECT IsCollected, IsLiked FROM UserCourseInteraction WHERE UserID = @UserID AND CourseID = @CourseID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        status["IsCollected"] = (bool)reader["IsCollected"];
                        status["IsLiked"] = (bool)reader["IsLiked"];
                    }
                }
            }
            return status;
        }

        /**
         * NEW: Gets all courses collected (bookmarked) by the user.
         */
        public List<Course> GetCollectedCourses(int userId, string searchTerm, string orderBy)
        {
            List<Course> courses = new List<Course>();
            string sql = @"
                SELECT c.CourseID, c.CourseName, c.Description, c.ImagePath, c.CourseType, uci.CollectedAt
                FROM UserCourseInteraction uci
                JOIN Courses c ON uci.CourseID = c.CourseID
                WHERE uci.UserID = @UserID AND uci.IsCollected = 1 AND c.isActivated = 1";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " AND c.CourseName LIKE @SearchTerm";
            }

            // --- NEW: Dynamic Ordering ---
            string orderClause = "c.CourseName ASC"; // Default alphabetical
            if (orderBy == "DateAdded")
            {
                orderClause = "uci.CollectedAt ASC"; // Oldest to newest
            }
            // --- END NEW ---

            sql += $" ORDER BY {orderClause}";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseID = (int)reader["CourseID"],
                            CourseName = (string)reader["CourseName"],
                            Description = reader["Description"] != DBNull.Value ? (string)reader["Description"] : null,
                            ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null,
                            CourseType = (string)reader["CourseType"]
                            // Note: CollectedAt is fetched for sorting but not stored in the Course DTO
                        });
                    }
                }
            }
            return courses;
        }

        /**
         * NEW: Gets all courses liked by the user.
         */
        public List<Course> GetLikedCourses(int userId, string searchTerm, string orderBy)
        {
            List<Course> courses = new List<Course>();
            string sql = @"
                SELECT c.CourseID, c.CourseName, c.Description, c.ImagePath, c.CourseType, uci.LikedAt
                FROM UserCourseInteraction uci
                JOIN Courses c ON uci.CourseID = c.CourseID
                WHERE uci.UserID = @UserID AND uci.IsLiked = 1 AND c.isActivated = 1";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " AND c.CourseName LIKE @SearchTerm";
            }

            // --- NEW: Dynamic Ordering ---
            string orderClause = "c.CourseName ASC"; // Default alphabetical
            if (orderBy == "DateAdded")
            {
                orderClause = "uci.LikedAt ASC"; // Oldest to newest
            }
            // --- END NEW ---

            sql += $" ORDER BY {orderClause}";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseID = (int)reader["CourseID"],
                            CourseName = (string)reader["CourseName"],
                            Description = reader["Description"] != DBNull.Value ? (string)reader["Description"] : null,
                            ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null,
                            CourseType = (string)reader["CourseType"]
                            // Note: LikedAt is fetched for sorting but not stored in the Course DTO
                        });
                    }
                }
            }
            return courses;
        }

        /**
         * NEW: Gets the count of all active registered users.
         */
        public int GetTotalActiveUserCount()
        {
            // We assume 'IsAdmin = 0' are regular users and only count 'Active' status
            string sql = "SELECT COUNT(UserID) FROM Users WHERE Status = 'Active' AND IsAdmin = 0";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    // ExecuteScalar is perfect for getting a single count value
                    return (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}