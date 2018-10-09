CREATE PROCEDURE Strava_Activity_Insert ( @AthleteId int, @ActivityId int, @ActivityTime datetime2, @ActivityType nvarchar(50), @Distance float, @MovingTime int)
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF NOT EXISTS ( SELECT * FROM dbo.[Strava.Activities] WITH (UPDLOCK) WHERE ActivityId = @ActivityId)

      INSERT dbo.[Strava.Activities] ( AthleteId, ActivityId, ActivityTime, ActivityType, Distance, MovingTime)
      VALUES ( @AthleteId, @ActivityId, @ActivityTime, @ActivityType, @Distance, @MovingTime);
 
  COMMIT