USE DotNetCourseDatabase

/* CREATE TABLE TutorialAppSchema.Posts (
    PostId INT IDENTITY(1,1),
    UserId INT,
    -- NVARCHAR so Unicode characters are fine
    PostTitle NVARCHAR(255),
    PostContent NVARCHAR(MAX),
    -- METADATA
    -- DATETIME more precise and takes up more space than DATETIME2
    PostCreated DATETIME,
    PostUpdated DATETIME
) */

-- Create clustered index informs order of physical storage in DB -> when storing new data, will be stored in order clustered index stipulates
-- sort by user id first, then post id
-- CREATE CLUSTERED INDEX cix_Posts_UserId_PostId ON TutorialAppSchema.Posts(UserId, PostId)

SELECT [PostId],
[UserId],
[PostTitle],
[PostContent],
[PostCreated],
[PostUpdated] FROM TutorialAppSchema.Posts

SELECT * FROM TutorialAppSchema.Users WHERE UserId = 1010

SELECT * FROM TutorialAppSchema.Posts

UPDATE TutorialAppSchema.Posts SET 
    PostTitle = 'ahh',
    PostContent = 'boo'
 WHERE PostId = 1

SELECT * FROM TutorialAppSchema.Auth

-- INSERT INTO TutorialAppSchema.Posts([PostId],
-- [UserId],
-- [PostTitle],
-- [PostContent],
-- [PostCreated],
-- [PostUpdated]) VALUES (

-- )

-- UPDATE TutorialAppSchema.Posts SET (
--     PostTitle = {postToEdit.PostTitle},
--     PostContent = {postToEdit.PostContent}
-- )
-- WHERE PostId = {postToEdit.PostId}