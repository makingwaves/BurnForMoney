CREATE PROCEDURE Strava_AthleteMonthlyResults_Upsert ( @Date nvarchar(7), @Results nvarchar(4000))
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF EXISTS ( SELECT * FROM dbo.[Strava.AthleteMonthlyResults] WITH (UPDLOCK) WHERE Date = @Date)
 
      UPDATE dbo.[Strava.AthleteMonthlyResults]
         SET Results = @Results
       WHERE Date = @Date;
 
    ELSE 
 
      INSERT dbo.[Strava.AthleteMonthlyResults] ( Date, Results)
      VALUES (@Date, @Results);
 
  COMMIT