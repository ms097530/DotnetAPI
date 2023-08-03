USE DotNetCourseDatabase

CREATE TABLE TutorialAppSchema.Posts (
    PostId INT IDENTITY(1,1),
    UserId INT,
    -- NVARCHAR so Unicode characters are fine
    PostTitle NVARCHAR(255),
    PostContent NVARCHAR(MAX),
    -- METADATA
    -- DATETIME more precise and takes up more space than DATETIME2
    PostCreated DATETIME,
    PostUpdated DATETIME
)