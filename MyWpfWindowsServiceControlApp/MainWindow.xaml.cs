using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;

namespace MyWpfWindowsServiceControlApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly string SERVICE_NAME = "My Windows File Service";
        ServiceController _service;
        bool _configurationIsDirty = false; 

        public string Revison { get; set; } = " Revison 0";

        bool runMyWindowsFileService = true;
        bool RunMyWindowsFileService 
        { 
            get
            {
                return runMyWindowsFileService;
            }
            set 
            {
                runMyWindowsFileService = value;
                EnableDisableButtons();
                if (runMyWindowsFileService)
                {
                    WindowsFileServiceIndicatorGreen = Visibility.Visible;
                    WindowsFileServiceIndicatorRed = Visibility.Hidden;
                }
                else
                {
                    WindowsFileServiceIndicatorGreen = Visibility.Hidden;
                    WindowsFileServiceIndicatorRed = Visibility.Visible;
                }
            }
        }

        #region INotify Changed Properties
        private Visibility windowsFileServiceIndicatorGreen;
        public Visibility WindowsFileServiceIndicatorGreen
        {
            get { return windowsFileServiceIndicatorGreen; }
            set
            {
                if (value != WindowsFileServiceIndicatorGreen)
                {
                    windowsFileServiceIndicatorGreen = value;
                    OnPropertyChanged("WindowsFileServiceIndicatorGreen");
                }
            }
        }
        private Visibility windowsFileServiceIndicatorRed;
        public Visibility WindowsFileServiceIndicatorRed
        {
            get { return windowsFileServiceIndicatorRed; }
            set
            {
                if (value != WindowsFileServiceIndicatorRed)
                {
                    windowsFileServiceIndicatorRed = value;
                    OnPropertyChanged("WindowsFileServiceIndicatorRed");
                }
            }
        }

        private string configurationFileName;
        public string ConfigurationFileName
        {
            get { return configurationFileName; }
            set
            {
                if (value != ConfigurationFileName)
                {
                    configurationFileName = value;
                    OnPropertyChanged("ConfigurationFileName");
                }
            }
        }
        private string importPath;
        public string ImportPath
        {
            get { return importPath; }
            set
            {
                if (value != ImportPath)
                {
                    if (!String.IsNullOrEmpty(importPath))
                        _configurationIsDirty = true;
                    importPath = value;
                    OnPropertyChanged("ImportPath");
                }
            }
        }
        private string exportPath;
        public string ExportPath
        {
            get { return exportPath; }
            set
            {
                if (value != ExportPath)
                {
                    if (!String.IsNullOrEmpty(exportPath))
                        _configurationIsDirty = true;
                    exportPath = value;
                    OnPropertyChanged("ExportPath");
                }
            }
        }
        private string importFilePattern;
        public string ImportFilePattern
        {
            get { return importFilePattern; }
            set
            {
                if (value != ImportFilePattern)
                {
                    if(!String.IsNullOrEmpty(importFilePattern))
                        _configurationIsDirty = true;
                    importFilePattern = value;
                    OnPropertyChanged("ImportFilePattern");
                }
            }
        }
        private int delay;
        public int Delay
        {
            get { return delay; }
            set
            {
                if (value != Delay)
                {
                    if (delay > 0)
                        _configurationIsDirty = true;
                    delay = value;
                    OnPropertyChanged("Delay");
                }
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

#if DEBUG
            Title += "    Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + Revison;
#else
            Title += "    Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + Revison;
#endif

            _service = new ServiceController(SERVICE_NAME);
            CheckServiceStatus();
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        #region Tab My Windows File Service

        /// <summary>
        /// Button_MyWindowsFileService_Start_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MyWindowsFileService_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _service.Start();
                _service.WaitForStatus(ServiceControllerStatus.Running);
                CheckServiceStatus();
            }
            catch (Exception)
            {
                MessageBox.Show("Error Starting Service!", "MyWpfWindowsServiceControlApp",MessageBoxButton.OK,MessageBoxImage.Error);
            }        
        }

        /// <summary>
        /// Button_MyWindowsFileService_Stop_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MyWindowsFileService_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _service.Stop();
                _service.WaitForStatus(ServiceControllerStatus.Stopped);
                CheckServiceStatus();
            }
            catch (Exception)
            {
                MessageBox.Show("Error Stoping Service!", "MyWpfWindowsServiceControlApp", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Tab Settings

        /// <summary>
        /// Button_ImportDir_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Settings_ImportDir_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ServiceConfigurationFileName = GetFileFromDialog(Properties.Settings.Default.ServiceConfigurationFileName);
            LoadVarsFromConfig();
        }

        /// <summary>
        /// Button_Settings_LoadSettings_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Settings_LoadSettings_Click(object sender, RoutedEventArgs e)
        {
            LoadVarsFromConfig();
        }

        /// <summary>
        /// Button_Settings_SaveSettings_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Settings_SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            //CheckDirectorys();
            SaveVarsToConfig();
        }

        /// <summary>
        /// Button_Settings_SCImportDir_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Settings_SCImportDir_Click(object sender, RoutedEventArgs e)
        {
            ImportPath =  GetPathFromDialog(Properties.Settings.Default.ServiceConfigurationFileName);
            _configurationIsDirty = true;
        }

        /// <summary>
        /// Button_Settings_SCExportDir_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Settings_SCExportDir_Click(object sender, RoutedEventArgs e)
        {
            ExportPath =  GetPathFromDialog(Properties.Settings.Default.ServiceConfigurationFileName);
            _configurationIsDirty = true;
        }

        #endregion

        #endregion
        /******************************/
        /*      Menu Events           */
        /******************************/
        #region Menu Events

        /// <summary>
        /// MenuItem_Exit_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// MenuItem_About_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            string version;
#if DEBUG
            version = "    Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + Revison;
#else
            version = "    Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + Revison;
#endif
            MessageBox.Show("Version " + version, "About MyWpfWindowsServiceControlApp");
        }

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// TabControl_SelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (TabControl.SelectedIndex)
            {
                case 1: // Settings
                    LoadVarsFromConfig();
                    break;
                default:
                    break;
            }
            if (TabControl.SelectedIndex != 1 && _configurationIsDirty)
            {
                MessageBoxResult result = MessageBox.Show($"The configuration has changed, do you want to save it?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    SaveVarsToConfig();
            }
        }

        /// <summary>
        /// TextBoxses_Configuration_TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxses_Configuration_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }

        /// <summary>
        /// Window_Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (_configurationIsDirty)
            {
                MessageBoxResult result = MessageBox.Show($"The configuration has changed, do you want to save it?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    SaveVarsToConfig();
            }
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// CheckDirectorys
        /// </summary>
        private void CheckDirectorys()
        {
            var p = Properties.Settings.Default;
        }
        private void CeckDirAndCreateIt(string dir)
        {
            if (!System.IO.Directory.Exists(dir))
            {
                MessageBoxResult result = MessageBox.Show($"{dir} does not exist. Do you want to create it?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool isExists = System.IO.Directory.Exists(dir);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(dir);
                }
            }
        }

        /// <summary>
        /// GetPathFromDialog
        /// </summary>
        /// <param name="defaultPath"></param>
        /// <returns></returns>
        public string GetPathFromDialog(string defaultPath)
        {
            string selectedPath = defaultPath;
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = defaultPath;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                selectedPath = dialog.SelectedPath;
            }

            return selectedPath;
        }

        /// <summary>
        /// GetFileFromDialog
        /// </summary>
        /// <param name="defaultPath"></param>
        /// <returns></returns>
        public string GetFileFromDialog(string defaultPath)
        {
            string selectedPath = defaultPath;
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.InitialDirectory = defaultPath;
                dialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                selectedPath = dialog.FileName;
            }

            return selectedPath;
        }

        /// <summary>
        /// CheckServiceStatus
        /// </summary>
        public void CheckServiceStatus()
        {
            try
            {
                if (_service.Status == ServiceControllerStatus.Stopped)
                    RunMyWindowsFileService = false;
                if (_service.Status == ServiceControllerStatus.Running)
                    RunMyWindowsFileService = true;
            }
            catch (System.Exception)
            {
                MessageBox.Show("The Service: "+SERVICE_NAME+" seams not installed!", "MyWpfWindowsServiceControlApp",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// EnableDisableButtons
        /// </summary>
        private void EnableDisableButtons()
        {
            Button_MyWindowsFileService_Start.IsEnabled = !RunMyWindowsFileService;
            Button_MyWindowsFileService_Stop.IsEnabled = RunMyWindowsFileService;
        }

        /// <summary>
        /// LoadVarsFromConfig
        /// </summary>
        private void LoadVarsFromConfig()
        {
            var p = Properties.Settings.Default;
            Configuration c = new Configuration(p.ServiceConfigurationFileName);
            c.Load();
            ConfigurationFileName = Path.GetFileName(p.ServiceConfigurationFileName);
            ImportPath = c.ImportPath;
            ExportPath = c.ExportPath;
            ImportFilePattern = c.ImportFilePattern;
            Delay = c.Delay;
        }

        /// <summary>
        /// SaveVarsToConfig
        /// </summary>
        private void SaveVarsToConfig()
        {
            var p = Properties.Settings.Default;
            Configuration c = new Configuration(p.ServiceConfigurationFileName);
            c.ImportPath = ImportPath;
            c.ExportPath = ExportPath;
            c.ImportFilePattern = ImportFilePattern;
            c.Delay = Delay;
            c.Save();
            _configurationIsDirty = false;
        }

        /// <summary>
        /// SetField
        /// for INotify Changed Properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }

}
