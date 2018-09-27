DROP PROCEDURE Strava_Activity_Insert
GO
CREATE PROCEDURE Strava_Activity_Insert (@AthleteId int, @ActivityId int, @ActivityTime datetime2, @ActivityType nvarchar(50), @Distance float, @MovingTime int, @Category nvarchar(60), @Points int)
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF NOT EXISTS ( SELECT * FROM dbo.[Strava.Activities] WITH (UPDLOCK) WHERE ActivityId = @ActivityId)

      INSERT dbo.[Strava.Activities] ( AthleteId, ActivityId, ActivityTime, ActivityType, Distance, MovingTime, Category, Points)
      VALUES ( @AthleteId, @ActivityId, @ActivityTime, @ActivityType, @Distance, @MovingTime, @Category, @Points);
 
  COMMIT