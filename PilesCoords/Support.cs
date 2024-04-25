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
#endregion

namespace PilesCoords
{

    /// <summary>
    /// Description of Support.
    /// </summary>
    public class Support
    {
        public static Parameter GetParameter(Element elem, string paramname, bool checkForWritable = false)
        {
            Parameter param = elem.LookupParameter(paramname);
            if(param == null)
            {
                ElementType etype = elem.Document.GetElement(elem.GetTypeId()) as ElementType;
                param = etype.LookupParameter(paramname);
                if (param == null)
                {
                    Debug.WriteLine("No parameter: " + paramname);
                }
            }
            if(checkForWritable && param.IsReadOnly)
            {
                Debug.WriteLine("Parameter is readonly: " + paramname);
            }
            return param;
        }

        public static XYZ GetTopPoint(List<XYZ> points)
        {
            XYZ topPoint = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                XYZ curPoint = points[i];
                if (curPoint.Z > topPoint.Z)
                {
                    topPoint = curPoint;
                }
            }
            return topPoint;
        }
        public static XYZ GetBottomPoint(List<XYZ> points)
        {
            XYZ topPoint = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                XYZ curPoint = points[i];
                if (curPoint.Z < topPoint.Z)
                {
                    topPoint = curPoint;
                }
            }
            return topPoint;
        }



        public static List<FamilyInstance> GetPiles(List<Element> elems, Settings sets)
        {
            Debug.WriteLine("Search piles by name: " + sets.pileFamilyName);
            List<FamilyInstance> piles = elems
                .Where(i => i is FamilyInstance)
                .Cast<FamilyInstance>()
                .Where(i => i.Symbol.FamilyName == sets.pileFamilyName)
                .ToList();
            Debug.WriteLine("Piles found: " + piles.Count.ToString());
            return piles;
        }




        public static Dictionary<string, string> GetMarksRange(Dictionary<string, List<int>> keysAndMarks)
        {
            Dictionary<string, string> keysAndRange = new Dictionary<string, string>();

            foreach (var kvp in keysAndMarks)
            {
                string key = kvp.Key;
                List<int> marks = kvp.Value;
                string range = Support.GetMarksRange(marks);

                keysAndRange.Add(key, range);
            }
            Debug.WriteLine("Mark ranges are created by keys: " + keysAndRange.Count.ToString());
            return keysAndRange;
        }

        public static string GetMarksRange(List<int> marks)
        {
            if (marks.Count == 1) return marks[0].ToString();

            string range = marks[0].ToString();
            if (marks[1] == marks[0] + 1) range += "-";
            if (marks[1] != marks[0] + 1) range += ", ";

            for (int i = 1; i < marks.Count - 1; i++)
            {
                int curMark = marks[i];
                if (marks[i + 1] != curMark + 1)
                {
                    range += curMark + ", ";
                    continue;
                }
                else if (marks[i - 1] != curMark - 1)
                {
                    range += curMark + "-";
                    continue;
                }
            }
            range += marks[marks.Count - 1];
            Debug.WriteLine("Marks range: " + range);
            return range;
        }

        public static ImageType GetImageTypeByName(Document doc, string name)
        {
            Debug.WriteLine("Try to get image by name: " + name);
            List<ImageType> images = new FilteredElementCollector(doc)
                .OfClass(typeof(ImageType))
                .Cast<ImageType>()
                .Where(i => i.Name.Equals(name))
                .ToList();
            Debug.WriteLine("Try to find image by name: " + name + ", found: " + images.Count.ToString());
            if (images.Count == 0)
            {
                List<ImageType> errImgs = new FilteredElementCollector(doc)
                    .OfClass(typeof(ImageType))
                    .Cast<ImageType>()
                    .Where(i => i.Name.Equals("Ошибка.png"))
                    .ToList();
                if (errImgs.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Загрузите в проект картинки для свай!");
                    Debug.WriteLine("Unable to find image Ошибка.png");
                    throw new Exception("Unable to find image Ошибка.png");
                }

                ImageType errImg = errImgs.First();
                return errImg;
            }
            ImageType it = images.First();
            Debug.WriteLine("Image is found, id: " + it.Id.GetElementId().ToString());
            return it;
        }



        public static string GetPileUsesPrefix(Element pile)
        {
            int isAnker = pile.LookupParameter("Анкерная").AsInteger();
            int isTested = pile.LookupParameter("Испытуемая").AsInteger();
            string prefix = "Р";
            if (isAnker != 0 && isTested == 0) prefix = "А";
            if (isAnker == 0 && isTested != 0) prefix = "И";
            Debug.WriteLine("Pile id " + pile.Id.GetElementId().ToString() + " prefix = " + prefix);
            return prefix;
        }

        public static string GetPileUsesText(Element pile)
        {
            int isAnker = pile.LookupParameter("Анкерная").AsInteger();
            int isTested = pile.LookupParameter("Испытуемая").AsInteger();
            string uses = "Рядовая";
            if (isAnker != 0 && isTested == 0) uses = "Анкеруемая";
            if (isAnker == 0 && isTested != 0) uses = "Подвергается стат. испытанию";
            Debug.WriteLine("Pile id " + pile.Id.GetElementId().ToString() + " uses = " + uses);
            return uses;
        }
    }
}
