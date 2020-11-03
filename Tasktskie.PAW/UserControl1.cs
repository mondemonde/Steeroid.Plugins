using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tasktskie.Common.Contracts;
using System.ComponentModel.Composition;
using Serilog;
using taskt.Core.Automation.Commands;
using Tasktskie.Common.UI.Forms;

namespace Tasktskie.PAW
{
    [Export(typeof(IPlugger))]
    public partial class UserControl1 : UserControl, IPlugger
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This is name will display in main plug board
        /// </summary>
        public string PluggerName { get; set; } = "Tasktskie.PAW";


        /// <summary>
        /// This will get called when user clicked on Regular option from plug board
        /// </summary>
        /// <returns></returns>
        public UserControl GetPlugger() => this;

        public Type CommandType
        {
            get
            {
                //var obj = new MessageBoxCommand();
                //return obj.GetType();
                return typeof(PyAutoWinCommand);
            }
        }

        public Image Icon {
            get => this.pictureBoxIcon.Image; 
        }

        public bool IsWebRecorder =>false;

        private void button1_Click(object sender, EventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateLogger();

            Log.Information("Ah, there you are!");
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            label1.Text = PluggerName;
        }

        public UIWebRecorderForm GetWebRecorder()
        {
            throw new NotImplementedException();
        }
    }
}
