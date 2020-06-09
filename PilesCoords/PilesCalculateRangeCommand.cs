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

    class PilesCalculateRangeCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Document doc = commandData.Application.ActiveUIDocument.Document;

            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            List<Element> selems = sel.GetElementIds().Select(i => doc.GetElement(i)).ToList();

            List<FamilyInstance> piles = Support.GetPiles(selems);

            //piles.Sort(new PileComparer());
            piles = piles.OrderBy(p => int.Parse(p.LookupParameter(Settings.paramPilePosition).AsString())).ToList();

            if (piles.Count == 0)
            {
                message = "Выберите сваи.";
                return Result.Failed;
            }


            Dictionary<string, PileType> pilesKeysAndTypesWithElev = new Dictionary<string, PileType>();
            Dictionary<string, PileType> pilesKeysAndTypes = new Dictionary<string, PileType>();


            foreach (FamilyInstance pile in piles)
            {
                string markString = pile.LookupParameter(Settings.paramPilePosition).AsString();
                int mark = int.Parse(markString);

                string pileUsesPrefix = Support.GetPileUsesPrefix(pile);

                double pileBottomElev =  MyPile.GetPileBottomPoint(pile).Z;

                double pileLengthAfterCut = pile.LookupParameter(Settings.paramPileLengthAfterCut).AsDouble();
                double pileTopElevAfterCut = Math.Round((pileBottomElev + pileLengthAfterCut) * 304.8);

                pileBottomElev = Math.Round(pileBottomElev * 304.8);
                double slabBottomElev = Math.Round(pile.LookupParameter(Settings.paramSlabBottomElev).AsDouble() * 304.8);
                double pileTopElevBeforeCut = Math.Round(MyPile.GetPileTopPointBeforeCut(pile).Z * 304.8);


                string pileKey_FirstTable = pileUsesPrefix + "_";
                pileKey_FirstTable += pile.Symbol.Name;

                if(Settings.sortByBottomElev_Table1)
                    pileKey_FirstTable += "_" + pileBottomElev.ToString();

                if (Settings.sortByTopElev_Table1)
                    pileKey_FirstTable += "_" + pileTopElevBeforeCut.ToString();

                if (Settings.sortByCutLength_Table1)
                    pileKey_FirstTable += "_" + pileTopElevAfterCut.ToString();

                if (Settings.sortBySlabElev_Table1)
                    pileKey_FirstTable += "_" + slabBottomElev.ToString();

                if (pilesKeysAndTypes.ContainsKey(pileKey_FirstTable))
                {
                    PileType pType = pilesKeysAndTypes[pileKey_FirstTable];
                    pType.marks.Add(mark);
                    pType.usesPrefix = pileUsesPrefix;
                    pType.piles.Add(pile);
                }
                else
                {
                    PileType pType = new PileType();
                    pType.marks = new List<int>() { mark };
                    pType.piles.Add(pile);
                    pType.usesPrefix = pileUsesPrefix;
                    pilesKeysAndTypes.Add(pileKey_FirstTable, pType);
                }




                //var groups_Table2 = piles.GroupBy(p => new
                //{
                //    pileUsesPrefix = Support.GetPileUsesPrefix(p),
                //    pileBottomElev = MyPile.GetPileBottomPoint(p).Z
                //});

                //foreach (var g in groups_Table2)
                //{

                //    PileType pt = new PileType();
                //    pt.marks.AddRange(g.Select(x => x.Id.IntegerValue));

                //}




                string pileKey_SecondTable = pileUsesPrefix + "_";
                pileKey_SecondTable += pile.Symbol.Name;

                if (Settings.sortByBottomElev_Table2)
                    pileKey_SecondTable += "_" + pileBottomElev.ToString();

                if (Settings.sortByTopElev_Table2)
                    pileKey_SecondTable += "_" + pileTopElevBeforeCut.ToString();

                if (Settings.sortByCutLength_Table2)
                    pileKey_SecondTable += "_" + pileTopElevAfterCut.ToString();

                if (Settings.sortBySlabElev_Table2)
                    pileKey_SecondTable += "_" + slabBottomElev.ToString();


                //pileKey_SecondTable += Math.Round(pileBottomElev * 304.8).ToString();
                //pileKey_SecondTable += Math.Round(pileTopElevBeforeCut * 304.8).ToString() + "_";
                //pileKey_SecondTable += Math.Round(pileTopElevAfterCut * 304.8).ToString() + "_";
                //pileKey_SecondTable += Math.Round(slabBottomElev * 304.8).ToString() + "_";

                if (pilesKeysAndTypesWithElev.ContainsKey(pileKey_SecondTable))
                {
                    PileType pType = pilesKeysAndTypesWithElev[pileKey_SecondTable];
                    pType.marks.Add(mark);
                    pType.usesPrefix = pileUsesPrefix;
                    pType.piles.Add(pile);
                }
                else
                {
                    PileType pType = new PileType();
                    pType.marks = new List<int>() { mark };
                    pType.piles.Add(pile);
                    pType.usesPrefix = pileUsesPrefix;
                    pilesKeysAndTypesWithElev.Add(pileKey_SecondTable, pType);
                }

            }



            using (Transaction t = new Transaction(doc))
            {
                t.Start("Указываю диапазон");

                //Заполняю Диапазон и номера типов
                // KR_Диапазон - сортировка без отметки ростверка, KR_Диапазон2 -  с отметкой ростверка
                int typeNumber = 1;
                foreach (var kvp in pilesKeysAndTypes)
                {
                    PileType pType = kvp.Value;
                    pType.calculateRange();

                    string imageName = pType.usesPrefix;
                    if (typeNumber < 10) imageName += "0" + typeNumber;
                    else imageName += typeNumber;
                    imageName += ".png";

                    ImageType image = Support.GetImageTypeByName(doc, imageName);
                    ElementId imageId = image.Id;

                    foreach (Element pile in pType.piles)
                    {
                        string range = pType.range;
                        pile.LookupParameter(Settings.paramRange).Set(range);
                        pile.LookupParameter("N типа рядовой (1-10)").Set(typeNumber);
                        pile.get_Parameter(BuiltInParameter.ALL_MODEL_IMAGE).Set(imageId);
                    }

                    pType.typeNumber = typeNumber;
                    typeNumber++;
                }

                foreach (var kvp in pilesKeysAndTypesWithElev)
                {
                    PileType pType = kvp.Value;
                    pType.calculateRange();

                    foreach (Element pile in pType.piles)
                    {
                        string range = pType.range;
                        pile.LookupParameter(Settings.paramRangeWithElevation).Set(range);
                    }
                }




                t.Commit();
            }




            return Result.Succeeded;
        }
    }
}
