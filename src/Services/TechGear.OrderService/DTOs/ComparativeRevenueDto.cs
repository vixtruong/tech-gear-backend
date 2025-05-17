namespace TechGear.OrderService.DTOs
{
    public class ComparativeRevenueDto
    {
        public List<PeriodRevenue> CurrentPeriod { get; set; } = new List<PeriodRevenue>();
        public List<PeriodRevenue> PreviousPeriod { get; set; } = new List<PeriodRevenue>();
    }

    public class PeriodRevenue
    {
        public string PeriodName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}