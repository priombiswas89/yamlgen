/*
State Diagram Class
Author: Priom Biswas
Organization: Fraunhofer IESE Kaiserslautern
Date: 08 February 2021
*/

using System.Collections.Generic;
using System.Text;

namespace YamlGenerator
{
    public class StateDiagram
    {
        public string refDiagramName { get; set; }
        public string refDiagramId { get; set; }
        public List<State> states { get; set; }
        public List<Transition> transitions { get; set; }
        public string initialState { get; set; }
        public string finalState { get; set; }

        private string stateName;
        private string effectsList;

        /// <function>
        /// Function to create an object of type StateDiagram using Enterprise Architect API which later used
        /// to be serialized as YAML
        /// </function>
        public void CreateObjectForStateDiagram(EA.Repository rep, EA.Diagram diag, StateDiagram diagramElementsObj)
        {
            char[] charsToReplaceFromDiagramId = new char[] { '{', '}' };
            diagramElementsObj.refDiagramName = diag.Name;
            diagramElementsObj.refDiagramId = diag.DiagramGUID.Trim(charsToReplaceFromDiagramId);

            diagramElementsObj.states = new List<State>();
            diagramElementsObj.transitions = new List<Transition>();

            foreach (EA.DiagramObject diagramObj in diag.DiagramObjects)
            {
                int elementId = diagramObj.ElementID;
                EA.Element element = rep.GetElementByID(elementId);

                State stateObj = new State();

                if (element.MetaType == "Pseudostate") // EA state diagram initial state is referred as Pseudostate
                {
                    diagramElementsObj.initialState = element.Name;
                }
                else if (element.MetaType == "FinalState") // EA state diagram final state is referred as FinalState
                {
                    diagramElementsObj.finalState = element.Name;
                }
                else
                {
                    if (element.MetaType == "State")
                    {
                        GetStateName(element.Name, rep, element); 
                        stateObj.name = stateName;
                        diagramElementsObj.states.Add(stateObj);
                    }
                }

                if (element.Methods.Count > 0)
                {
                    GetActionsByState(element, stateObj);
                }
                
                GetAllTransitions(rep, diagramElementsObj, element);
            }
        }

        /// <function>
        /// Recursive function to traverse the project tree and get the name of states as well as nested states
        /// If State1 has a nested state State2, then the name of the State2 will be listed as State1/State2
        /// </function>

        private void GetStateName(string result, EA.Repository rep, EA.Element element)
        {
            if (element.ParentID == 0)
            {
                stateName = result;
                return;
            }
            else
            {
                EA.Element parent = rep.GetElementByID(element.ParentID);
                GetStateName(parent.Name + "/" + result, rep, parent);
            }
        }

        /// <function>
        /// Function to get entry, exit and do actions of a state
        /// </function>
        private void GetActionsByState(EA.Element element, State stateObj)
        {
            StringBuilder entryAc = new StringBuilder();
            StringBuilder exitAc = new StringBuilder();
            StringBuilder doAc = new StringBuilder();
            string entryActions;
            string exitActions;
            string doActions;

            foreach (EA.Method method in element.Methods)
            {
                if (method.ReturnType.Equals("entry"))
                {
                    entryAc.Append(method.Name + ",");
                }
                else if (method.ReturnType.Equals("exit"))
                {
                    exitAc.Append(method.Name + ",");
                }
                else
                {
                    doAc.Append(method.Name + ",");
                }

                if (entryAc.Length != 0)
                {
                    entryActions = entryAc.ToString().TrimEnd(',');
                    stateObj.entryActions = "[" + entryActions + "]";
                }

                if (exitAc.Length != 0)
                {
                    exitActions = exitAc.ToString().TrimEnd(',');
                    stateObj.exitActions = "[" + exitActions + "]";
                }

                if (doAc.Length != 0)
                {
                    doActions = doAc.ToString().TrimEnd(',');
                    stateObj.doActions = "[" + doActions + "]";
                }
            }
        }

        /// <function>
        /// Function to get all transitions of state diagram
        /// </function>
        private void GetAllTransitions(EA.Repository rep, StateDiagram diagramElementsObj, EA.Element element)
        {
            foreach (EA.Connector item in element.Connectors)
            {
                bool isOld = false;
                int clientId = item.ClientID;     // "From" state of a transition, from where the transition begins
                int supplierId = item.SupplierID; // "To" state of a transition, where the transition ends
                char[] charsToReplaceFromEffects = new char[] { ';', '\r', '\t', '\n' };

                EA.Element clientElement = rep.GetElementByID(clientId);
                EA.Element supplierElement = rep.GetElementByID(supplierId);

                Transition transitionObj = new Transition();
                GetStateName(clientElement.Name, rep, clientElement);
                transitionObj.from = stateName;
                GetStateName(supplierElement.Name, rep, supplierElement);
                transitionObj.to = stateName;
                transitionObj.trigger = item.TransitionEvent;

                effectsList = item.TransitionAction;
                effectsList = effectsList.ReplaceAll(charsToReplaceFromEffects, ',');
                effectsList = Utilities.TruncateCommas(effectsList);

                if (string.IsNullOrEmpty(effectsList))
                {
                    transitionObj.effects = "";
                }
                else
                {
                    transitionObj.effects = "[" + effectsList + "]";
                }

                // To remove duplicate transitions returned by Enterprise architect API
                foreach (var transItem in diagramElementsObj.transitions)
                {
                    if (transItem.from.Equals(transitionObj.from) && transItem.to.Equals(transitionObj.to))
                    {
                        isOld = true;
                        break;
                    }
                }
                if (!isOld)
                {
                    diagramElementsObj.transitions.Add(transitionObj);
                }

            }
        }
    }
}
