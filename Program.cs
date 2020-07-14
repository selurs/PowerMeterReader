using System;
using System.Drawing;
using System.Threading.Tasks;
using CommandLine;
using PowerMeterReader.Models;
using ConsoleTables;
using System.Linq;
using System.Collections.Generic;
using InfluxDB.Client.Core;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using System.Threading;

namespace PowerMeterReader
{
    public class Program
    {
        public class Options
        {
            [Option('a',"address", Required = true, HelpText = "The IP Address of the power meter.")]
            public string MeterAddress { get; set; }
        }
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync<Options>(async o => 
                {
                    var reader = new MeterReader(o.MeterAddress);
                    for (int i = 0; i < 360000; i++)
                    {
                        // Once a second for ten seconds...
                        Thread.Sleep(1000);
                        var response = await reader.ReadPowerFlowData();

                        PublishData(response);
                        FormatOutput(response);
                    }
                });
        }

        public static void PublishData(PowerFlowRealtimeDataModel model)
        {            
            // You can generate a Token from the "Tokens Tab" in the UI
            const string token = "1Xli7Idwo8AAtoyEGHWQv_4MduTk3CmFqv7Cfdw8_Be8h-m6_ehLLjG8EhsRVmAHZKtsWtbUFpsHQ7lUXsGCug==";
            const string bucket = "bc57b7930b7fff33";
            const string org = "024ad8697c8a4f99";
                    
            var client = InfluxDBClientFactory.Create("https://us-west-2-1.aws.cloud2.influxdata.com", token.ToCharArray());
            
            var prod = new Production { Value =  (int)(model.Body.Data.Site.CurrentPVProduction ?? 0), Time = model.Head.TimeStamp.ToUniversalTime() };
            var cons = new Consumption { Value = (int)(model.Body.Data.Site.CurrentPowerConsumption), Time = model.Head.TimeStamp.ToUniversalTime() };

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteMeasurement(bucket, org, WritePrecision.Ns, prod);
                writeApi.WriteMeasurement(bucket, org, WritePrecision.Ns, cons);
            }
        }

        public static void FormatOutput(PowerFlowRealtimeDataModel model)
        {            
            if (model == null)
            {
                Console.WriteLine("Recieved error response from power meter. Please try again.");
                return;
            }

            Console.WriteLine($"Realtime power generation/consumption data. Collected at {model.Head.TimeStamp.ToString("r")}");
            var table = new ConsoleTable("Reading", "Value");
            table.AddRow("Current Generation", model.Body.Data.Site.CurrentPVProduction);
            table.AddRow("Current Consumption", model.Body.Data.Site.CurrentPowerConsumption);
            table.AddRow("Net Grid Power", model.Body.Data.Site.GridPowerUsage);
            table.Write();
        }
        
        public static void FormatOutput(RealtimeDataModel model)
        {
            if (model == null)
            {
                Console.WriteLine("Recieved error response from power meter. Please try again.");
                return;
            }            

            //Console.WriteLineFormatted("Power generation request for Scope {0}, Device Class {1} recived at {2} with response code {3}:\n", model.Head.RequestArguments.Scope, model.Head.RequestArguments.DeviceClass, model.Head.TimeStamp.ToShortTimeString(), model.Head.Status.Code, Color.Khaki, Color.NavajoWhite);
            Console.WriteLine("Power generation request for Scope {0}, Device Class {1} recived at {2} with response code {3}:\n", model.Head.RequestArguments.Scope, model.Head.RequestArguments.DeviceClass, model.Head.TimeStamp.ToShortTimeString(), model.Head.Status.Code);
            var table = new ConsoleTable("Metric", "Unit", "Value");
            table.AddRow("Current Generation", $"K{model.Body.Data.InstantaneousPowerGeneration.Unit}", GetDataValue(model.Body.Data.InstantaneousPowerGeneration.Values));
            table.AddRow("Today's Generation", $"K{model.Body.Data.DayEnergy.Unit}", GetDataValue(model.Body.Data.DayEnergy.Values));
            table.AddRow("Year's Generation", $"K{model.Body.Data.YearEnergy.Unit}", GetDataValue(model.Body.Data.YearEnergy.Values));
            table.AddRow("All Time Generation", $"K{model.Body.Data.TotalEnergy.Unit}", GetDataValue(model.Body.Data.TotalEnergy.Values));
            table.Write();
        }

        public static string GetDataValue(Dictionary<string, int> values)
        {
            var firstValue = values.FirstOrDefault();
            if (String.IsNullOrWhiteSpace(firstValue.Key))
            {
                return "NA";
            }
            
            var kiloUnit = (((double)firstValue.Value)/1000);
            var digits = (int) Math.Log10(kiloUnit);
            var maxDecimalplaces = 2;
            var format = "F" + Math.Max(0,(maxDecimalplaces - digits));
            return kiloUnit.ToString(format);
        }
        
        // Public class
        [Measurement("consumption")]
        private class Consumption
        {
            //[Column("host", IsTag = true)] public string Host { get; set; }
            [Column("value")] public int Value { get; set; }
            [Column(IsTimestamp = true)] public DateTime Time { get; set; }
        }

        [Measurement("production")]
        private class Production
        {
            [Column("value")] public int Value { get; set; }
            [Column(IsTimestamp = true)] public DateTime Time { get; set; }
        }
    }
}
