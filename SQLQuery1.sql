PRINT 'Adding IsDeleted column to Quiz table...';

ALTER TABLE Quiz
ADD IsDeleted BIT NOT NULL DEFAULT 0;

PRINT 'Column added successfully.';