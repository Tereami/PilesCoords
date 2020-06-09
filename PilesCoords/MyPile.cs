using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace PilesCoords
{
    public class MyPile
    {
        //public Element pile;
        //public int mark;


        public static XYZ GetPileBottomPoint(Element pileElement)
        {
            FamilyInstance _pile = pileElement as FamilyInstance;
            if (_pile == null) throw new Exception("Элемент - не семейство");

            LocationPoint lp = _pile.Location as LocationPoint;
            XYZ pileBottomPoint = lp.Point;

            return pileBottomPoint;
        }



        public static XYZ GetPileTopPointBeforeCut(Element pileElement)
        {
            XYZ pileBottomPoint = MyPile.GetPileBottomPoint(pileElement);

            FamilyInstance _pile = pileElement as FamilyInstance;


            double pileLengthBeforeCut = 99999;
            try
            {
                pileLengthBeforeCut = _pile.LookupParameter(Settings.paramPileLength).AsDouble();
            }
            catch
            {
                FamilySymbol pileSymbol = _pile.Symbol;
                pileLengthBeforeCut = pileSymbol.LookupParameter(Settings.paramPileLength).AsDouble();
            }

            XYZ pileTopPointBeforeCut = new XYZ(pileBottomPoint.X, pileBottomPoint.Y, pileBottomPoint.Z + pileLengthBeforeCut);

            return pileTopPointBeforeCut;
        }
    }
}
