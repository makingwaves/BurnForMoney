CREATE TABLE dbo.[Strava.AthleteMonthlyResults] (Id int primary key identity, Date datetime2 NOT NULL, AthleteId int UNIQUE NOT NULL, Results nvarchar(4000) NOT NULL, FOREIGN KEY (AthleteId) REFERENCES dbo.[Strava.Athletes](AthleteId) )
GO
ALTER TABLE dbo.[Strava.AthleteMonthlyResults]
    ADD CONSTRAINT [Results record should be formatted as JSON]
                   CHECK (ISJSON(Results)=1)