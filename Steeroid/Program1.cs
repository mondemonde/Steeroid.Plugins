using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using taskt.Core.Script;
using taskt.UI.Forms;
using Tasktskie.Common;
using Tasktskie.Common.UI.Forms;

namespace Steeroid
{
    static class Program1
    {
        [STAThread]
        static void Main(string[] args)
        {
            Script.ScriptDirector = new ScriptDirector();
            //frmScriptBuilder.RecordWeb += FrmScriptBuilder_RecordWeb; 
            taskt.Program.Main(args);
        }

        //private static void FrmScriptBuilder_RecordWeb(object sender, EventArgs e)
        //{
        //    var thisBuilder = (frmScriptBuilder)sender;
        //    thisBuilder.Hide();
        //    UIWebRecorderForm frm = new UIWebRecorderForm();
        //    //var frm = new frmWebRecord();
        //    frm.callBackForm = thisBuilder;
        //    frm.ShowDialog();

        //    thisBuilder.pnlCommandHelper.Hide();
        //    thisBuilder.Show();
        //    thisBuilder.BringToFront();
        //}




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
