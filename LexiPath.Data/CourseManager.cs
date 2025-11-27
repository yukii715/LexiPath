using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LexiPath.Data
{
    public class CourseManager
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["LexiPathDB"].ConnectionString;
        }

        // =============================================
        // SEARCH & FILTER METHODS (PUBLIC FACING)
        // =============================================

        public string GetLanguageCode(int languageId)
        {
            string code = "en-US"; // Default fallback
            string sql = "SELECT LanguageCode FROM Language WHERE LanguageID = @ID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", languageId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        code = result.ToString();
                    }
                }
            }
            return code;
        }

        public List<Course> GetAllCourses(string courseType, string searchTerm, int languageId)
        {
            List<Course> courses = new List<Course>();
            string sql = @"
                SELECT DISTINCT c.CourseID, c.CourseName, c.Description, c.ImagePath, c.CourseType 
                FROM Courses c
                LEFT JOIN CourseTagLink ctl ON c.CourseID = ctl.CourseID
                LEFT JOIN Tags t ON ctl.TagID = t.TagID
                WHERE c.isActivated = 1 AND c.LanguageID = @LanguageID";

            if (courseType != "All")
            {
                sql += " AND c.CourseType = @CourseType";
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " AND (c.CourseName LIKE @Search OR c.Description LIKE @Search OR t.TagName LIKE @Search)";
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@LanguageID", languageId);
                    if (courseType != "All")
                    {
                        cmd.Parameters.AddWithValue("@CourseType", courseType);
                    }
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
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
                        });
                    }
                }
            }
            return courses;
        }

        public List<Category> GetAllCategories(string searchTerm, int languageId)
        {
            List<Category> categories = new List<Category>();
            string sql = @"
                SELECT DISTINCT c.CategoryID, c.CategoryName, c.ImagePath 
                FROM Category c
                LEFT JOIN CategoryTagLink ctl ON c.CategoryID = ctl.CategoryID
                LEFT JOIN Tags t ON ctl.TagID = t.TagID
                WHERE c.isActivated = 1 AND c.LanguageID = @LanguageID";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " AND (c.CategoryName LIKE @Search OR t.TagName LIKE @Search)";
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@LanguageID", languageId);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
                    }

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            CategoryID = (int)reader["CategoryID"],
                            CategoryName = (string)reader["CategoryName"],
                            ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null
                        });
                    }
                }
            }
            return categories;
        }

        public List<Course> GetCoursesByCategoryID(int categoryId, string courseType, string searchTerm, int languageId)
        {
            List<Course> courses = new List<Course>();
            string sql = @"
                SELECT DISTINCT c.CourseID, c.CourseName, c.Description, c.ImagePath, c.CourseType 
                FROM Courses c
                LEFT JOIN CourseTagLink ctl ON c.CourseID = ctl.CourseID
                LEFT JOIN Tags t ON ctl.TagID = t.TagID
                WHERE c.CategoryID = @CategoryID AND c.isActivated = 1 AND c.LanguageID = @LanguageID";

            if (courseType != "All")
            {
                sql += " AND c.CourseType = @CourseType";
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " AND (c.CourseName LIKE @Search OR c.Description LIKE @Search OR t.TagName LIKE @Search)";
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    cmd.Parameters.AddWithValue("@LanguageID", languageId);
                    if (courseType != "All")
                    {
                        cmd.Parameters.AddWithValue("@CourseType", courseType);
                    }
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
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
                        });
                    }
                }
            }
            return courses;
        }

        // Overloads for backward compatibility
        public List<Course> GetCoursesByCategoryID(int categoryId)
        {
            return GetCoursesByCategoryID(categoryId, "All", null, 1);
        }

        public List<Course> GetAllCourses()
        {
            return GetAllCourses("All", null, 1);
        }

        // =============================================
        // DETAILS & BASIC READS
        // =============================================

        public Category GetCategoryDetails(int categoryId)
        {
            Category category = null;
            string sql = "SELECT CategoryID, CategoryName, ImagePath, LanguageID, isActivated FROM Category WHERE CategoryID = @CategoryID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        category = new Category
                        {
                            CategoryID = (int)reader["CategoryID"],
                            CategoryName = (string)reader["CategoryName"],
                            ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null,
                            LanguageID = (int)reader["LanguageID"],
                            isActivated = (bool)reader["isActivated"]
                        };
                    }
                }
            }
            return category;
        }

        public Course GetCourseDetails(int courseId)
        {
            Course course = null;
            string sql = "SELECT CourseID, CourseName, Description, ImagePath, CategoryID, CourseType, LanguageID, isActivated FROM Courses WHERE CourseID = @CourseID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        course = new Course
                        {
                            CourseID = (int)reader["CourseID"],
                            CourseName = (string)reader["CourseName"],
                            Description = reader["Description"] != DBNull.Value ? (string)reader["Description"] : null,
                            ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null,
                            CourseType = (string)reader["CourseType"],
                            CategoryID = (int)reader["CategoryID"],
                            LanguageID = (int)reader["LanguageID"],
                            isActivated = (bool)reader["isActivated"]
                        };
                    }
                }
            }
            return course;
        }

        public List<Vocabulary> GetVocabByCourseID(int courseId)
        {
            List<Vocabulary> vocabList = new List<Vocabulary>();
            string sql = "SELECT VocabID, VocabText, Meaning, ImagePath, SequenceOrder FROM Vocabulary WHERE CourseID = @CourseID AND isActivated = 1 ORDER BY SequenceOrder";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        vocabList.Add(new Vocabulary
                        {
                            VocabID = (int)reader["VocabID"],
                            VocabText = (string)reader["VocabText"],
                            Meaning = (string)reader["Meaning"],
                            ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null,
                            SequenceOrder = (int)reader["SequenceOrder"]
                        });
                    }
                }
            }
            return vocabList;
        }

        public List<Phrase> GetPhrasesByCourseID(int courseId)
        {
            List<Phrase> phraseList = new List<Phrase>();
            string sql = "SELECT PhraseID, PhraseText, Meaning, SequenceOrder FROM Phrase WHERE CourseID = @CourseID AND isActivated = 1 ORDER BY SequenceOrder";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        phraseList.Add(new Phrase
                        {
                            PhraseID = (int)reader["PhraseID"],
                            PhraseText = (string)reader["PhraseText"],
                            Meaning = (string)reader["Meaning"],
                            SequenceOrder = (int)reader["SequenceOrder"]
                        });
                    }
                }
            }
            foreach (Phrase p in phraseList)
            {
                p.Details = GetPhraseDetails(p.PhraseID);
            }
            return phraseList;
        }

        public List<PhraseDetail> GetPhraseDetails(int phraseId)
        {
            List<PhraseDetail> detailList = new List<PhraseDetail>();
            string sql = "SELECT PhraseDetailID, DetailType, Content FROM PhraseDetail WHERE PhraseID = @PhraseID ORDER BY PhraseDetailID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PhraseID", phraseId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        detailList.Add(new PhraseDetail
                        {
                            PhraseDetailID = (int)reader["PhraseDetailID"],
                            DetailType = (string)reader["DetailType"],
                            Content = (string)reader["Content"]
                        });
                    }
                }
            }
            return detailList;
        }

        // =============================================
        // ADMIN PANEL - CATEGORY & LANGUAGE MGMT
        // =============================================

        public DataTable GetAllCategoriesForAdmin()
        {
            DataTable dt = new DataTable();
            string sql = @"
                SELECT c.CategoryID, c.CategoryName, c.ImagePath, l.LanguageName, c.isActivated, c.LanguageID 
                FROM Category c
                LEFT JOIN Language l ON c.LanguageID = l.LanguageID
                ORDER BY l.LanguageName, c.CategoryName";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public void DeactivatedCategory(int categoryId)
        {
            string sql = "UPDATE Category SET isActivated = 0 WHERE CategoryID = @CategoryID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ActivateCategory(int categoryId)
        {
            string sql = "UPDATE Category SET isActivated = 1 WHERE CategoryID = @CategoryID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable GetAllLanguages()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT LanguageID, LanguageName FROM Language ORDER BY LanguageName";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public bool CreateCategory(string categoryName, int languageId, string imagePath)
        {
            string sql = "INSERT INTO Category (CategoryName, LanguageID, ImagePath) VALUES (@CategoryName, @LanguageID, @ImagePath)";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                    cmd.Parameters.AddWithValue("@LanguageID", languageId);

                    if (string.IsNullOrEmpty(imagePath))
                    {
                        cmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    }

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateCategory(int categoryId, string categoryName, int languageId, string imagePath)
        {
            string sql = "UPDATE Category SET CategoryName = @CategoryName, LanguageID = @LanguageID";
            if (imagePath != null)
            {
                sql += ", ImagePath = @ImagePath";
            }
            sql += " WHERE CategoryID = @CategoryID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                    cmd.Parameters.AddWithValue("@LanguageID", languageId);

                    if (imagePath != null)
                    {
                        cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    }

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable GetCategoriesByLanguage(int languageId)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT CategoryID, CategoryName FROM Category WHERE LanguageID = @LanguageID AND isActivated = 1 ORDER BY CategoryName";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@LanguageID", languageId);
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // =============================================
        // ADMIN PANEL - COURSE MGMT
        // =============================================

        public DataTable GetAllCoursesForAdmin()
        {
            DataTable dt = new DataTable();
            string sql = @"
                SELECT 
                    c.CourseID, c.ImagePath, c.CourseName, cat.CategoryName, 
                    l.LanguageName, c.CourseType, c.isActivated
                FROM Courses c
                LEFT JOIN Category cat ON c.CategoryID = cat.CategoryID
                LEFT JOIN Language l ON c.LanguageID = l.LanguageID
                ORDER BY c.CourseID ASC";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public bool CreateCourse(string name, string desc, int langId, int catId, string courseType, string imagePath)
        {
            string sql = @"
                INSERT INTO Courses (CourseName, Description, LanguageID, CategoryID, CourseType, ImagePath) 
                VALUES (@Name, @Desc, @LangID, @CatID, @Type, @Image)";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Desc", desc);
                    cmd.Parameters.AddWithValue("@LangID", langId);
                    cmd.Parameters.AddWithValue("@CatID", catId);
                    cmd.Parameters.AddWithValue("@Type", courseType);

                    if (string.IsNullOrEmpty(imagePath))
                        cmd.Parameters.AddWithValue("@Image", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@Image", imagePath);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateCourse(int courseId, string name, string desc, int langId, int catId, string courseType, string imagePath)
        {
            string sql = @"
                UPDATE Courses SET 
                    CourseName = @Name, Description = @Desc, LanguageID = @LangID, 
                    CategoryID = @CatID, CourseType = @Type";

            if (imagePath != null)
            {
                sql += ", ImagePath = @Image";
            }

            sql += " WHERE CourseID = @CourseID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Desc", desc);
                    cmd.Parameters.AddWithValue("@LangID", langId);
                    cmd.Parameters.AddWithValue("@CatID", catId);
                    cmd.Parameters.AddWithValue("@Type", courseType);

                    if (imagePath != null)
                    {
                        cmd.Parameters.AddWithValue("@Image", imagePath);
                    }

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public void DeactivatedCourse(int courseId)
        {
            string sql = "UPDATE Courses SET isActivated = 0 WHERE CourseID = @CourseID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetTotalActiveCourseCount()
        {
            string sql = "SELECT COUNT(CourseID) FROM Courses WHERE isActivated = 1";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public int GetTotalActiveCategoryCount()
        {
            string sql = "SELECT COUNT(CategoryID) FROM Category WHERE isActivated = 1";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public void ActivateCourse(int courseId)
        {
            string sql = "UPDATE Courses SET isActivated = 1 WHERE CourseID = @CourseID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // =============================================
        // LEARNING ITEM MANAGEMENT (Vocab & Phrase Unified)
        // =============================================

        public List<LearningItem> GetCourseContent(int courseId)
        {
            List<LearningItem> items = new List<LearningItem>();
            // 1. Get Vocab
            string sqlVocab = "SELECT VocabID, VocabText, Meaning, ImagePath, SequenceOrder FROM Vocabulary WHERE CourseID = @CourseID AND isActivated = 1";
            // 2. Get Phrases
            string sqlPhrase = "SELECT PhraseID, PhraseText, Meaning, SequenceOrder FROM Phrase WHERE CourseID = @CourseID AND isActivated = 1";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Fetch Vocab
                using (SqlCommand cmd = new SqlCommand(sqlVocab, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        items.Add(new LearningItem
                        {
                            ItemID = (int)r["VocabID"],
                            ItemType = "Vocabulary",
                            VocabText = r["VocabText"].ToString(),
                            VocabMeaning = r["Meaning"].ToString(),
                            VocabImagePath = r["ImagePath"] != DBNull.Value ? r["ImagePath"].ToString() : null,
                            SequenceOrder = (int)r["SequenceOrder"]
                        });
                    }
                    r.Close();
                }

                // Fetch Phrases
                using (SqlCommand cmd = new SqlCommand(sqlPhrase, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        items.Add(new LearningItem
                        {
                            ItemID = (int)r["PhraseID"],
                            ItemType = "Phrase",
                            PhraseText = r["PhraseText"].ToString(),
                            PhraseMeaning = r["Meaning"].ToString(),
                            SequenceOrder = (int)r["SequenceOrder"]
                        });
                    }
                }
            }

            return items.OrderBy(i => i.SequenceOrder).ToList();
        }

        // *** UPDATED METHOD: Returns INT (SCOPE_IDENTITY) for Auto-Expand ***
        public int CreateLearningItem(int courseId, string type, int sequence)
        {
            string sql;
            if (type == "Vocabulary")
                sql = "INSERT INTO Vocabulary (CourseID, VocabText, Meaning, SequenceOrder, isActivated) VALUES (@CID, 'New Word', 'Meaning', @Seq, 1); SELECT SCOPE_IDENTITY();";
            else
                sql = "INSERT INTO Phrase (CourseID, PhraseText, Meaning, SequenceOrder, isActivated) VALUES (@CID, 'New Phrase', 'Meaning', @Seq, 1); SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CID", courseId);
                    cmd.Parameters.AddWithValue("@Seq", sequence);
                    conn.Open();
                    // Use ExecuteScalar to get the new ID
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public void UpdateVocabulary(int vocabId, string text, string meaning, string imagePath)
        {
            string sql = "UPDATE Vocabulary SET VocabText=@T, Meaning=@M";
            if (imagePath != null) sql += ", ImagePath=@I";
            sql += " WHERE VocabID=@ID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", vocabId);
                    cmd.Parameters.AddWithValue("@T", text);
                    cmd.Parameters.AddWithValue("@M", meaning);
                    if (imagePath != null) cmd.Parameters.AddWithValue("@I", imagePath);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePhrase(int phraseId, string text, string meaning)
        {
            string sql = "UPDATE Phrase SET PhraseText=@T, Meaning=@M WHERE PhraseID=@ID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", phraseId);
                    cmd.Parameters.AddWithValue("@T", text);
                    cmd.Parameters.AddWithValue("@M", meaning);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteLearningItem(int id, string type, int courseId)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                // 1. Deactivate the item
                string table = type == "Vocabulary" ? "Vocabulary" : "Phrase";
                string pk = type == "Vocabulary" ? "VocabID" : "PhraseID";
                string sql = $"UPDATE {table} SET isActivated = 0 WHERE {pk} = @ID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            // 2. Re-sequence everything else
            ResequenceCourseItems(courseId);
        }

        // =============================================
        // PHRASE DETAIL MANAGEMENT
        // =============================================

        // *** UPDATED METHOD: Uses empty strings for Placeholders ***
        public void AddPhraseDetail(int phraseId, string type, string content)
        {
            string sql = "INSERT INTO PhraseDetail (PhraseID, DetailType, Content) VALUES (@PID, @Type, @Content)";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PID", phraseId);
                    // Use empty string if null, allowing placeholder to show in UI
                    cmd.Parameters.AddWithValue("@Type", type ?? "");
                    cmd.Parameters.AddWithValue("@Content", content ?? "");
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePhraseDetail(int detailId, string type, string content)
        {
            string sql = "UPDATE PhraseDetail SET DetailType = @Type, Content = @Content WHERE PhraseDetailID = @ID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", detailId);
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Content", content);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeletePhraseDetail(int detailId)
        {
            string sql = "DELETE FROM PhraseDetail WHERE PhraseDetailID = @ID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", detailId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // =============================================
        // REORDERING LOGIC
        // =============================================

        public void ReorderLearningItem(int courseId, int itemId, string itemType, string direction)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Get current sequence
                    string table = (itemType == "Vocabulary") ? "Vocabulary" : "Phrase";
                    string pkCol = (itemType == "Vocabulary") ? "VocabID" : "PhraseID";

                    int currentSeq = 0;
                    string getSeqSql = $"SELECT SequenceOrder FROM {table} WHERE {pkCol} = @ID";
                    using (SqlCommand cmd = new SqlCommand(getSeqSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ID", itemId);
                        currentSeq = (int)cmd.ExecuteScalar();
                    }

                    // 2. Find target sequence
                    int targetSeq = (direction == "UP") ? currentSeq - 1 : currentSeq + 1;
                    if (targetSeq <= 0) return; // Can't move up past 1

                    // 3. Find the OTHER item at that target sequence (could be Vocab OR Phrase!)
                    int otherId = 0;
                    string otherType = "";

                    // Check Vocab First
                    string checkVocab = "SELECT VocabID FROM Vocabulary WHERE CourseID=@CID AND SequenceOrder=@Seq AND isActivated=1";
                    using (SqlCommand cmd = new SqlCommand(checkVocab, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CID", courseId);
                        cmd.Parameters.AddWithValue("@Seq", targetSeq);
                        object res = cmd.ExecuteScalar();
                        if (res != null) { otherId = (int)res; otherType = "Vocabulary"; }
                    }

                    // If not found, Check Phrase
                    if (otherId == 0)
                    {
                        string checkPhrase = "SELECT PhraseID FROM Phrase WHERE CourseID=@CID AND SequenceOrder=@Seq AND isActivated=1";
                        using (SqlCommand cmd = new SqlCommand(checkPhrase, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CID", courseId);
                            cmd.Parameters.AddWithValue("@Seq", targetSeq);
                            object res = cmd.ExecuteScalar();
                            if (res != null) { otherId = (int)res; otherType = "Phrase"; }
                        }
                    }

                    // 4. Swap
                    if (otherId > 0)
                    {
                        // Move OTHER to CURRENT sequence
                        string otherTable = (otherType == "Vocabulary") ? "Vocabulary" : "Phrase";
                        string otherPk = (otherType == "Vocabulary") ? "VocabID" : "PhraseID";
                        string updateOther = $"UPDATE {otherTable} SET SequenceOrder = @NewSeq WHERE {otherPk} = @ID";
                        using (SqlCommand cmd = new SqlCommand(updateOther, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@NewSeq", currentSeq);
                            cmd.Parameters.AddWithValue("@ID", otherId);
                            cmd.ExecuteNonQuery();
                        }

                        // Move CURRENT to TARGET sequence
                        string updateCurrent = $"UPDATE {table} SET SequenceOrder = @NewSeq WHERE {pkCol} = @ID";
                        using (SqlCommand cmd = new SqlCommand(updateCurrent, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@NewSeq", targetSeq);
                            cmd.Parameters.AddWithValue("@ID", itemId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch { transaction.Rollback(); throw; }
            }
        }

        public void ResequenceCourseItems(int courseId)
        {
            var items = GetCourseContent(courseId); // This gets them in current order
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                for (int i = 0; i < items.Count; i++)
                {
                    int newSeq = i + 1;
                    string table = items[i].ItemType == "Vocabulary" ? "Vocabulary" : "Phrase";
                    string pk = items[i].ItemType == "Vocabulary" ? "VocabID" : "PhraseID";

                    string sql = $"UPDATE {table} SET SequenceOrder = @Seq WHERE {pk} = @ID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Seq", newSeq);
                        cmd.Parameters.AddWithValue("@ID", items[i].ItemID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}