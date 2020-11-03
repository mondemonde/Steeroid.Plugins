using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tasktskie.PAW
{
    public static class MyFileEndPoint
    {
        public static string MyPlayerExe
        {
            get
            {

                Assembly assembly = Assembly.GetAssembly(typeof(UserControl1));
                string MyTasktDirectory = Path.GetDirectoryName(assembly.CodeBase);
                //string filename = Path.GetFileName(assembly.CodeBase);
                //string assemblyPath = Path.Combine(directory, filename);


                MyTasktDirectory = MyTasktDirectory.Replace("file:\\", string.Empty).Trim();

                var exe = string.Format("{0}\\_EXE\\PyAutoWinRec\\pywinauto_recorder.exe", MyTasktDirectory);

                return exe;

            }
        }
    }
}
