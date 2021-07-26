using System.Text.Json;

namespace MyWpfWindowsServiceControlApp
{
    public class Configuration
    {
        public string ConfigurationFileName { get; set; } = "configuration.json";
        public string ImportPath { get; set; }
        public string ExportPath { get; set; }
        public string ImportFilePattern { get; set; }
        public int Delay { get; set; } = 5000;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Configuration()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationFileName"></param>
        public Configuration(string configurationFileName)
        {
            ConfigurationFileName = configurationFileName;
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
                    wr.Close();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="configFileName">a new Configuration File Name</param>
        public void Save(string configFileName)
        {
            ConfigurationFileName = configFileName;
            Save();
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
                Default();
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="configFileName">a new Configuration File Name</param>
        /// <returns></returns>
        public bool Load(string configFileName)
        {
            ConfigurationFileName = configFileName;
            return Load();
        }

        /// <summary>
        /// Default
        /// Use this to create a structured Json 
        /// configuration file with default values 
        /// </summary>
        public void Default()
        {
            ImportPath = "C:\\MyWindowsFileService\\Import";
            ExportPath = "C:\\MyWindowsFileService\\Import\\Done";
            ImportFilePattern = "*.csv";
            Delay = 5000;
            Save();
        }
    }
}
