if not exist "C:\MyWindowsFileService\ref\" mkdir C:\MyWindowsFileService\ref
if not exist "C:\MyWindowsFileService\Import\" mkdir C:\MyWindowsFileService\Import
if not exist "C:\MyWindowsFileService\Import\Done\" mkdir C:\MyWindowsFileService\Import\Done
if not exist "C:\MyWindowsFileService\Bat\" mkdir C:\MyWindowsFileService\Bat
copy *.bat C:\MyWindowsFileService\Bat
copy ..\MyWpfWindowsServiceControlApp\configuration.json C:\MyWindowsFileService\
copy ..\MyWindowsFileService\bin\Debug\net5.0\ref\*.* C:\MyWindowsFileService\ref
copy ..\MyWindowsFileService\bin\Debug\net5.0\MyWindowsFileService.exe C:\MyWindowsFileService
copy ..\MyWindowsFileService\bin\Debug\net5.0\*.json C:\MyWindowsFileService
copy ..\MyWindowsFileService\bin\Debug\net5.0\*.dll C:\MyWindowsFileService
pause