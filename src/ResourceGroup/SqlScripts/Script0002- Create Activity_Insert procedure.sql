CREATE PROCEDURE Activity_Insert (@Id nvarchar(32), @AthleteId nvarchar(32), @ExternalId nvarchar(100), @ActivityTime datetime2, @ActivityType nvarchar(50), @Distance float, @MovingTime int, @Category nvarchar(60), @Points float, @Source nvarchar(30))
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF NOT EXISTS ( SELECT * FROM dbo.[Activities] WITH (UPDLOCK) WHERE Id = @Id OR ExternalId = @ExternalId)

      INSERT dbo.[Activities] (Id, AthleteId, ExternalId, ActivityTime, ActivityType, Distance, MovingTime, Category, Points, Source)
      VALUES (@Id, @AthleteId, @ExternalId, @ActivityTime, @ActivityType, @Distance, @MovingTime, @Category, @Points, @Source);
 
  COMMIT