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
using System.Text;
using System.Threading.Tasks;
#endregion

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
