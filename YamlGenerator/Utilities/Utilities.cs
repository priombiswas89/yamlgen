using System.Linq;
using System.Text.RegularExpressions;

namespace YamlGenerator
{
    public static class Utilities
    {
        public static string ReplaceAll(this string seed, char[] chars, char replacementCharacter)
        {
            return chars.Aggregate(seed, (str, cItem) => str.Replace(cItem, replacementCharacter));
        }

        public static string TruncateCommas(string input)
        {
            return Regex.Replace(input, @",+", ",");
        }

        //public static void FormatElementName(string result, EA.Repository rep, EA.Element element)
        //{
        //    if (element.ParentID == 0 || element.Elements.Count == 0)
        //    {
        //        return;
        //    }
        //    foreach (EA.Element child in element.Elements)
        //    {
        //        FormatElementName(result + child.Name, rep, child);
        //    }
        //    //if (element.ParentID != 0)
        //    //{
        //    //    EA.Element parent = rep.GetElementByID(element.ParentID);
        //    //    result = parent.Name + "/" + result;
        //    //}
        //    //else
        //    //{
        //    //    result = element.Name;
        //    //}
        //    //return result;
        //}
    }
}

//public static string FormatElementName(string input)
//{
//    string result = input.Substring(input.IndexOf(".") + 1).Trim();
//    result = result.Replace(".", "/");
//    return result;
//}
