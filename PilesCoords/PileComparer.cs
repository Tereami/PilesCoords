using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace PilesCoords
{
    class PileComparer : IComparer<Element>
    {
        public int Compare(Element x, Element y)
        {
            Element elem1 = x as Element;
            string markString1 = elem1.LookupParameter(Settings.paramPilePosition).AsString();
            int mark1 = int.Parse(markString1);

            Element elem2 = y as Element;
            string markString2 = elem2.LookupParameter(Settings.paramPilePosition).AsString();
            int mark2 = int.Parse(markString2);


            if (mark1 > mark2)
                return 1;
            else if (mark1 < mark2)
                return -1;
            else
                return 0;
        }
    }
}
