using System.Collections.Generic;

namespace YamlGenerator
{
    public class Transition
    {
        public string from { get; set; }
        public string to { get; set; }
        public string trigger { get; set; }
        public string effects { get; set; }

        private List<Transition> GetAllTransitions(EA.Repository rep, StateDiagram diagramElementsObj, EA.Element element)
        {
            List<Transition> transitionList = new List<Transition>();
            foreach (EA.Connector item in element.Connectors)
            {
                bool isOld = false;
                int clientId = item.ClientID;
                int supplierId = item.SupplierID;
                char[] charsToReplaceFromEffects = new char[] { ';', '\r', '\t', '\n' };

                EA.Element clientElement = rep.GetElementByID(clientId);
                EA.Element supplierElement = rep.GetElementByID(supplierId);

                Transition transitionObj = new Transition();
                //GetStateName(clientElement.Name, rep, clientElement);
                //transitionObj.from = stateName;
                //GetStateName(supplierElement.Name, rep, supplierElement);
                //transitionObj.to = stateName;
                transitionObj.trigger = item.TransitionEvent;

                effects = item.TransitionAction;
                effects = effects.ReplaceAll(charsToReplaceFromEffects, ',');
                effects = Utilities.TruncateCommas(effects);

                if (string.IsNullOrEmpty(effects))
                {
                    transitionObj.effects = "";
                }
                else
                {
                    transitionObj.effects = "[" + effects + "]";
                }
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
                    transitionList.Add(transitionObj);
                }

            }
            return transitionList;
        }
    }
}
