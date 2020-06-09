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

    class PilesElevationCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            List<Element> selems = sel.GetElementIds().Select(i => doc.GetElement(i)).ToList();

            List<FamilyInstance> piles = Support.GetPiles(selems);
            List<Element> slabs = selems.Except(piles).ToList();


            if (piles.Count == 0)
            {
                message = "Выберите сваи.";
                return Result.Failed;
            }
            if (slabs.Count == 0)
            {
                message = "Выберите ростверк.";
                return Result.Failed;
            }


            using (Transaction t = new Transaction(doc))
            {
                t.Start("Отметки свай");
                foreach (FamilyInstance pile in piles)
                {
                    

                    XYZ pileTopPointBeforeCut = MyPile.GetPileTopPointBeforeCut(pile);

                    XYZ p1 = new XYZ(pileTopPointBeforeCut.X, pileTopPointBeforeCut.Y, pileTopPointBeforeCut.Z - 3000 / 304.8);
                    XYZ p2 = new XYZ(pileTopPointBeforeCut.X, pileTopPointBeforeCut.Y, pileTopPointBeforeCut.Z + 3000 / 304.8);
                    Line slabLine = Line.CreateBound(p1, p2);

                    List<Element> slabsIntersectWithPile = new List<Element>();
                    List<XYZ> intersectPointsWithAllSlabs = new List<XYZ>();

                    foreach (Element slab in slabs)
                    {
                        List<XYZ> intersectPoints = Intersection.CheckIntersectCurveAndElement(slabLine, slab);

                        if (intersectPoints.Count > 0)
                        {
                            intersectPointsWithAllSlabs.AddRange(intersectPoints);
                        }
                    }

                    XYZ slabBottomPoint = Support.GetBottomPoint(intersectPointsWithAllSlabs);
                    pile.LookupParameter(Settings.paramSlabBottomElev).Set(slabBottomPoint.Z);

                    try
                    {
                        XYZ pileBottomPoint = MyPile.GetPileBottomPoint(pile);
                        pile.LookupParameter("Рзм.ОтметкаРасположения").Set(pileBottomPoint.Z);
                    }
                    catch { }

                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
