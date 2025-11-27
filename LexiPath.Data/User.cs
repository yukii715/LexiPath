using System;

namespace LexiPath.Data
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string ProfilePicPath { get; set; }
    }
}