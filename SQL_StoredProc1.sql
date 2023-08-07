USE DotNetCourseDatabase
GO

-- be careful with underscore after sp_ (may get picked up as system-wide index?)
-- CREATE PROCEDURE TutorialAppSchema.spUsers_Get
-- red line may show up if procedure not cached
ALTER PROCEDURE TutorialAppSchema.spUsers_Get
/* to call stored proc -> EXEC TutorialAppSchema.spUsers_Get */
-- set boundaries of stored proc
-- start query
AS
BEGIN
    SELECT [Users].[UserId],
        [Users].[FirstName],
        [Users].[LastName],
        [Users].[Email],
        [Users].[Gender],
        [Users].[Active] 
    FROM TutorialAppSchema.Users AS Users
-- end query
END