namespace Scheduler.API.Models.Service
{
    public class ServiceInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int ServiceTypeId { get; set; }
        public decimal Rate { get; set; }
    }
}
