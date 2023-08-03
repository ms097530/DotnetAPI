USE DotNetCourseDatabase
GO

SELECT [UserId],
[FirstName],
[LastName],
[Email],
[Gender],
[Active] FROM TutorialAppSchema.Users

SELECT [UserId],
[JobTitle],
[Department] FROM TutorialAppSchema.UserJobInfo

SELECT [UserId],
[Salary] FROM TutorialAppSchema.UserSalary

SELECT * FROM TutorialAppSchema.Users
WHERE UserId = 1230

SELECT * FROM TutorialAppSchema.Users
    WHERE UserId = 1

SELECT * FROM TutorialAppSchema.Users
    WHERE FirstName = 'Boob'

SELECT * FROM TutorialAppSchema.Users
    ORDER BY UserId DESC

SELECT * FROM TutorialAppSchema.UserJobInfo
    -- WHERE JobTitle = 'Librarian'
    ORDER BY UserId DESC

SELECT * FROM TutorialAppSchema.UserSalary
    ORDER BY UserId DESC

DELETE FROM TutorialAppSchema.Users
    WHERE UserId = 1004

SELECT COUNT(*) FROM TutorialAppSchema.Users

SELECT * FROM TutorialAppSchema.Users WHERE UserId BETWEEN 990 AND 1005

INSERT INTO TutorialAppSchema.Users([FirstName],
[LastName],
[Email],
[Gender],
[Active])
    VALUES (
        'Bob',
        'Dob',
        'bob@dob.com',
        'Male',
        0
    )

-- UPDATE TutorialAppSchema.Users
--     SET
--     [FirstName] = '',
--     [LastName] = '',
--     [Email] = '',
--     [Gender] = '',
--     [Active] = 0
--     WHERE UserId = 

CREATE TABLE TutorialAppSchema.Auth  (
    Email NVARCHAR(50),
    PasswordHash VARBINARY(MAX),
    PasswordSalt VARBINARY(MAX)
);

SELECT [Email],
[PasswordHash],
[PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '';

INSERT INTO TutorialAppSchema.Auth (
    [Email],
    [PasswordHash],
    [PasswordSalt]
) VALUES (
    'test@test.com',
    0101010101,
    101010010101000010101
)

DELETE FROM TutorialAppSchema.Auth;

SELECT * FROM TutorialAppSchema.Users WHERE FirstName = 'Test';