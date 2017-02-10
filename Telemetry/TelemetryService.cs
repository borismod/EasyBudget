using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Telemetry
{
    public static class TelemetryService
    {
        public static void Initialize()
        {
            var elasticSearchUrl = new Uri("http://localhost:9200");

            try
            {
                var logger = new LoggerConfiguration()
                                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(elasticSearchUrl)
                                {
                                    AutoRegisterTemplate = true,
                                    IndexFormat = "easybudget-{0:yyyy.MM.dd}"
                                })
                                .WriteTo.Console()
                                .CreateLogger();

                Log.Logger = logger;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
        
        public static void PublishUsage(string machine, string version, string action, Phase executionPhase)
        {
            if (Log.Logger == null)
            {
                return;
            }

            Log.Information(
                executionPhase == Phase.Start
                    ? "The {User} is started execution of {Action} in version {Version} - {Phase}."
                    : "The {User} is completed execution of {Action} in version {Version} - {Phase}.", 
                machine, action, version, executionPhase == Phase.Start ? "Start" : "End");
        }
    }

    public enum Phase
    {
        Start = 0,
        End
    }
}
