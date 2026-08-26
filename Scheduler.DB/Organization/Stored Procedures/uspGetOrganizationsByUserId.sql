CREATE procedure  [Organization].[uspGetOrganizationsByUserId]-- [organization].[uspGetOrganizationsByUserId] '57EFE5E7-7475-4F1B-BA09-E26BC713F52A'
                                                             -- [organization].[uspGetOrganizationsByUserId] @pUserId='27DA5DD8-C0E8-4F9C-9BF1-FD0A2186E5'
@pUserId uniqueidentifier
AS 
begin

select distinct org.Id,org.Name,org.Description,org.IsActive
Into #FinalResults
   from [dbo].[tbUserFranchise] uf 
   Join tblFranchise f on uf.FranchiseId=f.Id
   Join tblOrganization org on org.Id=f.OrganizationId
where 1=1 and uf.UserId=@pUserId


select * from #FinalResults
select count(*) TotalRecords from #FinalResults

end