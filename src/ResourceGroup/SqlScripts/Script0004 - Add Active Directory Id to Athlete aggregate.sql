ALTER TABLE dbo.[Athletes]
  ADD ActiveDirectoryId nvarchar(100)
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_Aad_Id
ON dbo.[Athletes](ActiveDirectoryId)
WHERE ActiveDirectoryId IS NOT NULL;