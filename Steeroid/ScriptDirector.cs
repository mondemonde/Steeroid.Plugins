using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Data;
using System.Reflection;
using taskt.Core.Automation.Commands;
using taskt.Core.Script;
using Tasktskie.Common.Contracts;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Steeroid
{
    //STEP_.MEF #1 ScriptDirector
    public class ScriptDirector : IScriptDirector
    {

        public List<Type> Types { get; set; }

        /// <summary>
        /// Converts and serializes the user-defined commands into an XML file  
        /// </summary>
        public Script SerializeScript(ListView.ListViewItemCollection scriptCommands, List<ScriptVariable> scriptVariables, string scriptFilePath = "")
        {
            var script = new Script();

            //save variables to file

            script.Variables = scriptVariables;

            //save listview tags to command list

            int lineNumber = 1;

            List<ScriptAction> subCommands = new List<ScriptAction>();

            foreach (ListViewItem commandItem in scriptCommands)
            {
                var command = (ScriptCommand)commandItem.Tag;
                command.LineNumber = lineNumber;

                if ((command is BeginNumberOfTimesLoopCommand) || (command is BeginContinousLoopCommand) || (command is BeginListLoopCommand) || (command is BeginIfCommand) || (command is BeginMultiIfCommand) || (command is BeginExcelDatasetLoopCommand) || (command is TryCommand) || (command is BeginLoopCommand) || (command is BeginMultiLoopCommand))
                {
                    if (subCommands.Count == 0)  //if this is the first loop
                    {
                        //add command to root node
                        var newCommand = script.AddNewParentCommand(command);
                        //add to tracking for additional commands
                        subCommands.Add(newCommand);
                    }
                    else  //we are already looping so add to sub item
                    {
                        //get reference to previous node
                        var parentCommand = subCommands[subCommands.Count - 1];
                        //add as new node to previous node
                        var nextNodeParent = parentCommand.AddAdditionalAction(command);
                        //add to tracking for additional commands
                        subCommands.Add(nextNodeParent);
                    }
                }
                else if ((command is EndLoopCommand) || (command is EndIfCommand) || (command is EndTryCommand))  //if current loop scenario is ending
                {
                    //get reference to previous node
                    var parentCommand = subCommands[subCommands.Count - 1];
                    //add to end command // DECIDE WHETHER TO ADD WITHIN LOOP NODE OR PREVIOUS NODE
                    parentCommand.AddAdditionalAction(command);
                    //remove last command since loop is ending
                    subCommands.RemoveAt(subCommands.Count - 1);
                }
                else if (subCommands.Count == 0) //add command as a root item
                {
                    script.AddNewParentCommand(command);
                }
                else //we are within a loop so add to the latest tracked loop item
                {
                    var parentCommand = subCommands[subCommands.Count - 1];
                    parentCommand.AddAdditionalAction(command);
                }

                //increment line number
                lineNumber++;
            }

            //output to xml file
            //XmlSerializer serializer = new XmlSerializer(typeof(Script));
            XmlSerializer serializer = GetSerializer();



            var settings = new XmlWriterSettings
            {
                NewLineHandling = NewLineHandling.Entitize,
                Indent = true
            };

            //if output path was provided
            if (scriptFilePath != "")
            {
                //write to file
                System.IO.FileStream fs;
                using (fs = System.IO.File.Create(scriptFilePath))
                {
                    using (XmlWriter writer = XmlWriter.Create(fs, settings))
                    {
                        serializer.Serialize(writer, script);
                    }
                }
            }


            return script;
        }
        /// <summary>
        /// Deserializes a valid XML file back into user-defined commands
        /// </summary>
        public Script DeserializeFile(string scriptFilePath)
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(Script));

            XmlSerializer serializer = GetSerializer();


            System.IO.FileStream fs;

            //open file stream from file
            using (fs = new System.IO.FileStream(scriptFilePath, System.IO.FileMode.Open))
            {
                //read and return data
                XmlReader reader = XmlReader.Create(fs);
                Script deserializedData = (Script)serializer.Deserialize(reader);
                return deserializedData;
            }



        }
        /// <summary>
        /// Deserializes an XML string into user-defined commands (server sends a string to the client)
        /// </summary>
        public Script DeserializeXML(string scriptXML)
        {
            System.IO.StringReader reader = new System.IO.StringReader(scriptXML);
            XmlSerializer serializer = GetSerializer(); // new XmlSerializer(typeof(Script));
            Script deserializedData = (Script)serializer.Deserialize(reader);
            return deserializedData;
        }

     public   XmlSerializer GetSerializer()
        {
            // use reflection to get all derived types
            //var knownTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
            //    t => typeof(ScriptCommand).IsAssignableFrom(t) || typeof(
            //    MessageBoxCommand).IsAssignableFrom(t) || typeof(Door).IsAssignableFrom(t)).ToArray();

            var knownTypes = Types.ToArray();

            // prepare to serialize a car object
            XmlSerializer serializer = new XmlSerializer(typeof(Script), knownTypes);

            return serializer;
        }

        XmlSerializer GetSerializer(Type T)
        {
            // use reflection to get all derived types
            //var knownTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
            //    t => typeof(ScriptCommand).IsAssignableFrom(t) || typeof(
            //    MessageBoxCommand).IsAssignableFrom(t) || typeof(Door).IsAssignableFrom(t)).ToArray();

            var knownTypes = Types.ToArray();

            // prepare to serialize a car object
            XmlSerializer serializer = new XmlSerializer(T, knownTypes);

            return serializer;
        }


        /// <summary>
        /// Creates a unique 'clone' of an item. Used to create unique clones of commands when changing/updating new parameters.
        /// </summary>
        public T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }


            if (source == null)
            {
                return default(T);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                var formatter = GetSerializer(typeof(T));// new BinaryFormatter();
                                                         //var formatter =  new BinaryFormatter();

                formatter.Serialize(ms, source);
                ms.Position = 0;
               return (T)formatter.Deserialize(ms);
            }


           //const string FILENAME = @"MyObject.xml";            

           // T retunValue;
           // FileStream fs = new FileStream(FILENAME, FileMode.Open);
           // try
           // {
           //     var formatter = GetSerializer(typeof(T));// new BinaryFormatter();
           //                                     //var formatter =  new BinaryFormatter();

           //     // formatter.Context = new StreamingContext(StreamingContextStates.Clone);
           //     formatter.Serialize(fs, source);
           //     fs.Position = 0;
           //     retunValue = (T)formatter.Deserialize(fs);
           // }
           // catch (Exception err)
           // {

           //     Console.WriteLine(err.Message);
           //     retunValue = default(T);
           // }
           // finally
           // {
           //     fs.Close();
           // }
           // return retunValue;

            //const string FILENAME = @"MyObject.dat";

            ////FileStream fileStream = new FileStream(filename, FileMode.Open);
            ////using (MemoryStream ms = new MemoryStream())
            //using (FileStream ms = new FileStream(FILENAME, FileMode.Open))

            //{
            //    var formatter = GetSerializer();// new BinaryFormatter();
            //    //var formatter =  new BinaryFormatter();

            //    // formatter.Context = new StreamingContext(StreamingContextStates.Clone);
            //     formatter.Serialize(ms, source);
            //     //ms.Position = 0;
            //     return (T)formatter.Deserialize(ms);

            //    //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //    //binaryFormatter.Binder = new SearchAssembliesBinder(Assembly.GetExecutingAssembly(), true);

            //    // retunValue = (T)binaryFormatter.Deserialize(fileStream);
            //    // return (T)binaryFormatter.Deserialize(ms);
            //}

            ////const string FILENAME = @"MyObject.dat";

            ////// Serialize
            ////BinaryFormatterHelper.Write(source, FILENAME);

            ////// Deserialize
            ////return BinaryFormatterHelper.Read<T>(FILENAME, Assembly.GetExecutingAssembly()); // Current Assembly where the dll is referenced


        }
    }
}
