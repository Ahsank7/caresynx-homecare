CREATE procedure  [Franchise].[uspGetFranchisesByOranizationId] --[Franchise].[uspGetFranchisesByOranizationId] 'E1DE9D19-B127-4135-ABB9-3CBB12963414'
@pOrganizationId uniqueidentifier
AS 
begin

select f.Id,f.Name,f.Description,f.IsActive,org.Id as organizationId
into #FinalResults
   from  tblFranchise f 
   Join tblOrganization org on org.Id=f.OrganizationId
where 1=1 and org.Id=@pOrganizationId

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

end