namespace AddressBookWebUI.Areas.Admin.Models
{
    public class StatisticsChartViewModel
    {
        public List<string> Days { get; set; } = new List<string>() { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar" };

        public StatisticsChartLabelViewModel Label1 { get; set; }
        public StatisticsChartLabelViewModel Label2 { get; set; }
        public StatisticsChartLabelViewModel Label3 { get; set; }
    }

   
}
