CREATE TABLE dbo.[Systems.UpdateHistory] ([System][nvarchar](20) NOT NULL UNIQUE, [LastUpdate][datetime2])

INSERT dbo.[Systems.UpdateHistory] (System, LastUpdate) VALUES ('Strava', NULL);
INSERT dbo.[Systems.UpdateHistory] (System, LastUpdate) VALUES ('Endomondo', NULL);
INSERT dbo.[Systems.UpdateHistory] (System, LastUpdate) VALUES ('Runkeeper', NULL);