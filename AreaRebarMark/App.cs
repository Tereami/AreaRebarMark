#region License
//Данный код опубликован под лицензией Creative Commons Attribution-NonCommercial-ShareAlike.
//Разрешено редактировать, изменять и брать данный код за основу для производных в некоммерческих целях,
//при условии указания авторства и если производные лицензируются на тех же условиях.
//Программа поставляется "как есть". Автор не несет ответственности за возможные последствия её использования.
//Зуев Александр, 2019, все права защищены.
//This code is listed under the Creative Commons Attribution-NonCommercial-ShareAlike license.
//You may redistribute, remix, tweak, and build upon this work non-commercially, 
//as long as you credit the author by linking back and license your new creations under the same terms.
//This code is provided 'as is'. Author disclaims any implied warranty. 
//Zuev Aleksandr, 2019, all rigths reserved.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace AreaRebarMark
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class App : IExternalApplication
    {
        public static string assemblyPath = "";
        public Result OnStartup(UIControlledApplication application)
        {
            assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string tabName = "Weandrevit";
            try { application.CreateRibbonTab(tabName); } catch { }

            RibbonPanel panel1 = application.CreateRibbonPanel(tabName, "Армирование");
            PushButton btn = panel1.AddItem(new PushButtonData(
                "AreaMark",
                "Марка по площади",
                assemblyPath,
                "AreaRebarMark.CommandManualStart")
                ) as PushButton;
            btn.LargeImage = this.PngImageSource("AreaRebarMark.Resources.markLarge.png");
            btn.Image = this.PngImageSource("AreaRebarMark.Resources.markLarge.png");
            btn.ToolTip = "Указание позиции арматурного стержня в зону армирования";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }



        private System.Windows.Media.ImageSource PngImageSource(string embeddedPathname)
        {
            System.IO.Stream st = this.GetType().Assembly.GetManifestResourceStream(embeddedPathname);
            PngBitmapDecoder decoder = new PngBitmapDecoder(st, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

    }
}
