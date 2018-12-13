CREATE PROCEDURE MonthlyResultsSnapshots_Upsert ( @Date nvarchar(7), @Results nvarchar(max))
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF EXISTS ( SELECT * FROM dbo.[MonthlyResultsSnapshots] WITH (UPDLOCK) WHERE Date = @Date)
 
      UPDATE dbo.[MonthlyResultsSnapshots]
         SET Results = @Results, LastUpdate = GETUTCDATE()
       WHERE Date = @Date;
 
    ELSE 
 
      INSERT dbo.[MonthlyResultsSnapshots] ( Date, Results, LastUpdate)
      VALUES (@Date, @Results, GETUTCDATE());
 
  COMMIT