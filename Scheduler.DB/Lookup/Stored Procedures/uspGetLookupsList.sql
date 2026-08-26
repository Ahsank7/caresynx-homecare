CREATE   procedure  [Lookup].[uspGetLookupsList]   
AS   
begin  
  
SELECT [Name] as [key],DisplayName as [value]
FROM [dbo].[tblLookups]  
WHERE ISNULL(IsVisible,0)=1
  
end