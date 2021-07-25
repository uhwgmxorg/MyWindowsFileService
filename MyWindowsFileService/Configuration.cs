using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace MyWindowsFileService
{
    public class Configuration
    {
        public string ConfigurationFileName { get; set; }
        public string ImportPath { get; set; }
        public string ExportPath { get; set; }
        public string ImportFilePattern { get; set; }
        public int Delay { get; set; } = 5000;

        private readonly ILogger<MoveFileWorker> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Configuration()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public Configuration(ILogger<MoveFileWorker> logger)
        {
            _logger = logger;
            ConfigurationFileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\configuration.json";
            _logger.LogInformation($"We are running hier: {Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}");
            _logger.LogInformation($"Try to load configuration from {ConfigurationFileName}");
        }

        /// <summary>
        /// Save
        /// Serialize this class to a Json file
        /// </summary>
        public void Save()
        {
            try
            {
                byte[] jsonUtf8Bytes;
                var options = new JsonSerializerOptions { WriteIndented = false }; // <-- true for better human readable

                jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(this, options);

                string byteString = System.Text.Encoding.UTF8.GetString(jsonUtf8Bytes);
                using (System.IO.StreamWriter wr = new System.IO.StreamWriter(ConfigurationFileName))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(byteString);
                    wr.Write(sb);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Load
        /// Loads the values from the Json file 
        /// (ConfigurationFileName) into this class
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            try
            {
                string byteString = "";
                int BufferSize = 128;
                using (var rd = new System.IO.StreamReader(ConfigurationFileName, System.Text.Encoding.UTF8, true, BufferSize))
                {
                    string line;
                    while ((line = rd.ReadLine()) != null)
                        byteString += line;
                }

                var c = JsonSerializer.Deserialize<Configuration>(byteString);
                ImportPath = c.ImportPath;
                ExportPath = c.ExportPath;
                ImportFilePattern = c.ImportFilePattern;
                Delay = c.Delay;
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception in ExecuteAsync {ex.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// Default
        /// Use this to create a structured Json 
        /// configuration file with default values 
        /// </summary>
        public void Default()
        {
            ImportPath = "\\MyWindowsFileService\\Import";
            ExportPath = "\\MyWindowsFileService\\Import\\Done";
            ImportFilePattern = "*.csv";
            Delay = 5000;
            Save();
        }
    }
}
