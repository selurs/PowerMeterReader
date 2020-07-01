using System;
using System.Drawing;
using System.Threading.Tasks;
using CommandLine;
using PowerMeterReader.Models;
using ConsoleTables;
using Console = Colorful.Console;
using System.Linq;
using System.Collections.Generic;

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
                    var response = await reader.Read("System");
                    FormatOutput(response);
                });
        }

        public static void FormatOutput(RealtimeDataModel model)
        {
            if (model == null)
            {
                Console.WriteLine("Recieved error response from power meter. Please try again.", Color.IndianRed);
            }            

            Console.WriteLineFormatted("Power generation request for Scope {0}, Device Class {1} recived at {2} with response code {3}:\n", model.Head.RequestArguments.Scope, model.Head.RequestArguments.DeviceClass, model.Head.TimeStamp.ToShortTimeString(), model.Head.Status.Code, Color.Khaki, Color.NavajoWhite);
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
    }
}
