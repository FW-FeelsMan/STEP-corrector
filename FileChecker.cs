using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace STEP_corrector
{
    class FileChecker
    {
        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException ex)
            {
                MainWindow.LogError(ex.Message);
                return true;
            }
        }
    }
}
