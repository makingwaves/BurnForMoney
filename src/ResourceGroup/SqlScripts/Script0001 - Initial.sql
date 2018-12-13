CREATE TABLE dbo.[Athletes] (Id nvarchar(36) NOT NULL, ExternalId nvarchar(100) UNIQUE, FirstName nvarchar(50), LastName nvarchar(50), ProfilePictureUrl nvarchar(200), Active bit, System nvarchar(30), PRIMARY KEY (Id))
GO

CREATE TABLE dbo.[Activities] (Id nvarchar(36) NOT NULL, AthleteId nvarchar(36) NOT NULL, ExternalId nvarchar(100), ActivityTime datetime2, ActivityType nvarchar(50), Distance int, MovingTime int, Category nvarchar(60), Source nvarchar(30), PRIMARY KEY (Id), FOREIGN KEY (AthleteId) REFERENCES dbo.Athletes(Id))
GO

CREATE UNIQUE INDEX ix_acitivites_externalId_unique
  ON dbo.[Activities](ExternalId) 
  WHERE ExternalId IS NOT NULL
GO
CREATE INDEX ix_strava_activities_activityTime ON dbo.[Activities](ActivityTime)
GO



CREATE TABLE dbo.[MonthlyResultsSnapshots] (Id int PRIMARY KEY IDENTITY, Date nvarchar(7) NOT NULL, Results nvarchar(max) NOT NULL, LastUpdate datetime2)
GO

ALTER TABLE dbo.[MonthlyResultsSnapshots]
    ADD CONSTRAINT [Results record should be formatted as JSON]
                   CHECK (ISJSON(Results)=1)
GO
CREATE INDEX ix_strava_athletemonthlyresults_date ON dbo.[MonthlyResultsSnapshots](Date)
GO