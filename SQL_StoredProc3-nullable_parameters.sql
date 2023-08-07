USE DotNetCourseDatabase
GO

ALTER PROCEDURE TutorialAppSchema.spUsers_Get
/* 
    to run proc -> 
    * add, remove, change parameters to filter return *
    EXEC TutorialAppSchema.spUsers_Get @FirstName = "Bob"
*/
-- NOTE: explicitly specifying parameter to avoid ambiguity and bugs in future, should proc be altered
    -- make parameter nullable
    @UserId INT = NULL,
    @FirstName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50) = NULL
AS
BEGIN
    SELECT [Users].[UserId],
        [Users].[FirstName],
        [Users].[LastName],
        [Users].[Email],
        [Users].[Gender],
        [Users].[Active]
    FROM TutorialAppSchema.Users AS Users
    -- handle null (default or passed null)
    -- if @UserId is null, compare Users.UserId against itself (matches everything and returns all users)
    -- can now get a single user by UserId, or get all users
        WHERE Users.UserId = ISNULL(@UserId, Users.UserId)
            AND Users.FirstName = ISNULL(@FirstName, Users.FirstName)
            AND Users.LastName = ISNULL(@LastName, Users.LastName)    
END