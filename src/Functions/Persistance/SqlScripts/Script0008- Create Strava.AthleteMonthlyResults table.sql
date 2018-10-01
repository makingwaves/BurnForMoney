CREATE TABLE dbo.[Strava.AthleteMonthlyResults] (Id int primary key identity, Date nvarchar(7) NOT NULL, Results nvarchar(4000) NOT NULL)
GO
ALTER TABLE dbo.[Strava.AthleteMonthlyResults]
    ADD CONSTRAINT [Results record should be formatted as JSON]
                   CHECK (ISJSON(Results)=1)
GO
CREATE INDEX ix_strava_athletemonthlyresults_date ON dbo.[Strava.AthleteMonthlyResults](Date)