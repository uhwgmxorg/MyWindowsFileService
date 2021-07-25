using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyWindowsFileService
{
    public class MoveFileWorker : BackgroundService
    {
        ulong _counter = 0;

        public string ImportPath { get; set; } = "\\Import";
        public string ExportPath { get; set; } = "\\Import\\Done";
        public string ImportFilePattern { get; set; } = "*.csv";
        public int Delay { get; set; } = 5000;

        private readonly ILogger<MoveFileWorker> _logger;

        public MoveFileWorker(ILogger<MoveFileWorker> logger)
        {
            _logger = logger;
            LoadConfiguration();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service MyWindowsFileService started");

            try
            {
                /******* Here we do our work *********/
                ProcessFiles(stoppingToken, ImportPath, ExportPath,Delay);
                /*************************************/
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in ExecuteAsync {ex.ToString()}");
            }

            var tcs = new TaskCompletionSource<bool>();
            stoppingToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            await tcs.Task;

            _logger.LogInformation("Service MyWindowsFileService stopped");
        }

        public void ProcessFiles(CancellationToken ct = new CancellationToken(), string importPath = "", string exportPath = "", int delay = 5000)
        {
            bool run;
            int count;
            string nextFile;
            string newFileName;

            if (!String.IsNullOrEmpty(importPath))
                ImportPath = importPath;
            if (!String.IsNullOrEmpty(exportPath))
                ExportPath = exportPath;
            if (!ImportPath.EndsWith("\\")) ImportPath += '\\';
            if (!ExportPath.EndsWith("\\")) ExportPath += '\\';

            run = true;
            while (run)
            {
                count = 1;
                while (count > 0)
                {
                    count = LST.CountFilesInDir(importPath, ImportFilePattern);
                    if (count > 0)
                    {
                        nextFile = LST.GetFirstFileInDir(importPath, ImportFilePattern);
                        newFileName = CreateNewFileName(nextFile, LST.CountFilesInDir(exportPath, ImportFilePattern));
                        if (LST.IsValiFileName(nextFile))
                            ProcessFile(ct, nextFile, newFileName);
                    }
                    if (ct.IsCancellationRequested)
                    {
                        run = false;
                        break;
                    }
                }
                if(run)
                    Thread.Sleep(delay);
            }
        }
        private string CreateNewFileName(string oldFileName, int num)
        {
            string newFileName = "";

            string path = ExportPath;
            string name = Path.GetFileNameWithoutExtension(oldFileName);
            string ext = Path.GetExtension(oldFileName);

            newFileName = $"{path}{name}_{num}{ext}";

            return newFileName;
        }
        public void ProcessFile(CancellationToken ct = new CancellationToken(), string importFile = "", string exportFile = "")
        {
            _counter = 0;

            try
            {
                _logger.LogInformation($"Try to open Import-File {importFile}");
                using (var reader = new StreamReader(importFile))
                {
                    _logger.LogInformation($"Open file {importFile}");
                    string currentLine;
                    DateTime importTime = DateTime.Now;
                    while ((currentLine = reader.ReadLine()) != null)
                    {
                        _counter++;
                        try
                        {
                            /******* Here we can have action with the file content  *********/

                            /****************************************************************/
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"{ex.ToString()}");
                        }
                        if (ct.IsCancellationRequested)
                        {
                            reader.Close();
                            _logger.LogInformation($"A Cancellation was Requested for moving files");
                            break;
                        }
                    }
                    reader.Close();
                    _logger.LogInformation($"Finish moving file from {importFile} to {exportFile} {_counter} records were processed");
                }
                LST.MoveFile(importFile, exportFile);
                _logger.LogInformation($"Move Import-File from {importFile} to {exportFile}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                _logger.LogError(ex, $"Exception in ProcessFile {ex.ToString()}");
            }
        }

        public void LoadConfiguration()
        {
            Configuration c = new Configuration(_logger);
            // Use this to create a structured Json configuration file with default values 
            //c.Default();
            //c.Save();
            c.Load();
            ImportPath = c.ImportPath;
            ExportPath = c.ExportPath;
            ImportFilePattern = c.ImportFilePattern;
            Delay = c.Delay;
            _logger.LogInformation($"ImportPath is  {ImportPath}");
            _logger.LogInformation($"ExportPath is  {ExportPath}");
            _logger.LogInformation($"ImportFilePattern is  {ImportFilePattern}");
            _logger.LogInformation($"Delay is  {Delay}");
        }

        public void SaveConfiguration()
        {
            Configuration c = new Configuration(_logger);
            c.ImportPath = ImportPath;
            c.ExportPath = ExportPath;
            c.ImportFilePattern = ImportFilePattern;
            c.Delay = Delay;
            c.Save();
        }
    }
}
