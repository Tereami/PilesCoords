using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;

namespace PilesCoords
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class App : IExternalApplication
    {
        private static UIControlledApplication ctrlApp;

        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyPath = typeof(PilesCoords.App).Assembly.Location;
            string iconsPath = Path.GetDirectoryName(assemblyPath);
            string tabName = "Weandrevit";
            try { application.CreateRibbonTab(tabName); }
            catch { }

            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Сваи");
            PushButton btnAdd = panel.AddItem(new PushButtonData(
                "Numbering",
                "Нумеровать",
                assemblyPath,
                "PilesCoords.PilesNumberingCommand")
                ) as PushButton;
            //btnAdd.LargeImage = new BitmapImage(new Uri(Path.Combine(iconsPath, "add.png")));
            btnAdd.ToolTip = "Нумеровать сваи по положению на плане. Нужные сваи должны быть предварительно выделены.";


            PushButton btnCut = panel.AddItem(new PushButtonData(
                "PileCut",
                "Срубка",
                assemblyPath,
                "PilesCoords.PileCutCommand")
                ) as PushButton;
            //btnCut.LargeImage = new BitmapImage(new Uri(Path.Combine(iconsPath, "add.png")));
            btnCut.ToolTip = "Подрубить сваи под низ ростверка или приямка. При необходимости можно откорретировать результат вручную.";

            PushButton btnElevation = panel.AddItem(new PushButtonData(
                "PileElevation",
                "Отметки",
                assemblyPath,
                "PilesCoords.PilesElevationCommand")
                ) as PushButton;
            //btnCut.LargeImage = new BitmapImage(new Uri(Path.Combine(iconsPath, "add.png")));
            btnElevation.ToolTip = "Выполнить расчет высотных отметок - низа и верха, отметки ростверка, срубки и так далее.";


            PushButton btnRange = panel.AddItem(new PushButtonData(
                "PileRange",
                "Диапазоны",
                assemblyPath,
                "PilesCoords.PilesCalculateRangeCommand")
                ) as PushButton;
            //btnCut.LargeImage = new BitmapImage(new Uri(Path.Combine(iconsPath, "add.png")));
            btnRange.ToolTip = "Выполнить пересчет диапазонов позиций ";


            PushButton btnSettings = panel.AddItem(new PushButtonData(
                "Settings",
                "Настройки",
                assemblyPath,
                "PilesCoords.SettingsCommand")
                ) as PushButton;
            //btnCut.LargeImage = new BitmapImage(new Uri(Path.Combine(iconsPath, "add.png")));
            btnSettings.ToolTip = "Настройки программы";



            return Result.Succeeded;

        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;

        }
    }
}
