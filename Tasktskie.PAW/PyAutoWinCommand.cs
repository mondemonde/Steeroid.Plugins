using Common;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using taskt.UI.CustomControls;
using taskt.UI.Forms;
using DevNoteExt.Common;
using System.IO;
using taskt.DevNoteMod;
using taskt.DevNoteMod.Output;
using System.Diagnostics;
using System.Linq;
using Tasktskie.PAW;

namespace taskt.Core.Automation.Commands
{


    [Serializable]
    [Attributes.ClassAttributes.Group("Pyhon")]
    [Attributes.ClassAttributes.Description("This will play PyWinAuto recorder script.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command if you have recorded script")]
    [Attributes.ClassAttributes.ImplementationDescription("This command implements PyWinAutoRec automation.")]
    public class PyAutoWinCommand : ScriptCommand
    {
        //[XmlAttribute]
        //[Attributes.PropertyAttributes.PropertyDescription("Enter Encoded Base64 HTML doc.")]
        //[Attributes.PropertyAttributes.Remarks("")]
        //public string v_Script { get; set; }


        public string v_ProgramName { get; set; }

        public string v_WaitForExit { get; set; }



        [XmlAttribute]
        [Attributes.PropertyAttributes.PropertyDescription("Please indicate the python script file path")]
        [Attributes.PropertyAttributes.PropertyUIHelper(Attributes.PropertyAttributes.PropertyUIHelper.UIAdditionalHelperType.ShowFileSelectionHelper)]
        [Attributes.PropertyAttributes.InputSpecification("Enter or Select the path to the applicable file that should be opened.")]
        [Attributes.PropertyAttributes.SampleUsage(@"D:\Tasktskie.PAW\_EXE\PyAutoWinRec\Record_files\test1.py or [vFilePath]")]
        [Attributes.PropertyAttributes.Remarks("")]
        public string v_FilePath { get; set; }

        public PyAutoWinCommand()
        {
            this.CommandName = "PyAutoWinCommand";
            this.SelectionName = "Start PyAutoWin";
            this.CommandEnabled = true;
            this.CustomRendering = true;

            v_WaitForExit = "Yes";
        }

        public override void RunCommand(object sender)
        {
            //get sending instance
            var engine = (Core.Automation.Engine.AutomationEngineInstance)sender;

            //add vOutput variable
            var requiredVariable = LookupVariable(engine);

            try
            {
              

                //if still not found and user has elected option, create variable at runtime
                if ((requiredVariable == null) && (engine.engineSettings.CreateMissingVariablesDuringExecution))
                {
                    engine.VariableList.Add(new Script.ScriptVariable() { VariableName = v_userVariableName });
                    requiredVariable = LookupVariable(engine);
                }


                //clear DevNotePlayer process
                CloseAll();
                OutputManager.ClearResult();

                // FileEndPointManager.MyPlayerExe2
                string vProgramName = MyFileEndPoint.MyPlayerExe;// v_ProgramName.ConvertToUserVariable(sender);
                string vProgramArgs = v_FilePath;

                System.Diagnostics.Process p;


                p = System.Diagnostics.Process.Start(vProgramName, vProgramArgs);

                if (v_WaitForExit == "Yes")
                {
                    p.WaitForExit();
                }


                if (!File.Exists(FileEndPointManager.DefaultLatestResultFile))
                {
                    //File.Delete(FileEndPointManager.DefaultLatestResultFile);
                    OutputManager.Result = "Success";
                }

            }
            catch (Exception err)
            {
                OutputManager.Result = "Error" + Environment.NewLine + err.Message;
                //throw;
            }

          

            //STEP_.RESULT #10 Katalon result
            //System.Threading.Thread.Sleep(1000);
            //read result
            //OutputManager.ThrowError(OutputManager.ThrowErrorOnResult());

            var result = OutputManager.Result;

            if(result.StartsWith("Error") || result.StartsWith("Failed"))
            {
                OutputManager.ThrowError("ErrorOnPyAutoWinCommand");
            }
           

            if (requiredVariable != null)
            {

                requiredVariable.VariableValue = OutputManager.Result;

            }
            else
            {
                throw new Exception("Attempted to update variable index, but variable was not found. Enclose variables within brackets, ex. {vVariable}");
            }
        }

        public void CloseAll()
        {



            var procs = Process.GetProcesses().OrderBy(p => p.ProcessName);
            foreach (var proc in procs)
            {


                // proc.Kill();
                if (proc.ProcessName.ToLower().Contains("pywinauto_recorder"))
                {
                    try
                    {
                        var msg = string.Format("{0} - {1} ", proc.ProcessName, proc.MainModule.FileName);
                        Console.WriteLine(msg);
                        if (proc.MainModule.FileName.ToLower().Contains("blastasia"))
                        {
                            proc.Kill();
                        }

                        //proc.Kill();
                    }
                    catch (Exception)
                    {


                    }



                }



                // proc.Kill();
           

            }




        }



        public override List<Control> Render(frmCommandEditor editor)
        {
            base.Render(editor);

            //STEP_.Designer #1   RenderedControls
            //RenderedControls.AddRange(CommandControls.CreateKatalonGroupControls("v_Script", this, editor));

            //RenderedControls.AddRange(CommandControls.CreateDefaultInputGroupFor("v_Script", this, editor));
            // RenderedControls.AddRange(CommandControls.CreateDefaultDropdownGroupFor("v_WaitForExit", this, editor));
            RenderedControls.AddRange(CommandControls.CreateDefaultInputGroupFor("v_FilePath", this, editor));

            return RenderedControls;
        }
        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + " [Process: " + v_ProgramName + "]";
        }

        string v_userVariableName = EnumOutputKeywords.vOutput.ToString();// "vOutput";
        private Script.ScriptVariable LookupVariable(Core.Automation.Engine.AutomationEngineInstance sendingInstance)
        {
            //search for the variable
            var requiredVariable = sendingInstance.VariableList.Where(var => var.VariableName == v_userVariableName).FirstOrDefault();

            //if variable was not found but it starts with variable naming pattern
            if ((requiredVariable == null) && (v_userVariableName.StartsWith(sendingInstance.engineSettings.VariableStartMarker)) && (v_userVariableName.EndsWith(sendingInstance.engineSettings.VariableEndMarker)))
            {
                //reformat and attempt
                var reformattedVariable = v_userVariableName.Replace(sendingInstance.engineSettings.VariableStartMarker, "").Replace(sendingInstance.engineSettings.VariableEndMarker, "");
                requiredVariable = sendingInstance.VariableList.Where(var => var.VariableName == reformattedVariable).FirstOrDefault();
            }

            return requiredVariable;
        }

    }
}