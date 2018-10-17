CREATE PROCEDURE MonthlyResultsSnapshots_Upsert ( @Date nvarchar(7), @Results nvarchar(4000))
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF EXISTS ( SELECT * FROM dbo.[MonthlyResultsSnapshots] WITH (UPDLOCK) WHERE Date = @Date)
 
      UPDATE dbo.[Strava.MonthlyResultsSnapshots]
         SET Results = @Results
       WHERE Date = @Date;
 
    ELSE 
 
      INSERT dbo.[MonthlyResultsSnapshots] ( Date, Results)
      VALUES (@Date, @Results);
 
  COMMIT