ALTER TABLE dbo.[Strava.Activities]
	ADD Category nvarchar(60),
		Points int DEFAULT 0 NOT NULL