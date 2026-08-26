using System.Text.Json.Serialization;

namespace Scheduler.API.Models.User
{
    public class UserSearchRequest
    {
        public Guid FranchiseId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserNo { get; set; }
        public int UserType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int? GenderId { get; set; }
        public int? StatusId { get; set; }
        public int? EthnicityId { get; set; }
        [JsonPropertyName("sortColumn")]
        public string? SortColumn { get; set; }

        /// <summary>Must be 'ASC' or 'DESC' (client sends <c>sortType</c> in JSON).</summary>
        [JsonPropertyName("sortType")]
        public string? SortType { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Guid? CurrentUserId { get; set; } // For role hierarchy filtering
    }
}
