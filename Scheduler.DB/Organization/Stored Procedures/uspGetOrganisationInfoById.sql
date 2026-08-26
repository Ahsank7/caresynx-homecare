CREATE procedure  [Organization].[uspGetOrganisationInfoById] --[organization].[uspGetOrganisationInfoById] '66047CDF-FCC4-4C7F-982E-A310AB455F6E'
@pOrganizationId uniqueidentifier
AS 
begin

select *
   --from [dbo].[tbUserFranchise] uf 
   --Join tblFranchise f on uf.FranchiseId=f.Id
   from tblOrganization org 
where 1=1 and org.Id=@pOrganizationId and IsActive=1

end