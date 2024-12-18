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
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
#endregion

namespace PilesCoords
{
    [Serializable]
    public class Settings
    {
        public bool numberingUpDown = true;
        public int firstNumber = 1;

        public string pileFamilyName = "201_Свая прямоугольная (Фунд_Ур)";
        public double pileDepth = 50;

        public string paramPilePosition = "О_Позиция";
        public static string staticParamPilePosition = "О_Позиция";

        public string paramPileLength = "Рзм.Длина";
        public string paramPileLengthAfterCut = "Рзм.ДлинаБалкиИстинная";

        public string paramRange = "Орг.ДиапазонПозиций";
        public string paramRangeWithElevation = "Орг.ДиапазонПозиций2";

        public string paramSlabBottomElev = "Рзм.ОтметкаНизаПлиты";
        public string paramPileCutHeigth = "Высота срубаемой части";
        public string paramPlacementElevation = "Рзм.ОтметкаРасположения";
        public string paramPileTypeNumber = "N типа рядовой (1-10)";

        public bool sortByPileType_Table1 = true;
        public bool sortByPileUses_Table1 = true;
        public bool sortByBottomElev_Table1 = false;
        public bool sortByTopElev_Table1 = false;
        public bool sortByCutLength_Table1 = false;
        public bool sortBySlabElev_Table1 = false;

        public bool sortByPileType_Table2 = true;
        public bool sortByPileUses_Table2 = true;
        public bool sortByBottomElev_Table2 = true;
        public bool sortByTopElev_Table2 = true;
        public bool sortByCutLength_Table2 = true;
        public bool sortBySlabElev_Table2 = false;

        private static string xmlPath = "";

        public static Settings Activate(bool forceShowSettingsWindow = false)
        {
            Trace.WriteLine("Start activate settins");
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string rbspath = Path.Combine(appdataPath, "bim-starter");
            if (!Directory.Exists(rbspath))
            {
                Trace.WriteLine("Create directory " + rbspath);
                Directory.CreateDirectory(rbspath);
            }
            string solutionName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string solutionFolder = Path.Combine(rbspath, solutionName);
            if (!Directory.Exists(solutionFolder))
            {
                Directory.CreateDirectory(solutionFolder);
                Trace.WriteLine("Create directory " + solutionFolder);
            }
            xmlPath = Path.Combine(solutionFolder, "settings.xml");
            Settings sets = null;

            if (File.Exists(xmlPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamReader reader = new StreamReader(xmlPath))
                {
                    try
                    {
                        sets = (Settings)serializer.Deserialize(reader);
                        Trace.WriteLine("Settings deserialize success");
                    }
                    catch { }
                }
            }
            bool newSettingsCreated = false;
            if (sets == null)
            {
                sets = new Settings();
                Trace.WriteLine("Settings is null, create new one");
                newSettingsCreated = true;
            }
            if (newSettingsCreated || forceShowSettingsWindow)
            {
                FormSettings form = new FormSettings(sets);
                Trace.WriteLine("Show settings form");
                form.ShowDialog();
                if (form.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    Trace.WriteLine("Setting form cancelled");
                    throw new OperationCanceledException();
                }
                sets = form.newSets;
            }

            Settings.staticParamPilePosition = sets.paramPilePosition;
            Trace.WriteLine("Settings success");
            return sets;
        }

        public void Save()
        {
            Trace.WriteLine("Start save settins to file " + xmlPath);
            if (File.Exists(xmlPath)) File.Delete(xmlPath);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (FileStream writer = new FileStream(xmlPath, FileMode.OpenOrCreate))
            {
                serializer.Serialize(writer, this);
            }
            Trace.WriteLine("Save settings success");
        }
    }
}
