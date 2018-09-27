ALTER TABLE dbo.[Strava.Activities]
	ADD Category nvarchar(60),
		Points float DEFAULT 0 NOT NULL