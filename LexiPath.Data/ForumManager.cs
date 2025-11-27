using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LexiPath.Data
{
    // Helper class for display
    public class ForumPostDisplay
    {
        public int PostID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string ProfilePicPath { get; set; }
        public List<ForumCommentDisplay> Comments { get; set; }
    }

    public class ForumCommentDisplay
    {
        public int CommentID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string ProfilePicPath { get; set; }
    }

    public class ForumManager
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["LexiPathDB"].ConnectionString;
        }

        public List<ForumPostDisplay> GetPostsByCourseID(int courseId)
        {
            List<ForumPostDisplay> posts = new List<ForumPostDisplay>();
            // JOIN with Users table to get the poster's info efficiently
            string sql = @"
                SELECT p.PostID, p.Content, p.CreatedAt, u.Username, u.ProfilePicPath 
                FROM ForumPost p
                INNER JOIN Users u ON p.UserID = u.UserID
                WHERE p.CourseID = @CourseID AND p.Status = 'Published'
                ORDER BY p.CreatedAt DESC";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        posts.Add(new ForumPostDisplay
                        {
                            PostID = (int)reader["PostID"],
                            Content = reader["Content"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            Username = reader["Username"].ToString(),
                            ProfilePicPath = reader["ProfilePicPath"] != DBNull.Value ? reader["ProfilePicPath"].ToString() : "/Image/System/placeholder_profile.png"
                        });
                    }
                }
            }
            return posts;
        }

        public List<ForumCommentDisplay> GetCommentsByPostID(int postId)
        {
            List<ForumCommentDisplay> comments = new List<ForumCommentDisplay>();
            // JOIN with Users table
            string sql = @"
                SELECT c.CommentID, c.Content, c.CreatedAt, u.Username, u.ProfilePicPath 
                FROM ForumPostComment c
                INNER JOIN Users u ON c.UserID = u.UserID
                WHERE c.PostID = @PostID AND c.Status = 'Published'
                ORDER BY c.CreatedAt ASC";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PostID", postId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        comments.Add(new ForumCommentDisplay
                        {
                            CommentID = (int)reader["CommentID"],
                            Content = reader["Content"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            Username = reader["Username"].ToString(),
                            ProfilePicPath = reader["ProfilePicPath"] != DBNull.Value ? reader["ProfilePicPath"].ToString() : "/Image/System/placeholder_profile.png"
                        });
                    }
                }
            }
            return comments;
        }

        public bool CreatePost(int courseId, int userId, string content)
        {
            string sql = "INSERT INTO ForumPost (CourseID, UserID, Content) VALUES (@CID, @UID, @Content)";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CID", courseId);
                    cmd.Parameters.AddWithValue("@UID", userId);
                    cmd.Parameters.AddWithValue("@Content", content);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool CreateComment(int postId, int userId, string content)
        {
            string sql = "INSERT INTO ForumPostComment (PostID, UserID, Content) VALUES (@PID, @UID, @Content)";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PID", postId);
                    cmd.Parameters.AddWithValue("@UID", userId);
                    cmd.Parameters.AddWithValue("@Content", content);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}