﻿#region License
//Данный код опубликован под лицензией Creative Commons Attribution-NonCommercial-ShareAlike.
//Разрешено использовать, распространять, изменять и брать данный код за основу для производных в некоммерческих целях,
//при условии указания авторства и если производные лицензируются на тех же условиях.
//Код поставляется "как есть". Автор не несет ответственности за возможные последствия использования.
//Зуев Александр, 2019, все права защищены.
//This code is listed under the Creative Commons Attribution-NonCommercial-ShareAlike license.
//You may use, redistribute, remix, tweak, and build upon this work non-commercially, 
//as long as you credit the author by linking back and license your new creations under the same terms.
//This code is provided 'as is'. Author disclaims any implied warranty. 
//Zuev Aleksandr, 2019, all rigths reserved.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaRebarMark
{
    public static class BalloonTip
    {
        public static void Show(string title, string message)
        {
            Autodesk.Internal.InfoCenter.ResultItem ri = new Autodesk.Internal.InfoCenter.ResultItem();

            ri.Category = title;
            ri.Title = message;
            //ri.TooltipText = tolltip; //кажется, это не работает

            ri.Type = Autodesk.Internal.InfoCenter.ResultType.LocalFile;

            ri.IsFavorite = true;
            ri.IsNew = true;

            Autodesk.Windows.ComponentManager
                .InfoCenterPaletteManager.ShowBalloon(ri);
        }
    }
}
