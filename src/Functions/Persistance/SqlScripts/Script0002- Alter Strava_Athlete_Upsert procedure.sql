CREATE PROCEDURE Strava_Athlete_Upsert ( @AthleteId int, @FirstName nvarchar(50), @LastName nvarchar(50), @AccessToken nvarchar(100), @Active bit)
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF EXISTS ( SELECT * FROM dbo.[Strava.Athletes] WITH (UPDLOCK) WHERE AthleteId = @AthleteId)
 
      UPDATE dbo.[Strava.Athletes]
         SET FirstName = @FirstName, LastName = @LastName, AccessToken = @AccessToken, Active = @Active
       WHERE AthleteId = @AthleteId;
 
    ELSE 
 
      INSERT dbo.[Strava.Athletes] ( AthleteId, FirstName, LastName, AccessToken, Active)
      VALUES ( @AthleteId, @FirstName, @LastName, @AccessToken, @Active);
 
  COMMIT