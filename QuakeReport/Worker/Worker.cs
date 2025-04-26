using QuakeReport.ParserCore.Interfaces;

namespace QuakeReport.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        readonly IParse _parser;
        readonly string? _logFilePath;

        public Worker(ILogger<Worker> logger,IConfiguration config, IParse parse)
        {
            _logger = logger;
            _logFilePath = config.GetValue<string>("LogFilePath");
            _parser = parse;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(_logFilePath))
                return;

            string? strLine;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Open the text file using a stream reader.
                    using (var sr = new StreamReader(_logFilePath))
                    {
                        // Read the stream as a string
                        while ((strLine = await sr.ReadLineAsync(stoppingToken)) != null)
                        {

                            if (string.IsNullOrEmpty(strLine))
                                continue;

                             _parser.ParseLogLine(strLine);
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Parse error.");
                    await Task.Delay(25000);
                }
            }
        }

    }
}
