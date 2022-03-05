namespace Loupedeck.DemoPlugin
{
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Amazon.CostExplorer;
    using Amazon.CostExplorer.Model;

    public class ToggleSwitchButton : PluginDynamicCommand
    {
        private Boolean _toggleState = false;
        private readonly Thread t;
        private readonly AmazonCostExplorerClient client = new AmazonCostExplorerClient("AKIAQWHP55LEUE5VAXZO", "u43tHaDBwBe4m2AXzpvKpER2XYnP0eGwc9t9rICS");
        private float metric;
        public ToggleSwitchButton() : base("Toggle Switch", null, "Tests")
        {
            Console.WriteLine(Environment.GetEnvironmentVariable("SECRET_KEY"));
            this.t = new Thread(new ThreadStart(this.ThreadProc));
        }

        protected override Boolean OnLoad()
        {
            base.OnLoad();
            this.t.Start();
            return true;
        }

        protected override void RunCommand(String actionParameter)
        {
        }

        protected override Boolean OnUnload() {
            base.OnUnload();
            this.t.Join();
            return true;
        }

        public void ThreadProc()
        {
            while (true)
            {
                this._toggleState = !this._toggleState;
                this.ActionImageChanged();
                var request = new GetCostAndUsageRequest();
                var expression = new Expression();
                var not = new Expression();
                var dimension_values = new DimensionValues();
                dimension_values.Key = "RECORD_TYPE";
                var match_options = new List<String> { "EQUALS" };
                dimension_values.MatchOptions = match_options;
                var values = new List<String> { "Credit" };
                dimension_values.Values = values;
                not.Dimensions = dimension_values;
                expression.Not =not;
                request.Filter = expression;
                request.Granularity = "MONTHLY";
                request.Metrics.Add("BlendedCost");
                var date_interval = new DateInterval();
                date_interval.Start = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                date_interval.End = DateTime.Now.ToString("yyyy-MM-dd");
                request.TimePeriod = date_interval;
                try
                {
                    GetCostAndUsageResponse response = this.client.GetCostAndUsage(request);
                    //client is a AmazonCostExplorerClient object
                    this.metric = float.Parse(response.ResultsByTime[0].Total["BlendedCost"].Amount, CultureInfo.InvariantCulture.NumberFormat);
                    Console.WriteLine(JsonConvert.SerializeObject(response));
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadKey();
                }
                Thread.Sleep(360000);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.DrawText(this.metric.ToString());
                return bitmapBuilder.ToImage();  
            }
        }
    }
}
