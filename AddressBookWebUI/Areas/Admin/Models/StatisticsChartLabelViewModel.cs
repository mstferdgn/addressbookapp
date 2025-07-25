namespace AddressBookWebUI.Areas.Admin.Models
{
    public class StatisticsChartLabelViewModel
    {
        public  string LabelName { get; set; }
        public  string BorderColor { get; set; }
        public  string PointBackgroundColor { get; set; }
        public  int PointRadius { get; set; }
        public  string BackgroundColor { get; set; }
        public  string LegendColor { get; set; }
        public  bool Fill { get; set; }
        public  int BorderWidth { get; set; }
        public List<int> Data { get; set; } = new List<int>();
       
    }
}
