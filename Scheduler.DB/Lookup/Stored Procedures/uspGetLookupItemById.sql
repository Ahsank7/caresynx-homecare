CREATE   procedure  [Lookup].[uspGetLookupItemById] 
@pId int
AS 
begin

SELECT [Id]
      ,[LookupType]
      ,[Name]
      ,[Description]
      ,[IsActive]
      ,[InsertedById]
      ,[InsertedAt]
      ,[UpdatedById]
      ,[UpdatedAt]
  FROM [dbo].[tblLookupItems]
  WHERE Id=@pId 

end