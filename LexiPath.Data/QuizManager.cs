using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LexiPath.Data
{
    public class QuizManager
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["LexiPathDB"].ConnectionString;
        }

        // --- QUIZ RETRIEVAL ---

        /**
         * Gets all quizzes, with optional search filter (Title, Description, or Tag)
         * and optional IsPractice filter.
         */
        /**
         * UPDATED: Now filters by LanguageID
         */
        public List<Quiz> GetAllQuizzes(string searchTerm, bool? isPractice, int languageId)
        {
            List<Quiz> quizzes = new List<Quiz>();
            string sql = @"
                SELECT DISTINCT q.QuizID, q.Title, q.Description, q.ImagePath, q.IsPractice
                FROM Quiz q
                LEFT JOIN QuizTagLink qtl ON q.QuizID = qtl.QuizID
                LEFT JOIN Tags t ON qtl.TagID = t.TagID
                WHERE q.LanguageID = @LanguageID AND q.isActivated = 1"; // <-- LANGUAGE FILTER

            if (isPractice.HasValue)
            {
                sql += " AND q.IsPractice = @IsPractice";
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " AND (q.Title LIKE @Search OR q.Description LIKE @Search OR t.TagName LIKE @Search)";
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@LanguageID", languageId); // <-- ADDED
                    if (isPractice.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@IsPractice", isPractice.Value);
                    }
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
                    }

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        quizzes.Add(new Quiz
                        {
                            QuizID = (int)reader["QuizID"],
                            Title = (string)reader["Title"],
                            Description = reader["Description"] != DBNull.Value ? (string)reader["Description"] : null,
                            ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null,
                            IsPractice = (bool)reader["IsPractice"]
                        });
                    }
                }
            }
            return quizzes;
        }

        private List<Course> GetQuizRelatedCourses(int quizId)
        {
            List<Course> courses = new List<Course>();
            string sql = @"
                SELECT c.CourseID, c.CourseName
                FROM Courses c
                INNER JOIN QuizRelatedCourse qrc ON c.CourseID = qrc.CourseID
                WHERE qrc.QuizID = @QuizID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseID = (int)reader["CourseID"],
                            CourseName = (string)reader["CourseName"]
                        });
                    }
                }
            }
            return courses;
        }

        /**
         * Gets all questions for a specific quiz, ordered by sequence.
         */
        public List<QuizQuestion> GetQuizQuestions(int quizId)
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();
            string sql = @"
                SELECT qq.QuestionID, qq.QuestionText, qq.QuestionTypeID, qt.TypeName AS QuestionTypeName, qq.CorrectAnswer, qq.ImagePath, qq.SequenceOrder
                FROM QuizQuestion qq
                INNER JOIN QuestionType qt ON qq.QuestionTypeID = qt.QuestionTypeID
                WHERE qq.QuizID = @QuizID
                ORDER BY qq.SequenceOrder";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        QuizQuestion question = new QuizQuestion
                        {
                            QuestionID = (int)reader["QuestionID"],
                            QuestionText = (string)reader["QuestionText"],
                            QuestionTypeID = (int)reader["QuestionTypeID"],
                            QuestionTypeName = (string)reader["QuestionTypeName"],
                            CorrectAnswer = reader["CorrectAnswer"] != DBNull.Value ? (string)reader["CorrectAnswer"] : null,
                            ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null,
                            SequenceOrder = (int)reader["SequenceOrder"]
                        };
                        question.Options = GetQuizAnswerOptions(question.QuestionID);
                        questions.Add(question);
                    }
                }
            }
            return questions;
        }

        private List<QuizAnswerOption> GetQuizAnswerOptions(int questionId)
        {
            List<QuizAnswerOption> options = new List<QuizAnswerOption>();
            string sql = "SELECT OptionID, OptionText, IsCorrect, OptionValue FROM QuizAnswerOption WHERE QuestionID = @QuestionID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionID", questionId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        options.Add(new QuizAnswerOption
                        {
                            OptionID = (int)reader["OptionID"],
                            OptionText = (string)reader["OptionText"],
                            IsCorrect = (bool)reader["IsCorrect"],
                            OptionValue = reader["OptionValue"] != DBNull.Value ? (string)reader["OptionValue"] : null
                        });
                    }
                }
            }
            return options;
        }

        // --- ATTEMPT MANAGEMENT ---

        /**
         * Saves a quiz attempt for a registered user.
         */
        public void SaveQuizAttempt(int quizId, int userId, int score)
        {
            string sql = "INSERT INTO QuizAttempt (QuizID, UserID, Score, AttemptedAt) VALUES (@QuizID, @UserID, @Score, GETDATE())";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Score", score);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /**
         * NEW (READ): Gets all Quizzes/Practices for the admin grid
         */
        public DataTable GetAllQuizzesForAdmin()
        {
            DataTable dt = new DataTable();
            string sql = @"
                SELECT 
                    q.QuizID, q.Title, q.Description, q.ImagePath, l.LanguageName, 
                    q.IsPractice, q.isActivated,
                    STUFF((SELECT ', ' + C2.CourseName
                           FROM QuizRelatedCourse QRC2
                           JOIN Courses C2 ON QRC2.CourseID = C2.CourseID
                           WHERE QRC2.QuizID = q.QuizID
                           FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS RelatedCourses,
                    STUFF((SELECT ', ' + T2.TagName
                           FROM QuizTagLink QTL2
                           JOIN Tags T2 ON QTL2.TagID = T2.TagID
                           WHERE QTL2.QuizID = q.QuizID
                           FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS RelatedTags
                FROM Quiz q
                LEFT JOIN Language l ON q.LanguageID = l.LanguageID
                ORDER BY q.QuizID ASC";

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

        /**
         * UPDATED (READ): Gets details for a single quiz/practice for the Edit Modal
         */
        public Quiz GetQuizDetails(int quizId)
        {
            Quiz quiz = null;
            string sql = @"
                SELECT 
                    q.QuizID, q.Title, q.Description, q.LanguageID, q.IsPractice, q.isActivated, q.ImagePath,
                    STUFF((SELECT ',' + CONVERT(NVARCHAR(MAX), QRC2.CourseID)
                           FROM QuizRelatedCourse QRC2
                           WHERE QRC2.QuizID = q.QuizID
                           FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS RelatedCourseIDs,
                    STUFF((SELECT ',' + CONVERT(NVARCHAR(MAX), QTL2.TagID)
                           FROM QuizTagLink QTL2
                           WHERE QTL2.QuizID = q.QuizID
                           FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS RelatedTagIDs
                FROM Quiz q
                WHERE q.QuizID = @QuizID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quiz = new Quiz
                            {
                                QuizID = (int)reader["QuizID"],
                                Title = (string)reader["Title"],
                                Description = (string)reader["Description"],
                                LanguageID = (int)reader["LanguageID"],
                                IsPractice = (bool)reader["IsPractice"],
                                isActivated = (bool)reader["isActivated"],
                                ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : null,
                                RelatedCourseIDs = reader["RelatedCourseIDs"] != DBNull.Value ? (string)reader["RelatedCourseIDs"] : null,
                                RelatedTagIDs = reader["RelatedTagIDs"] != DBNull.Value ? (string)reader["RelatedTagIDs"] : null
                            };
                        }
                    }
                }
            }

            // --- THIS IS THE CRITICAL FIX ---
            // If we found a quiz, now we go get its questions
            if (quiz != null)
            {
                quiz.Questions = GetQuizQuestions(quizId);
                quiz.RelatedCourses = GetQuizRelatedCourses(quizId); // Also get related courses
            }
            // --- END OF FIX ---

            return quiz;
        }


        /**
         * NEW (CREATE): Inserts a new Quiz/Practice
         */
        public bool CreateQuiz(string title, string description, int languageId, bool isPractice, string imagePath, int[] selectedCourseIds, int[] selectedTagIds)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    string quizSql = "INSERT INTO Quiz (Title, Description, LanguageID, IsPractice, ImagePath) VALUES (@Title, @Description, @LanguageID, @IsPractice, @ImagePath); SELECT SCOPE_IDENTITY();";
                    int quizId;
                    using (SqlCommand cmd = new SqlCommand(quizSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@LanguageID", languageId);
                        cmd.Parameters.AddWithValue("@IsPractice", isPractice);

                        if (string.IsNullOrEmpty(imagePath))
                            cmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@ImagePath", imagePath);

                        quizId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // --- Link Courses ---
                    if (selectedCourseIds != null && selectedCourseIds.Any())
                    {
                        foreach (int courseId in selectedCourseIds)
                        {
                            string courseLinkSql = "INSERT INTO QuizRelatedCourse (QuizID, CourseID) VALUES (@QuizID, @CourseID)";
                            using (SqlCommand cmd = new SqlCommand(courseLinkSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", quizId);
                                cmd.Parameters.AddWithValue("@CourseID", courseId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // --- Link Tags ---
                    if (selectedTagIds != null && selectedTagIds.Any())
                    {
                        foreach (int tagId in selectedTagIds)
                        {
                            string tagLinkSql = "INSERT INTO QuizTagLink (QuizID, TagID) VALUES (@QuizID, @TagID)";
                            using (SqlCommand cmd = new SqlCommand(tagLinkSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", quizId);
                                cmd.Parameters.AddWithValue("@TagID", tagId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        /**
         * NEW (UPDATE): Updates an existing Quiz/Practice
         */
        public bool UpdateQuiz(int quizId, string title, string description, int languageId, bool isPractice, string imagePath, int[] selectedCourseIds, int[] selectedTagIds)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Update Quiz main table
                    string quizSql = "UPDATE Quiz SET Title = @Title, Description = @Description, LanguageID = @LanguageID, IsPractice = @IsPractice";
                    if (imagePath != null) // Only update image if a new one is provided
                    {
                        quizSql += ", ImagePath = @ImagePath";
                    }
                    quizSql += " WHERE QuizID = @QuizID";

                    using (SqlCommand cmd = new SqlCommand(quizSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", quizId);
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@LanguageID", languageId);
                        cmd.Parameters.AddWithValue("@IsPractice", isPractice);
                        if (imagePath != null)
                        {
                            cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                        }
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Update QuizRelatedCourse (clear existing, insert new)
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM QuizRelatedCourse WHERE QuizID = @QuizID", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", quizId);
                        cmd.ExecuteNonQuery();
                    }
                    if (selectedCourseIds != null && selectedCourseIds.Any())
                    {
                        foreach (int courseId in selectedCourseIds)
                        {
                            string courseLinkSql = "INSERT INTO QuizRelatedCourse (QuizID, CourseID) VALUES (@QuizID, @CourseID)";
                            using (SqlCommand cmd = new SqlCommand(courseLinkSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", quizId);
                                cmd.Parameters.AddWithValue("@CourseID", courseId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // 3. Update QuizTagLink (clear existing, insert new)
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM QuizTagLink WHERE QuizID = @QuizID", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", quizId);
                        cmd.ExecuteNonQuery();
                    }
                    if (selectedTagIds != null && selectedTagIds.Any())
                    {
                        foreach (int tagId in selectedTagIds)
                        {
                            string tagLinkSql = "INSERT INTO QuizTagLink (QuizID, TagID) VALUES (@QuizID, @TagID)";
                            using (SqlCommand cmd = new SqlCommand(tagLinkSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", quizId);
                                cmd.Parameters.AddWithValue("@TagID", tagId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        /**
         * NEW (DELETE): Marks a Quiz/Practice as deleted
         */
        public void DeactivatedQuiz(int quizId)
        {
            // We'll use a soft delete
            string sql = "UPDATE Quiz SET isActivated = 0 WHERE QuizID = @QuizID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /**
         * NEW: Activates a quiz
         */
        public void ActivateQuiz(int quizId)
        {
            string sql = "UPDATE Quiz SET isActivated = 1 WHERE QuizID = @QuizID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- Helper methods to get data for dropdowns ---

        public DataTable GetAllCourses()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT CourseID, CourseName FROM Courses WHERE isActivated = 1 ORDER BY CourseName";
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

        public DataTable GetAllTags()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT TagID, TagName FROM Tags ORDER BY TagName";
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

        // You might want methods to get user's past attempts/high scores later

        /**
         * NEW: Gets all Quiz IDs a user has attempted.
         * Returns a HashSet for very fast lookups.
         */
        public HashSet<int> GetAttemptedQuizIds(int userId)
        {
            HashSet<int> quizIds = new HashSet<int>();
            string sql = "SELECT DISTINCT QuizID FROM QuizAttempt WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        quizIds.Add((int)reader["QuizID"]);
                    }
                }
            }
            return quizIds;
        }

        /**
         * NEW: Finds the Practice ID associated with a specific Course ID.
         * Returns 0 if no practice is found.
         */
        public int GetPracticeQuizIDByCourseID(int courseId)
        {
            int practiceQuizId = 0; // Default to 0 (not found)

            // This query finds the one quiz that is a "Practice" AND
            // is linked to the specific Course ID in the junction table.
            string sql = @"
                SELECT TOP 1 q.QuizID 
                FROM Quiz q
                INNER JOIN QuizRelatedCourse qrc ON q.QuizID = qrc.QuizID
                WHERE qrc.CourseID = @CourseID AND q.IsPractice = 1";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();

                    // ExecuteScalar is perfect for getting just one value
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        practiceQuizId = (int)result;
                    }
                }
            }
            return practiceQuizId;
        }

        /**
         * NEW: Gets the total number of quizzes a user has attempted.
         */
        public int GetQuizAttemptCount(int userId)
        {
            string sql = "SELECT COUNT(*) FROM QuizAttempt WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        /**
         * NEW (READ): Gets all available Question Types for a dropdown
         */
        public DataTable GetQuestionTypes()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT QuestionTypeID, TypeName FROM QuestionType ORDER BY TypeName";
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

        public List<Quiz> GetActivePracticesByCourseID(int courseId)
        {
            List<Quiz> practices = new List<Quiz>();
            string sql = @"
        SELECT q.QuizID, q.Title, q.Description 
        FROM Quiz q
        INNER JOIN QuizRelatedCourse qrc ON q.QuizID = qrc.QuizID
        WHERE qrc.CourseID = @CourseID 
          AND q.IsPractice = 1 
          AND q.isActivated = 1  -- FIX: Ensure only active practices show
        ORDER BY q.Title";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        practices.Add(new Quiz
                        {
                            QuizID = (int)reader["QuizID"],
                            Title = (string)reader["Title"],
                            Description = reader["Description"] != DBNull.Value ? (string)reader["Description"] : ""
                        });
                    }
                }
            }
            return practices;
        }

        public int GetRealQuizAttemptCount(int userId)
        {
            // Join QuizAttempt with Quiz table to filter by IsPractice
            string sql = @"
                SELECT COUNT(qa.QuizID) 
                FROM QuizAttempt qa
                INNER JOIN Quiz q ON qa.QuizID = q.QuizID
                WHERE qa.UserID = @UserID AND q.IsPractice = 0";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        #region Question & Option Management (for EditQuiz.aspx)

        /**
         * NEW: Gets the string name of a question type
         */
        public string GetQuestionTypeNameById(int questionTypeID)
        {
            string sql = "SELECT TypeName FROM QuestionType WHERE QuestionTypeID = @QuestionTypeID";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionTypeID", questionTypeID);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : null;
                }
            }
        }

        /**
         * NEW: Adds a new question to a quiz.
         * If the question is an MCQ, it adds two blank options by default.
         */
        public bool CreateQuestion(int quizId, string questionText, int questionTypeID, string imagePath)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Get the next sequence order
                    int nextSequence;
                    string seqSql = "SELECT ISNULL(MAX(SequenceOrder), 0) + 1 FROM QuizQuestion WHERE QuizID = @QuizID";
                    using (SqlCommand seqCmd = new SqlCommand(seqSql, conn, transaction))
                    {
                        seqCmd.Parameters.AddWithValue("@QuizID", quizId);
                        nextSequence = (int)seqCmd.ExecuteScalar();
                    }

                    // 2. Insert the new question
                    string questionSql = @"
                        INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, ImagePath, SequenceOrder)
                        VALUES (@QuizID, @QuestionTypeID, @QuestionText, @ImagePath, @SequenceOrder);
                        SELECT SCOPE_IDENTITY();";

                    int newQuestionId;
                    using (SqlCommand cmd = new SqlCommand(questionSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", quizId);
                        cmd.Parameters.AddWithValue("@QuestionTypeID", questionTypeID);
                        cmd.Parameters.AddWithValue("@QuestionText", questionText);
                        cmd.Parameters.AddWithValue("@SequenceOrder", nextSequence);

                        if (string.IsNullOrEmpty(imagePath))
                            cmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@ImagePath", imagePath);

                        newQuestionId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 3. If it's an MCQ, add default options
                    string typeName = GetQuestionTypeNameById(questionTypeID);
                    if (typeName == "MCQ")
                    {
                        string optSql = "INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@QuestionID, @OptionText, 0)";

                        // Add option 1
                        using (SqlCommand optCmd = new SqlCommand(optSql, conn, transaction))
                        {
                            optCmd.Parameters.AddWithValue("@QuestionID", newQuestionId);
                            optCmd.Parameters.AddWithValue("@OptionText", "New Option 1");
                            optCmd.ExecuteNonQuery();
                        }
                        // Add option 2
                        using (SqlCommand optCmd = new SqlCommand(optSql, conn, transaction))
                        {
                            optCmd.Parameters.AddWithValue("@QuestionID", newQuestionId);
                            optCmd.Parameters.AddWithValue("@OptionText", "New Option 2");
                            optCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // You should log the exception (ex) here
                    return false;
                }
            }
        }

        /**
         * NEW: Deletes a question and its options.
         * Then re-orders the remaining questions to fill the sequence gap.
         */
        public bool DeleteQuestion(int questionId)
        {
            int quizId = 0;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Get the QuizID for re-sequencing later
                    string getQuizSql = "SELECT QuizID FROM QuizQuestion WHERE QuestionID = @QuestionID";
                    using (SqlCommand cmd = new SqlCommand(getQuizSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuestionID", questionId);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            quizId = (int)result;
                        }
                        else
                        {
                            throw new Exception("Question not found."); // Abort transaction
                        }
                    }

                    // 2. Delete all related answer options (cascade)
                    string optSql = "DELETE FROM QuizAnswerOption WHERE QuestionID = @QuestionID";
                    using (SqlCommand cmd = new SqlCommand(optSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuestionID", questionId);
                        cmd.ExecuteNonQuery();
                    }

                    // 3. Delete the question itself
                    string qSql = "DELETE FROM QuizQuestion WHERE QuestionID = @QuestionID";
                    using (SqlCommand cmd = new SqlCommand(qSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuestionID", questionId);
                        cmd.ExecuteNonQuery();
                    }

                    // 4. Re-sequence the remaining questions for that quiz
                    string reorderSql = @"
                        WITH OrderedQuestions AS (
                            SELECT QuestionID, ROW_NUMBER() OVER (ORDER BY SequenceOrder) AS NewSeq
                            FROM QuizQuestion
                            WHERE QuizID = @QuizID
                        )
                        UPDATE q
                        SET q.SequenceOrder = oq.NewSeq
                        FROM QuizQuestion q
                        JOIN OrderedQuestions oq ON q.QuestionID = oq.QuestionID
                        WHERE q.QuizID = @QuizID";

                    using (SqlCommand cmd = new SqlCommand(reorderSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", quizId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log ex
                    return false;
                }
            }
        }

        /**
         * NEW: Swaps the sequence order of a question with its neighbor (Up or Down).
         */
        public bool ReorderQuestion(int quizId, int questionId, string direction)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Get current sequence and the target sequence/ID to swap with
                    int currentSeq = 0;
                    int otherQID = 0;
                    int otherSeq = 0;

                    string getSeqSql = "SELECT SequenceOrder FROM QuizQuestion WHERE QuestionID = @QuestionID AND QuizID = @QuizID";
                    using (SqlCommand cmd = new SqlCommand(getSeqSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuestionID", questionId);
                        cmd.Parameters.AddWithValue("@QuizID", quizId);
                        currentSeq = (int)cmd.ExecuteScalar();
                    }

                    string getOtherSql = "";
                    if (direction == "UP")
                    {
                        otherSeq = currentSeq - 1;
                        // Get the ID of the question that is 1 position *before* this one
                        getOtherSql = "SELECT QuestionID FROM QuizQuestion WHERE SequenceOrder = @OtherSeq AND QuizID = @QuizID";
                    }
                    else // "DOWN"
                    {
                        otherSeq = currentSeq + 1;
                        // Get the ID of the question that is 1 position *after* this one
                        getOtherSql = "SELECT QuestionID FROM QuizQuestion WHERE SequenceOrder = @OtherSeq AND QuizID = @QuizID";
                    }

                    // No need to swap if it's already first or last
                    if (otherSeq <= 0)
                    {
                        transaction.Rollback();
                        return true; // Not an error, just no action
                    }

                    using (SqlCommand cmd = new SqlCommand(getOtherSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OtherSeq", otherSeq);
                        cmd.Parameters.AddWithValue("@QuizID", quizId);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            otherQID = (int)result;
                        }
                    }

                    // 2. If a question to swap with was found, perform the swap
                    if (otherQID > 0)
                    {
                        // Move our question to the other's sequence
                        string update1 = "UPDATE QuizQuestion SET SequenceOrder = @NewSeq WHERE QuestionID = @QID";
                        using (SqlCommand cmd = new SqlCommand(update1, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@NewSeq", otherSeq);
                            cmd.Parameters.AddWithValue("@QID", questionId);
                            cmd.ExecuteNonQuery();
                        }

                        // Move the other question to our old sequence
                        string update2 = "UPDATE QuizQuestion SET SequenceOrder = @NewSeq WHERE QuestionID = @QID";
                        using (SqlCommand cmd = new SqlCommand(update2, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@NewSeq", currentSeq);
                            cmd.Parameters.AddWithValue("@QID", otherQID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log ex
                    return false;
                }
            }
        }

        /**
         * NEW: Updates a question's text, type, image, and options.
         */
        public bool UpdateQuestion(int questionId, string questionText, int questionTypeID, string imagePath, string correctAnswer, List<QuizAnswerOption> options)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Update the main question table
                    string updateSql = @"
                        UPDATE QuizQuestion 
                        SET QuestionText = @QuestionText, 
                            QuestionTypeID = @QuestionTypeID,
                            CorrectAnswer = @CorrectAnswer";

                    // Only update image if a new one was uploaded (imagePath is not null)
                    if (imagePath != null)
                    {
                        updateSql += ", ImagePath = @ImagePath";
                    }

                    updateSql += " WHERE QuestionID = @QuestionID";

                    using (SqlCommand cmd = new SqlCommand(updateSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuestionID", questionId);
                        cmd.Parameters.AddWithValue("@QuestionText", questionText);
                        cmd.Parameters.AddWithValue("@QuestionTypeID", questionTypeID);

                        if (string.IsNullOrEmpty(correctAnswer))
                            cmd.Parameters.AddWithValue("@CorrectAnswer", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@CorrectAnswer", correctAnswer);

                        if (imagePath != null)
                            cmd.Parameters.AddWithValue("@ImagePath", imagePath);

                        cmd.ExecuteNonQuery();
                    }

                    // 2. Get type name to decide how to handle options
                    string typeName = GetQuestionTypeNameById(questionTypeID);

                    // 3. Clear existing options
                    string delSql = "DELETE FROM QuizAnswerOption WHERE QuestionID = @QuestionID";
                    using (SqlCommand delCmd = new SqlCommand(delSql, conn, transaction))
                    {
                        delCmd.Parameters.AddWithValue("@QuestionID", questionId);
                        delCmd.ExecuteNonQuery();
                    }

                    // 4. If it's an MCQ, re-add all the new options
                    if (typeName == "MCQ" && options != null)
                    {
                        string optSql = "INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@QuestionID, @OptionText, @IsCorrect)";

                        foreach (var option in options)
                        {
                            using (SqlCommand optCmd = new SqlCommand(optSql, conn, transaction))
                            {
                                optCmd.Parameters.AddWithValue("@QuestionID", questionId);
                                optCmd.Parameters.AddWithValue("@OptionText", option.OptionText);
                                optCmd.Parameters.AddWithValue("@IsCorrect", option.IsCorrect);
                                optCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log ex
                    return false;
                }
            }
        }

        /**
         * NEW: Adds a single blank answer option to a question.
         */
        public bool AddBlankOption(int questionId)
        {
            string sql = "INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@QuestionID, 'New Option', 0)";
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuestionID", questionId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log ex
                return false;
            }
        }

        /**
         * NEW: Deletes a single answer option.
         */
        public bool DeleteOption(int optionId)
        {
            string sql = "DELETE FROM QuizAnswerOption WHERE OptionID = @OptionID";
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@OptionID", optionId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log ex
                return false;
            }
        }

        /**
         * NEW: Gets the count of all active quizzes and practices.
         */
        public int GetTotalQuizCount()
        {
            string sql = "SELECT COUNT(QuizID) FROM Quiz WHERE isActivated = 1";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        #endregion

    }
}