#region License
/*Данный код опубликован под лицензией Creative Commons Attribution-ShareAlike.
Разрешено использовать, распространять, изменять и брать данный код за основу для производных в коммерческих и
некоммерческих целях, при условии указания авторства и если производные лицензируются на тех же условиях.
Код поставляется "как есть". Автор не несет ответственности за возможные последствия использования.
Зуев Александр, 2020, все права защищены.
This code is listed under the Creative Commons Attribution-ShareAlike license.
You may use, redistribute, remix, tweak, and build upon this work non-commercially and commercially,
as long as you credit the author by linking back and license your new creations under the same terms.
This code is provided 'as is'. Author disclaims any implied warranty.
Zuev Aleksandr, 2020, all rigths reserved.*/
#endregion
#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace PilesCoords
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class PilesElevationCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new RbsLogger.Logger("PilesElevation"));
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            List<Element> selems = sel.GetElementIds().Select(i => doc.GetElement(i)).ToList();

            List<FamilyInstance> piles = Support.GetPiles(selems);
            List<Element> slabs = selems.Except(piles).ToList();
            Debug.WriteLine("Piles count: " + piles.Count.ToString() + ", slabs count: " + slabs.Count.ToString());
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
                    Debug.WriteLine("Current pile id: " + pile.Id.IntegerValue.ToString());
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
                    Debug.WriteLine("SlabBottomPoint Z = " + (slabBottomPoint.Z * 304.8).ToString("F1"));
                    Parameter elevParam = pile.LookupParameter(Settings.paramSlabBottomElev);
                    if(elevParam == null)
                    {
                        TaskDialog.Show("Ошибка", "Нет параметра " + Settings.paramSlabBottomElev);
                        message = "No parameter " + Settings.paramSlabBottomElev;
                        return Result.Failed;
                    }
                    elevParam.Set(slabBottomPoint.Z);

                    try
                    {
                        XYZ pileBottomPoint = MyPile.GetPileBottomPoint(pile);
                        Debug.WriteLine("Pile bottom elevation: " + (pileBottomPoint.Z * 304.8).ToString("F2"));
                        Parameter pileElevParam = pile.LookupParameter(Settings.paramPlacementElevation);
                        if(pileElevParam == null)
                        {
                            Debug.WriteLine("No parameter: " + Settings.paramPlacementElevation);
                        }
                        else
                        {
                            pileElevParam.Set(pileBottomPoint.Z);
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                t.Commit();
            }
            Debug.WriteLine("Piles elevation succeded");
            return Result.Succeeded;
        }
    }
}
