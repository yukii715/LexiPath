-- =============================================
-- 1. SETUP & VARIABLES
-- =============================================
DECLARE @LanguageID INT = 2; -- English
DECLARE @CatTravelID INT, @CatDailyID INT;
DECLARE @CourseVocabID INT, @CoursePhraseID INT, @CourseMixedID INT;
DECLARE @QuizGeneralID INT, @QuizPracticeID INT;
DECLARE @Q1 INT, @Q2 INT, @Q3 INT, @Q4 INT, @Q5 INT;

-- Ensure Question Types exist (Assuming 1=MCQ, 2=Type In, 3=Fill Blank)
-- Adjust IDs based on your actual QuestionType table if different
IF NOT EXISTS (SELECT * FROM QuestionType WHERE QuestionTypeID = 1)
    INSERT INTO QuestionType (QuestionTypeID, TypeName) VALUES (1, 'MCQ');
IF NOT EXISTS (SELECT * FROM QuestionType WHERE QuestionTypeID = 2)
    INSERT INTO QuestionType (QuestionTypeID, TypeName) VALUES (2, 'TypeInAnswer');

-- =============================================
-- 2. CREATE CATEGORIES
-- =============================================
INSERT INTO Category (CategoryName, LanguageID, ImagePath, isActivated) 
VALUES ('Travel Essentials', @LanguageID, '/Image/Category/travel_eng.jpg', 1);
SET @CatTravelID = SCOPE_IDENTITY();

INSERT INTO Category (CategoryName, LanguageID, ImagePath, isActivated) 
VALUES ('Daily Conversation', @LanguageID, '/Image/Category/daily_eng.jpg', 1);
SET @CatDailyID = SCOPE_IDENTITY();

-- =============================================
-- 3. CREATE COURSES (One of each type)
-- =============================================

-- Course A: Vocabulary Type (Travel)
INSERT INTO Courses (CategoryID, CourseName, Description, ImagePath, CourseType, LanguageID, isActivated)
VALUES (@CatTravelID, 'Airport Vocabulary', 'Essential words for navigating an airport.', '/Image/Course/airport.jpg', 'Vocabulary', @LanguageID, 1);
SET @CourseVocabID = SCOPE_IDENTITY();

-- Course B: Phrase Type (Daily)
INSERT INTO Courses (CategoryID, CourseName, Description, ImagePath, CourseType, LanguageID, isActivated)
VALUES (@CatDailyID, 'Basic Greetings', 'Common ways to say hello and goodbye.', '/Image/Course/greetings.jpg', 'Phrase', @LanguageID, 1);
SET @CoursePhraseID = SCOPE_IDENTITY();

-- Course C: Mixed Type (Travel)
INSERT INTO Courses (CategoryID, CourseName, Description, ImagePath, CourseType, LanguageID, isActivated)
VALUES (@CatTravelID, 'Restaurant Survival', 'Words and phrases for ordering food.', '/Image/Course/restaurant.jpg', 'Mixed', @LanguageID, 1);
SET @CourseMixedID = SCOPE_IDENTITY();

-- =============================================
-- 4. POPULATE LEARNING ITEMS (At least 5 per course)
-- =============================================

-- --- Content for Course A (Vocabulary) ---
INSERT INTO Vocabulary (CourseID, VocabText, Meaning, ImagePath, SequenceOrder, isActivated) VALUES
(@CourseVocabID, 'Passport', 'An official document issued by a government.', '/Image/Vocab/passport.png', 1, 1),
(@CourseVocabID, 'Luggage', 'Suitcases or bags used for traveling.', '/Image/Vocab/luggage.png', 2, 1),
(@CourseVocabID, 'Ticket', 'A piece of paper that gives the holder a right to enter.', '/Image/Vocab/ticket.png', 3, 1),
(@CourseVocabID, 'Gate', 'An exit from an airport building to an aircraft.', '/Image/Vocab/gate.png', 4, 1),
(@CourseVocabID, 'Customs', 'The official department that administers and collects duties.', '/Image/Vocab/customs.png', 5, 1);

-- --- Content for Course B (Phrase) ---
INSERT INTO Phrase (CourseID, PhraseText, Meaning, SequenceOrder, isActivated) VALUES
(@CoursePhraseID, 'Good morning', 'Used to say hello in the morning.', 1, 1),
(@CoursePhraseID, 'How are you?', 'Used to ask about someone''s health or feelings.', 2, 1),
(@CoursePhraseID, 'Nice to meet you', 'Used when meeting someone for the first time.', 3, 1),
(@CoursePhraseID, 'See you later', 'Used for saying goodbye.', 4, 1),
(@CoursePhraseID, 'Have a nice day', 'A polite way to say goodbye.', 5, 1);

-- Add Details for the first Phrase (Example)
DECLARE @FirstPhraseID INT = (SELECT TOP 1 PhraseID FROM Phrase WHERE CourseID = @CoursePhraseID AND SequenceOrder = 1);
INSERT INTO PhraseDetail (PhraseID, DetailType, Content) VALUES 
(@FirstPhraseID, 'Pronunciation', 'Gud mor-ning'),
(@FirstPhraseID, 'Usage Note', 'Only use before 12:00 PM.');

-- --- Content for Course C (Mixed) ---
-- 3 Vocabulary Items
INSERT INTO Vocabulary (CourseID, VocabText, Meaning, ImagePath, SequenceOrder, isActivated) VALUES
(@CourseMixedID, 'Menu', 'A list of dishes available in a restaurant.', '/Image/Vocab/menu.png', 1, 1),
(@CourseMixedID, 'Fork', 'An implement with two or more prongs used for lifting food.', '/Image/Vocab/fork.png', 2, 1),
(@CourseMixedID, 'Napkin', 'A square piece of cloth or paper used at a meal.', '/Image/Vocab/napkin.png', 3, 1);

-- 2 Phrase Items
INSERT INTO Phrase (CourseID, PhraseText, Meaning, SequenceOrder, isActivated) VALUES
(@CourseMixedID, 'Table for two, please', 'Asking for a table for two people.', 4, 1),
(@CourseMixedID, 'Can I have the bill?', 'Asking to pay for the meal.', 5, 1);

-- =============================================
-- 5. CREATE QUIZZES (At least 5 questions each)
-- =============================================

-- --- Quiz 1: General Quiz (Linked to Course A and B) ---
INSERT INTO Quiz (Title, Description, ImagePath, IsPractice, LanguageID, isActivated)
VALUES ('English Basics Quiz', 'Test your airport and greeting knowledge.', '/Image/Quiz/general.jpg', 0, @LanguageID, 1);
SET @QuizGeneralID = SCOPE_IDENTITY();

-- Link Quiz to Courses
INSERT INTO QuizRelatedCourse (QuizID, CourseID) VALUES (@QuizGeneralID, @CourseVocabID), (@QuizGeneralID, @CoursePhraseID);

-- Question 1 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizGeneralID, 1, 'What is the document needed to travel internationally?', 1);
SET @Q1 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q1, 'Passport', 1), (@Q1, 'Ticket', 0), (@Q1, 'Map', 0), (@Q1, 'Receipt', 0);

-- Question 2 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizGeneralID, 1, 'Which phrase is used in the morning?', 2);
SET @Q2 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q2, 'Good night', 0), (@Q2, 'Good morning', 1), (@Q2, 'Good evening', 0);

-- Question 3 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizGeneralID, 1, 'Where do you go to board the plane?', 3);
SET @Q3 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q3, 'The Gate', 1), (@Q3, 'The Hotel', 0), (@Q3, 'The Parking Lot', 0);

-- Question 4 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizGeneralID, 1, 'What holds your clothes while traveling?', 4);
SET @Q4 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q4, 'Wallet', 0), (@Q4, 'Luggage', 1), (@Q4, 'Menu', 0);

-- Question 5 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizGeneralID, 1, 'How do you ask about someone''s health?', 5);
SET @Q5 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q5, 'Who are you?', 0), (@Q5, 'How are you?', 1), (@Q5, 'Where are you?', 0);


-- --- Quiz 2: Practice Mode (Linked to Course C - Restaurant) ---
INSERT INTO Quiz (Title, Description, ImagePath, IsPractice, LanguageID, isActivated)
VALUES ('Restaurant Practice', 'Practice your ordering skills.', '/Image/Quiz/practice.jpg', 1, @LanguageID, 1);
SET @QuizPracticeID = SCOPE_IDENTITY();

-- Link to Course C
INSERT INTO QuizRelatedCourse (QuizID, CourseID) VALUES (@QuizPracticeID, @CourseMixedID);

-- Question 1 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizPracticeID, 1, 'What do you read to choose your food?', 1);
SET @Q1 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q1, 'Menu', 1), (@Q1, 'Napkin', 0), (@Q1, 'Fork', 0);

-- Question 2 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizPracticeID, 1, 'Tool used to eat food.', 2);
SET @Q2 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q2, 'Fork', 1), (@Q2, 'Table', 0), (@Q2, 'Passport', 0);

-- Question 3 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizPracticeID, 1, 'Used to wipe your mouth.', 3);
SET @Q3 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q3, 'Plate', 0), (@Q3, 'Napkin', 1), (@Q3, 'Ticket', 0);

-- Question 4 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizPracticeID, 1, 'How to ask for payment?', 4);
SET @Q4 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q4, 'Can I have the bill?', 1), (@Q4, 'Hello', 0), (@Q4, 'Table for two', 0);

-- Question 5 (MCQ)
INSERT INTO QuizQuestion (QuizID, QuestionTypeID, QuestionText, SequenceOrder) VALUES (@QuizPracticeID, 1, 'How to request a seat for a couple?', 5);
SET @Q5 = SCOPE_IDENTITY();
INSERT INTO QuizAnswerOption (QuestionID, OptionText, IsCorrect) VALUES (@Q5, 'Table for two, please', 1), (@Q5, 'Where is the gate?', 0), (@Q5, 'Good morning', 0);

SELECT 'Data Insertion Complete' AS Status;