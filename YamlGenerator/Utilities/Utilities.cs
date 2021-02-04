/*
Utility Class 
Author: Priom Biswas
Organization: Fraunhofer IESE Kaiserslautern
Date: 04 February 2021
*/

using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace YamlGenerator
{
    public static class Utilities
    {
        static string outputYamlString;
        public static string ReplaceAll(this string seed, char[] chars, char replacementCharacter)
        {
            return chars.Aggregate(seed, (str, cItem) => str.Replace(cItem, replacementCharacter));
        }
        public static string TruncateCommas(string input)
        {
            return Regex.Replace(input, @",+", ",");
        }

        /// <function>
        /// Serializes the object to YAML with the help of YAMLDOTNET library
        /// </function>
        public static void SerializeAsYaml(object obj)
        {
            string yamlData;
            var serializer = new YamlDotNet.Serialization.SerializerBuilder()
                            .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                            .Build();

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                yamlData = writer.ToString();
                outputYamlString = yamlData.Replace("'", string.Empty);
            }
        }

        /// <function>
        /// Display save dialog to save the generated yaml content as .yaml
        /// </function>
        public static void SaveAsYaml(string fileName)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = fileName;
            savefile.Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(savefile.FileName))
                    sw.WriteLine(outputYamlString);
            }
        }
    }
}
