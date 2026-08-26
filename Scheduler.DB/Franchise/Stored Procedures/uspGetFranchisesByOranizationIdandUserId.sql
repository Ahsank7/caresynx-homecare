CREATE procedure  [Franchise].[uspGetFranchisesByOranizationIdandUserId] --[Franchise].[uspGetFranchisesByOranizationIdandUserId] 'E1DE9D19-B127-4135-ABB9-3CBB12963414','57EFE5E7-7475-4F1B-BA09-E26BC713F52A'
@pOrganizationId uniqueidentifier,
@pUserId uniqueidentifier
AS 
begin

Declare @vRoleId int =0

Select top 1 @vRoleId=RoleId from tblUserRole UR Where  UR.UserId=@pUserId AND UR.IsActive=1

CREATE TABLE #FinalResults
(Id uniqueidentifier, Name nvarchar(500), Description nvarchar(500), IsActive bit, organizationId uniqueidentifier)

IF @vRoleId=6
Begin

print 'Super Admin'
INSERT INTO #FinalResults
select f.Id,f.Name,f.Description,f.IsActive,org.Id as organizationId
   from  tblFranchise f 
   Join tblOrganization org on org.Id=f.OrganizationId
where 1=1 and f.IsActive=1 and org.IsActive=1 and org.Id=@pOrganizationId

END
Else
Begin

print 'else'
INSERT INTO #FinalResults
select f.Id,f.Name,f.Description,f.IsActive,org.Id as organizationId
   from [dbo].[tbUserFranchise] uf 
   Left Join tblFranchise f on uf.FranchiseId=f.Id and f.IsActive=1
   Join tblOrganization org on org.Id=f.OrganizationId
where 1=1 and org.IsActive=1 and org.Id=@pOrganizationId and uf.UserId=@pUserId

END
select * from #FinalResults
select count(*) TotalRecords from #FinalResults

end