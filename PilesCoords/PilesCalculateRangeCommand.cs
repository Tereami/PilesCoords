﻿#region License
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

    class PilesCalculateRangeCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new RbsLogger.Logger("PilesCalculateRange"));
            Settings sets = null;
            try { sets = Settings.Activate(); }
            catch (OperationCanceledException ex) { return Result.Cancelled; }
            sets.Save();

            Document doc = commandData.Application.ActiveUIDocument.Document;

            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            List<Element> selems = sel.GetElementIds().Select(i => doc.GetElement(i)).ToList();

            List<FamilyInstance> piles = Support.GetPiles(selems, sets);

            piles = piles.OrderBy(p => int.Parse(Support.GetParameter(p, sets.paramPilePosition).AsString())).ToList();
            Debug.WriteLine("Piles found: " + piles.Count.ToString());

            if (piles.Count == 0)
            {
                message = "Выберите сваи.";
                return Result.Failed;
            }

            Dictionary<string, PileType> pilesKeysAndTypesWithElev = new Dictionary<string, PileType>();
            Dictionary<string, PileType> pilesKeysAndTypes = new Dictionary<string, PileType>();


            foreach (FamilyInstance pile in piles)
            {
                Debug.WriteLine("Current pile id: " + pile.Id.GetElementId().ToString());
                string markString = Support.GetParameter(pile, sets.paramPilePosition).AsString();

                Debug.WriteLine("Pile mark: " + markString);
                int mark = int.Parse(markString);

                string pileUsesPrefix = Support.GetPileUsesPrefix(pile);

                double pileBottomElev =  MyPile.GetPileBottomPoint(pile).Z;

                double pileLengthAfterCut = Support.GetParameter(pile, sets.paramPileLengthAfterCut).AsDouble();
                double pileTopElevAfterCut = Math.Round((pileBottomElev + pileLengthAfterCut) * 304.8);

                pileBottomElev = Math.Round(pileBottomElev * 304.8);

                double slabBottomElev = Support.GetParameter(pile, sets.paramSlabBottomElev).AsDouble();
                slabBottomElev = Math.Round(slabBottomElev * 304.8);
                double pileTopElevBeforeCut = Math.Round(MyPile.GetPileTopPointBeforeCut(pile, sets).Z * 304.8);

                string pileKey_FirstTable = pileUsesPrefix + "_";
                pileKey_FirstTable += pile.Symbol.Name;

                if(sets.sortByBottomElev_Table1)
                    pileKey_FirstTable += "_" + pileBottomElev.ToString();

                if (sets.sortByTopElev_Table1)
                    pileKey_FirstTable += "_" + pileTopElevBeforeCut.ToString();

                if (sets.sortByCutLength_Table1)
                    pileKey_FirstTable += "_" + pileTopElevAfterCut.ToString();

                if (sets.sortBySlabElev_Table1)
                    pileKey_FirstTable += "_" + slabBottomElev.ToString();
                Debug.WriteLine("Key for first table:" + pileKey_FirstTable);

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

                string pileKey_SecondTable = pileUsesPrefix + "_";
                pileKey_SecondTable += pile.Symbol.Name;

                if (sets.sortByBottomElev_Table2)
                    pileKey_SecondTable += "_" + pileBottomElev.ToString();

                if (sets.sortByTopElev_Table2)
                    pileKey_SecondTable += "_" + pileTopElevBeforeCut.ToString();

                if (sets.sortByCutLength_Table2)
                    pileKey_SecondTable += "_" + pileTopElevAfterCut.ToString();

                if (sets.sortBySlabElev_Table2)
                    pileKey_SecondTable += "_" + slabBottomElev.ToString();
                Debug.WriteLine("Key for second table: " + pileKey_SecondTable);
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

            Debug.WriteLine("pilesKeysAndTypes count: " + pilesKeysAndTypes.Count.ToString());
            Debug.WriteLine("pilesKeysAndTypesWithElev count: " + pilesKeysAndTypesWithElev.Count.ToString());

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
                        Support.GetParameter(pile, sets.paramRange, true).Set(range);
                        Support.GetParameter(pile, sets.paramPileTypeNumber, true).Set(typeNumber);
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
                        Support.GetParameter(pile, sets.paramRangeWithElevation, true).Set(range);
                    }
                }
                t.Commit();
            }
            sets.Save();
            Debug.WriteLine("Success");
            return Result.Succeeded;
        }
    }
}
