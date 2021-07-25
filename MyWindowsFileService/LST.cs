namespace MyWindowsFileService
{
    /// <summary>
    /// Local Static Tools
    /// This class only contains static methods for utilities
    /// </summary>
    public class LST
    {
        /// <summary>
        /// CountFilesInDir
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        static public int CountFilesInDir(string sdir, string pattern = "*.*")
        {
            int count = 0;

            if (!sdir.EndsWith("\\")) sdir += '\\';
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sdir);
            count = dir.GetFiles(pattern).Length;

            return count;
        }

        /// <summary>
        /// GetFirstFileInDir
        /// </summary>
        /// <param name="sdir"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        static public string GetFirstFileInDir(string sdir, string pattern = "*.*")
        {
            string firstFile = "";

            if (string.IsNullOrEmpty(pattern)) pattern = "*.*";
            if (!sdir.EndsWith("\\")) sdir += '\\';

            try
            {
                string[] files = System.IO.Directory.GetFiles(sdir, pattern);
                if(files.Length > 0)
                    firstFile = files[0];
                else
                    firstFile = "";
            }
            catch (System.Exception)
            {
                firstFile = "";
            }

            return firstFile;
        }

        /// <summary>
        /// CreateDirectory
        /// </summary>
        /// <param name="dir"></param>
        static public void CreateDirectory(string dir)
        {
            try
            {
                bool isExists = System.IO.Directory.Exists(dir);
                if (!isExists)
                    System.IO.Directory.CreateDirectory(dir);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new System.Exception("LST.CreateDirectory", ex);
            }
        }

        /// <summary>
        /// DoesDirectoryExist
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        static public bool DoesDirectoryExist(string dir)
        {
            return System.IO.Directory.Exists(dir);
        }

        /// <summary>
        /// DeleteAllFiles
        /// </summary>
        /// <param name="dir"></param>
        static public void DeleteAllFiles(string dir)
        {
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dir);

                foreach (System.IO.FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new System.Exception("LST.DeleteAllFiles", ex);
            }
        }

        /// <summary>
        /// MoveFile
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="sourceFileName"></param>
        static public void MoveFile(string sourceFileName, string destFileName)
        {
            try
            {
                System.IO.File.Move(sourceFileName, destFileName);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new System.Exception("LST.MoveFile", ex);
            }
        }

        /// <summary>
        /// IsValiFileName
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public bool IsValiFileName(string fileName)
        {
            System.IO.FileInfo fi = null;
            try
            {
                fi = new System.IO.FileInfo(fileName);
            }
            catch (System.ArgumentException) { }
            catch (System.IO.PathTooLongException) { }
            catch (System.NotSupportedException) { }

            if (ReferenceEquals(fi, null))
                return false;
            else
                return true;
        }
    }
}
