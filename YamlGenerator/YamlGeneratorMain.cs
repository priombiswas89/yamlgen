/*
Enterprise Architect YAML Generator Plugin Main Class
Author: Priom Biswas
Organization: Fraunhofer IESE Kaiserslautern
Date: 08 February 2021
*/

using System;
using System.Windows.Forms;

namespace YamlGenerator
{
    public class YamlGeneratorMain
    {
        // To get enterprise architect context menu from plugin menu, in this case YAML generator
        public object EA_GetMenuItems(EA.Repository repository, string location, string menuName)
        {
            if (menuName == "")
                return "-&YAML Generator";
            else
            {
                string[] ret = { "Save diagram as YAML", "About" };
                return ret;
            }
        }
        public void EA_MenuClick(EA.Repository rep, string location, string menuName, string itemName)
        {
            if (itemName == "Save diagram as YAML")
            {
                EA.Diagram diag;

                // To get what a user selected from project browser, a package, a diagram or an element within a diagram
                switch (rep.GetContextItemType()) 
                {
                    case EA.ObjectType.otPackage:
                        {
                            MessageBox.Show("Please select a diagram");
                            break;
                        }
                    case EA.ObjectType.otDiagram:
                        {
                            diag = rep.GetContextObject();
                            
                            if (diag.Type.Equals("Statechart")) // EA State Diagram type is Statechart
                            {
                                StateDiagram stateDiagram = new StateDiagram();
                                stateDiagram.CreateObjectForStateDiagram(rep, diag, stateDiagram); 
                                Utilities.SerializeAsYaml(stateDiagram);
                                Utilities.SaveAsYaml(stateDiagram.refDiagramName);
                            }
                           
                            break;
                        }
                    case EA.ObjectType.otElement:
                        {
                            MessageBox.Show("Please select a diagram");
                            break;
                        }
                }
            }
            else if (itemName == "About")
            {
                MessageBox.Show("Yaml Generator - Version 1.0");
            }
        }
        public void EA_Disconnect() 
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
      
    }
}
