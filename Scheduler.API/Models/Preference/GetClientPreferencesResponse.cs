namespace Scheduler.API.Models.Preference
{
    public class GetClientPreferencesResponse
    {
        public List<ClientPreferenceInfo> Preferences { get; set; } = new List<ClientPreferenceInfo>();
    }
}

