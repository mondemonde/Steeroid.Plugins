using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tasktskie.Common.Contracts;

namespace Steeroid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            director = new ScriptDirector();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            LoadView();
            LoadUI();
        }

        private CompositionContainer _container;

        [ImportMany(typeof(IPlugger))]
        private IEnumerable<Lazy<IPlugger>> _pluggers;

        public void LoadView()
        {
            //string plugName = ConfigurationSettings.AppSettings["Plugs"].ToString();
            //var connectors = Directory.GetDirectories(plugName);

            string plugName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");// ConfigurationSettings.AppSettings["Plugs"].ToString();
            var connectors = Directory.GetDirectories(plugName);



            var catalog = new AggregateCatalog();
            connectors.ToList().ForEach(connect => catalog.Catalogs.Add(new DirectoryCatalog(connect)));
            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
        }

        ScriptDirector director { get; }

        /// <summary>
        /// Load all IPlggers available in PlugBoard Folder
        /// </summary>
        public void LoadUI()
        {
            try
            {
                //TabItem buttonA = new TabItem();
                //buttonA.Header = "Welcome";
                //buttonA.Height = 30;
                //buttonA.Content = "You welcome :)";
                //tabPlugs.Items.Add(buttonA);



                director.Types = new List<Type>();

                foreach (Lazy<IPlugger> plug in _pluggers)
                {
                    //TabItem button = new TabItem
                    //{
                    //    Header = plug?.Value?.PluggerName,
                    //    Height = 30,
                    //    Content = plug?.Value?.GetPlugger()
                    //};
                    //tabPlugs.Items.Add(button);
                    Console.WriteLine(plug?.Value?.PluggerName);

                    flowLayoutPanel1.Controls.Add(plug?.Value?.GetPlugger());

                    director.Types.Add(plug?.Value?.CommandType);

                    //splitContainer1.Panel2.Controls.Add(plug?.Value?.GetPlugger());//.PluggerName);
                }

                foreach(var c in flowLayoutPanel1.Controls)
                {
                   Console.WriteLine("Control:"  +  c.ToString());
                }
                foreach (var c in director.Types)
                {
                    Console.WriteLine("Control:" + c.ToString());
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);//, "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

    }
}
