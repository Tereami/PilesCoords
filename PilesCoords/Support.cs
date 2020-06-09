using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace PilesCoords
{

    /// <summary>
    /// Description of Support.
    /// </summary>
    public class Support
    {
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



        public static List<FamilyInstance> GetPiles(List<Element> elems)
        {
            List<FamilyInstance> piles = elems
                .Where(i => i is FamilyInstance)
                .Cast<FamilyInstance>()
                .Where(i => i.Symbol.FamilyName == Settings.pileFamilyName)
                .ToList();
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

            return range;
        }

        public static ImageType GetImageTypeByName(Document doc, string name)
        {
            List<ImageType> images = new FilteredElementCollector(doc)
                .OfClass(typeof(ImageType))
                .Cast<ImageType>()
                .Where(i => i.Name.Equals(name))
                .ToList();

            if (images.Count == 0)
            {
                List<ImageType> errImgs = new FilteredElementCollector(doc)
                    .OfClass(typeof(ImageType))
                    .Cast<ImageType>()
                    .Where(i => i.Name.Equals("Ошибка.png"))
                    .ToList();
                if (errImgs.Count == 0)
                {
                    throw new Exception("Загрузите в проект картинки для свай!");
                }

                ImageType errImg = errImgs.First();
                return errImg;
            }

            return images.First();
        }



        public static string GetPileUsesPrefix(Element pile)
        {
            int isAnker = pile.LookupParameter("Анкерная").AsInteger();
            int isTested = pile.LookupParameter("Испытуемая").AsInteger();

            if (isAnker != 0 && isTested == 0) return "А";
            if (isAnker == 0 && isTested != 0) return "И";

            return "Р";
        }

        public static string GetPileUsesText(Element pile)
        {
            int isAnker = pile.LookupParameter("Анкерная").AsInteger();
            int isTested = pile.LookupParameter("Испытуемая").AsInteger();

            if (isAnker != 0 && isTested == 0) return "Анкеруемая";
            if (isAnker == 0 && isTested != 0) return "Подвергается стат. испытанию";

            return "Рядовая";
        }

    }

}
