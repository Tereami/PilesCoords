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
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
#endregion

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
