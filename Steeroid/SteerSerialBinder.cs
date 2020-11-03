using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using taskt;

namespace Steeroid
{
    //https://stackoverflow.com/questions/5170333/binaryformatter-deserialize-unable-to-find-assembly-after-ilmerge
    public static class BinaryFormatterHelper
    {
        public static T Read<T>(string filename, Assembly currentAssembly)
        {
            T retunValue;
            FileStream fileStream = new FileStream(filename, FileMode.Open);

            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Binder = new SearchAssembliesBinder(currentAssembly, true);
                retunValue = (T)binaryFormatter.Deserialize(fileStream);
            }
            finally
            {
                fileStream.Close();
            }

            return retunValue;
        }

        public static void Write<T>(T obj, string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fileStream, obj);
            }
            finally
            {
                fileStream.Close();
            }
        }
    }

    public class SearchAssembliesBinder : SerializationBinder
    {
        private readonly bool _searchInDlls;
        private readonly Assembly _currentAssembly;

        public SearchAssembliesBinder(Assembly currentAssembly, bool searchInDlls)
        {
            _currentAssembly = currentAssembly;
            _searchInDlls = searchInDlls;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            List<AssemblyName> assemblyNames = new List<AssemblyName>();
            assemblyNames.Add(_currentAssembly.GetName()); // EXE

            foreach (var item in taskt.Program.MyBuilder.MyScriptDirector.Types)
            {
               var asm=  Assembly.GetAssembly(item);
                assemblyNames.Add(asm.GetName());

            }

           


            if (_searchInDlls)
            {
                assemblyNames.AddRange(_currentAssembly.GetReferencedAssemblies()); // DLLs
            }

            foreach (AssemblyName an in assemblyNames)
            {
                var typeToDeserialize = GetTypeToDeserialize(typeName, an);
                if (typeToDeserialize != null)
                {
                    return typeToDeserialize; // found
                }
            }

            return null; // not found
        }

        private static Type GetTypeToDeserialize(string typeName, AssemblyName an)
        {
            string fullTypeName = string.Format("{0}, {1}", typeName, an.FullName);
            var typeToDeserialize = Type.GetType(fullTypeName);
           //var typeToDeserialize = Type.GetType(typeName);


            return typeToDeserialize;
        }
    }
}
