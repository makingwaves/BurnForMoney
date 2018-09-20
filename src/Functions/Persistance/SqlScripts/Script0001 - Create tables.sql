CREATE TABLE dbo.[Strava.Athletes] ([AthleteId][int] NOT NULL, [FirstName][nvarchar](50), [LastName][nvarchar](50), [AccessToken][nvarchar](100) NOT NULL, [Active][bit] NOT NULL, PRIMARY KEY (AthleteId))

CREATE TABLE dbo.[Strava.Activities] ([AthleteId][int] NOT NULL, [ActivityId][int] NOT NULL, [ActivityTime][datetime2], [ActivityType][nvarchar](50), [Distance][int], [MovingTime][int], FOREIGN KEY (AthleteId) REFERENCES dbo.[Strava.Athletes](AthleteId))
