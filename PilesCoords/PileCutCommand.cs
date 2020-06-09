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

    class PileCutCommand : IExternalCommand
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
                message = "Выберите ростверк";
                return Result.Failed;
            }


            using (var t = new Transaction(doc))
            {
                t.Start("Срубка свай под ростверк");
                foreach (Element pileElement in piles)
                {
                    FamilyInstance pile = pileElement as FamilyInstance;
                    if (pile == null) continue;

                    XYZ pileBottomPoint = MyPile.GetPileBottomPoint(pile);
                    XYZ pileTopPointBeforeCut = MyPile.GetPileTopPointBeforeCut(pile);

                    //строю фиктивную линию для определения пересечения с плитой
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

                    if (intersectPointsWithAllSlabs.Count == 0) continue;

                    XYZ slabBottomPoint = Support.GetBottomPoint(intersectPointsWithAllSlabs);

                    double cutLength = pileTopPointBeforeCut.Z - slabBottomPoint.Z - (Settings.pileDepth/304.8);
                    pile.LookupParameter(Settings.paramPileCutHeigth).Set(cutLength);

                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
