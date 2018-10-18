CREATE TABLE dbo.[Athletes] (Id int NOT NULL IDENTITY(1,1), ExternalId int UNIQUE, FirstName nvarchar(50), LastName nvarchar(50) NOT NULL, ProfilePictureUrl nvarchar(max), Active bit NOT NULL, System nvarchar(30), PRIMARY KEY (Id))
GO

CREATE TABLE dbo.[Athletes.UpdateHistory] (AthleteId int NOT NULL, LastUpdate datetime2, FOREIGN KEY (AthleteId) REFERENCES dbo.[Athletes](Id))
GO

CREATE TABLE dbo.[Strava.AccessTokens] (AthleteId int NOT NULL, AccessToken nvarchar(100) NOT NULL, RefreshToken nvarchar(100) NOT NULL, ExpiresAt datetime2 NOT NULL, FOREIGN KEY (AthleteId) REFERENCES dbo.[Athletes](Id))
GO

CREATE TABLE dbo.[Activities] (AthleteId int NOT NULL, ActivityId int NOT NULL UNIQUE, ActivityTime datetime2, ActivityType nvarchar(50), Distance int, MovingTime int, Category nvarchar(60), Points float DEFAULT 0 NOT NULL, Source nvarchar(30), FOREIGN KEY (AthleteId) REFERENCES dbo.Athletes(Id))
GO

CREATE TABLE dbo.[MonthlyResultsSnapshots] (Id int PRIMARY KEY IDENTITY, Date nvarchar(7) NOT NULL, Results nvarchar(4000) NOT NULL)
GO

ALTER TABLE dbo.[MonthlyResultsSnapshots]
    ADD CONSTRAINT [Results record should be formatted as JSON]
                   CHECK (ISJSON(Results)=1)
GO

CREATE INDEX ix_strava_activities_activityTime ON dbo.[Activities](ActivityTime)
GO
CREATE INDEX ix_strava_athletemonthlyresults_date ON dbo.[MonthlyResultsSnapshots](Date)
GO