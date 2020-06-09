using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace PilesCoords
{
    public class PileType
    {
        public List<int> marks { get; set; } = new List<int>();


        public List<Element> piles = new List<Element>();
        public List<MyPile> myPiles = new List<MyPile>();
        public int typeNumber = 0;
        public string typeKey = "";

        public string range = "";

        public void calculateRange()
        {
            range = Support.GetMarksRange(marks);
        }

        public ImageType image = null;


        public string usesPrefix;
    }
}
