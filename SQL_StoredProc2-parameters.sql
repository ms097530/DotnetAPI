USE DotNetCourseDatabase
GO

ALTER PROCEDURE TutorialAppSchema.spUsers_Get
-- to run proc -> EXEC TutorialAppSchema.spUsers_Get @UserId = 1
-- NOTE: explicitly specifying parameter to avoid ambiguity and bugs in future, should proc be altered
    -- when using param as filter, check definition to make sure types match up to avoid implicit conversion
    @UserId INT
AS
BEGIN
    SELECT [Users].[UserId],
        [Users].[FirstName],
        [Users].[LastName],
        [Users].[Email],
        [Users].[Gender],
        [Users].[Active]
    FROM TutorialAppSchema.Users AS Users
    -- fully qualify UserId
        WHERE Users.UserId = @UserId
END