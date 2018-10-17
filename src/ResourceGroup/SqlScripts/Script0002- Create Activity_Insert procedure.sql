CREATE PROCEDURE Activity_Insert (@AthleteId int, @ActivityId int, @ActivityTime datetime2, @ActivityType nvarchar(50), @Distance float, @MovingTime int, @Category nvarchar(60), @Points float, @Source nvarchar(30))
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF NOT EXISTS ( SELECT * FROM dbo.[Strava.Activities] WITH (UPDLOCK) WHERE ActivityId = @ActivityId)

      INSERT dbo.[Activities] ( AthleteId, ActivityId, ActivityTime, ActivityType, Distance, MovingTime, Category, Points, Source)
      VALUES ( @AthleteId, @ActivityId, @ActivityTime, @ActivityType, @Distance, @MovingTime, @Category, @Points, @Source);
 
  COMMIT