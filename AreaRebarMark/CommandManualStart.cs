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

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB; //для работы с элементами модели Revit
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI; //для работы с элементами интерфейса
using Autodesk.Revit.UI.Selection; //работы с выделенными элементами
#endregion

namespace AreaRebarMark
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CommandManualStart : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            double offset = 0; //мм

            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector areas = new FilteredElementCollector(doc)
                .OfClass(typeof(Autodesk.Revit.DB.Structure.AreaReinforcement))
                .WhereElementIsNotElementType();
            FilteredElementCollector paths = new FilteredElementCollector(doc)
                .OfClass(typeof(Autodesk.Revit.DB.Structure.PathReinforcement))
                .WhereElementIsNotElementType();
            List<Element> col = areas.Concat(paths).ToList();
            int count = 0;
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Area rebar mark");
                foreach (Element rebarAreaPath in col)
                {
                    HashSet<string> marks = new HashSet<string>();
                    string rebarSystemMark = "";

                    List<ElementId> rebarIds = null;
                    List<ElementId> curveIds = null;

                    double maxZ = 0;
                    double minZ = 0;

                    if (rebarAreaPath is AreaReinforcement)
                    {
                        AreaReinforcement ar = rebarAreaPath as AreaReinforcement;
                        rebarIds = ar.GetRebarInSystemIds().ToList();
                        curveIds = ar.GetBoundaryCurveIds().ToList();

                        //вычисляю отметки верха и низа
                        List<double> zs = new List<double>();
                        foreach (ElementId curveId in curveIds)
                        {
                            AreaReinforcementCurve curve =
                                doc.GetElement(curveId) as AreaReinforcementCurve;

                            XYZ start = curve.Curve.GetEndPoint(0);
                            double z1 = start.Z;
                            zs.Add(z1);

                            XYZ end = curve.Curve.GetEndPoint(1);
                            double z2 = end.Z;
                            zs.Add(z2);
                        }

                        maxZ = zs.Max();
                        maxZ += offset / 304.8;
                        minZ = zs.Min();
                        minZ += offset / 304.8;
                    }

                    if(rebarAreaPath is PathReinforcement)
                    {
                        PathReinforcement pr = rebarAreaPath as PathReinforcement;
                        rebarIds = pr.GetRebarInSystemIds().ToList();

                        maxZ = pr.get_BoundingBox(doc.ActiveView).Max.Z;
                        minZ = pr.get_BoundingBox(doc.ActiveView).Min.Z;
                    }

                    Parameter topelev = rebarAreaPath.LookupParameter("АрмПлощ.ОтмВерха");
                    Parameter botelev = rebarAreaPath.LookupParameter("АрмПлощ.ОтмНиза");

                    if(topelev == null)
                    {
                        topelev = rebarAreaPath.LookupParameter("Рзм.ОтмВерха");
                    }
                    if(botelev == null)
                    {
                        botelev = rebarAreaPath.LookupParameter("Рзм.ОтмНиза");
                    }

                    if (topelev != null && botelev != null)
                    {
                        topelev.Set(maxZ);
                        botelev.Set(minZ);
                    }

                    double length = 0;
                    foreach (ElementId rebarId in rebarIds)
                    {
                        Element rebar = doc.GetElement(rebarId);
                        string rebarMark = rebar.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                        marks.Add(rebarMark);
                        length = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH).AsDouble();
                    }
                    List<string> marksList = marks.ToList();
                    marksList.Sort();
                    for (int i = 0; i < marks.Count; i++)
                    {
                        rebarSystemMark += marksList[i];
                        if (i < marks.Count - 1) rebarSystemMark += ", ";
                    }


                    rebarAreaPath.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(rebarSystemMark);

                    Parameter lengthParam = rebarAreaPath.LookupParameter("Рзм.Длина");
                    if(lengthParam != null)
                    {
                        lengthParam.Set(length);
                    }

                    count++;
                }
                t.Commit();
            }

            BalloonTip.Show("Успешно!", "Обработано зон: " + count.ToString());

            return Result.Succeeded;
        }
    }
}
