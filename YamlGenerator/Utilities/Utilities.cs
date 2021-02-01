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
        public static void SerializeAsYaml(StateDiagram diagramElementsObj)
        {
            string yamlData;
            var serializer = new YamlDotNet.Serialization.SerializerBuilder()
                            .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                            .Build();

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, diagramElementsObj);
                yamlData = writer.ToString();
                outputYamlString = yamlData.Replace("'", string.Empty);
            }
        }
        public static void SaveAsYaml(StateDiagram diagramElementsObj)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = diagramElementsObj.refDiagramName;
            savefile.Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(savefile.FileName))
                    sw.WriteLine(outputYamlString);
            }
        }
    }
}
