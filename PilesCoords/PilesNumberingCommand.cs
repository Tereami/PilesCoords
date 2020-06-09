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
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class PilesNumberingCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            List<Element> selems = sel.GetElementIds().Select(i => doc.GetElement(i)).ToList();

            List<FamilyInstance> piles = Support.GetPiles(selems);

            if (piles.Count == 0)
            {
                message = "Выберите сваи.";
                return Result.Failed;
            }

            //Сортирую по координатам
            int numberingUpDown = 1;
            if (Settings.numberingUpDown) numberingUpDown = -1;
            List<FamilyInstance> pilesSorted = piles
                .OrderBy(x => numberingUpDown * Math.Round((x.Location as LocationPoint).Point.Y))
                .ThenBy(x => Math.Round((x.Location as LocationPoint).Point.X))
                .ToList();

            //Указываю позиции по координатам
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Указываю позиции по координатам");
                for (int i = 0; i < pilesSorted.Count; i++)
                {
                    Element pile = pilesSorted[i];
                    pile.LookupParameter(Settings.paramPilePosition).Set((i + 1).ToString());
                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
