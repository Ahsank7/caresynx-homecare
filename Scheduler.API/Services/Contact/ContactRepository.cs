using Dapper;
using Scheduler.API.Models.Contact;
using System.Data;

namespace Scheduler.API.Services.Contact
{
    public class ContactRepository : IContact
    {
        IDapperRepository _dapperRepository = null;
        public ContactRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }
        public async Task<Guid?> CreateUpdateContactAsync(SaveContactInfoViewModel saveContactInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pFirstName", saveContactInfoViewModel.FirstName, DbType.String);
                dp_params.Add("@pSurName",saveContactInfoViewModel.SurName, DbType.String);
                dp_params.Add("@pLastName", saveContactInfoViewModel.LastName, DbType.String);
                dp_params.Add("@pAlias", saveContactInfoViewModel.Alias, DbType.String);
                dp_params.Add("@pPhoneNo", saveContactInfoViewModel.PhoneNo, DbType.String);
                dp_params.Add("@pMobileNo", saveContactInfoViewModel.MobileNo, DbType.String);
                dp_params.Add("@pIdentityNo", saveContactInfoViewModel.IdentityNo, DbType.String);
                dp_params.Add("@pBirthDate", saveContactInfoViewModel.BirthDate, DbType.Date);
                dp_params.Add("@pCountyId", saveContactInfoViewModel.CountyId, DbType.Int32);
                dp_params.Add("@pEmail", saveContactInfoViewModel.Email, DbType.String);
                dp_params.Add("@pAddressLine1", saveContactInfoViewModel.AddressLine1, DbType.String);
                dp_params.Add("@pAddressLine2", saveContactInfoViewModel.AddressLine2, DbType.String);
                dp_params.Add("@pAddressLine3", saveContactInfoViewModel.AddressLine3, DbType.String);
                dp_params.Add("@pLatitude", saveContactInfoViewModel.Latitude, DbType.Decimal);
                dp_params.Add("@pLongitude", saveContactInfoViewModel.Longitude, DbType.Decimal);
                dp_params.Add("@pStateId", saveContactInfoViewModel.StateId, DbType.Int32);
                dp_params.Add("@pCountryId", saveContactInfoViewModel.CountryId, DbType.Int32);
                dp_params.Add("@pGenderId", saveContactInfoViewModel.GenderId, DbType.Int32);
                dp_params.Add("@pTitleId", saveContactInfoViewModel.TitleId, DbType.Int32);
                dp_params.Add("@pContactTypeId", saveContactInfoViewModel.ContactTypeId, DbType.Int32);
                dp_params.Add("@pFranchiseId", saveContactInfoViewModel.FranchiseId, DbType.Guid);
                dp_params.Add("@pNotes", saveContactInfoViewModel.Notes, DbType.String);
                dp_params.Add("@pContactUserId", saveContactInfoViewModel.UserId, DbType.Guid);
                dp_params.Add("@pId", saveContactInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pIsBillingContact", saveContactInfoViewModel.IsBillingContact, DbType.Boolean);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<ContactInfo>("[Contact].[InsertUpdateContact]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));


                return dp_params.Get<Guid?>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteContact(Guid Id)
        {
            bool  result = true;
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", Id, DbType.Guid);
                 _dapperRepository.Update<Guid>("[Contact].[DeleteUserContact]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }

        public async Task<ContactInfo> GetContactInfoAsync(Guid ContactID)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", ContactID, DbType.Guid);
                //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.GetList<ContactInfo>("[Contact].[GetUserContactInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ContactSearchResponse> GetContactsAsync(ContactSearchRequest request)
        {
            try
            {
                ContactSearchResponse staffResponse = new ContactSearchResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortType", request.SortType, DbType.String);
                dp_params.Add("@pEmail", request.Email, DbType.String);
                dp_params.Add("@pMobileNumber", request.MobileNumber, DbType.String);
                dp_params.Add("@pPhoneNumber", request.PhoneNumber, DbType.String);
                dp_params.Add("@pFirstName", request.FirstName, DbType.String);
                dp_params.Add("@pEthnicityId", request.EthnicityId, DbType.Int32);
                dp_params.Add("@pContactTypeId", request.ContactTypeId, DbType.Int32);
                dp_params.Add("@pLastName", request.LastName, DbType.String);
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@pGenderId", request.GenderId, DbType.Int32);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
                var result = await Task.FromResult(_dapperRepository.GetAll<SearchContactViewModel>("[Contact].[uspGetAllContacts]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                staffResponse.Response = result.Item1;
                staffResponse.TotalRecords = result.Item2;

                return staffResponse;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
