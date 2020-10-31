using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using taskt.Core.Script;
using Tasktskie.Common;

namespace Steeroid
{
    static class Program1
    {
        [STAThread]
        static void Main(string[] args)
        {
            Script.ScriptDirector = new ScriptDirector();

            taskt.Program.Main(args);
        }


   

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void xMain()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Script.ScriptDirector = new ScriptDirector();

            Application.Run(new Form1());
        }
    }
}
