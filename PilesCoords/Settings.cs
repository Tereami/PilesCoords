using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PilesCoords
{
    public static class Settings
    {
        public static string pileFamilyName = "201_Свая прямоугольная (Фунд)";

        public static double pileDepth = 50;
        public static bool numberingUpDown = true;
        public static string paramPilePosition = "О_Позиция";
        public static string paramPileLength = "Рзм.Длина";
        public static string paramPileLengthAfterCut = "Рзм.ДлинаБалкиИстинная";

        public static string paramRange = "Орг.ДиапазонПозиций";
        public static string paramRangeWithElevation = "Орг.ДиапазонПозиций2";

        public static string paramSlabBottomElev = "Рзм.ОтметкаНизаПлиты";

        public static string paramPileCutHeigth = "Высота срубаемой части";


        public static bool sortByBottomElev_Table1 = false;
        public static bool sortByTopElev_Table1 = false;
        public static bool sortByCutLength_Table1 = false;
        public static bool sortBySlabElev_Table1 = false;

        public static bool sortByBottomElev_Table2 = true;
        public static bool sortByTopElev_Table2 = true;
        public static bool sortByCutLength_Table2 = true;
        public static bool sortBySlabElev_Table2 = false;

    }
}
