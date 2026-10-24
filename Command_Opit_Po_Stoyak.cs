using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;
using static LKS_DEC.Data;
using static LKS_DEC.DopFunc;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using static LKS_DEC.Vse_Classi;
using static LKS_DEC.MetaDan_fuction;
using System.Windows.Forms;
using static LKS_DEC.DopFunc_Postr_graf;
using static LKS_DEC.Sem_Postr_Graff;
using Autodesk.Revit.DB.Mechanical;

namespace LKS_DEC.Buttons
{
    [Transaction(TransactionMode.Manual)]

    internal class Command_Opit_Po_Stoyak : IExternalCommand
    {
        public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
        {
            load(commandData);



            //NaytiVseStoyaki();

            //TaskDialog.Show("Количество стояков", D_Vse_StoYak.Count.ToString());

            //Stoyak st = D_Vse_StoYak[0];

            //TaskDialog.Show("Стояк", st.NazvIdentIf);

            //TaskDialog.Show("Количество связан стояков", st.SpSvyaz.Count.ToString());

            //TaskDialog.Show("Количество связан стояков на всех видах", st.SpSvyazVse.Count.ToString());

            //foreach (var stt in st.SpSvyazVse)
            //{
            //    TaskDialog.Show("Стояк", stt.NazvIdentIf);
            //}

            //TaskDialog.Show("Количество видов", st.SPViewPlan.Count.ToString());

            //foreach (var vv in st.SPViewPlan)
            //{
            //    TaskDialog.Show("Вид", vv.Name);
            //}

            //TaskDialog.Show("Количество всех видов", st.VSESPViewPlan.Count.ToString());

            //foreach (var vv in st.VSESPViewPlan)
            //{
            //    TaskDialog.Show("Вид", vv.Name);
            //}

            //TaskDialog.Show("Количество всех видов", st.RoditViewPlan.Name.ToString());
           


            return Result.Succeeded;
        }
    }
}


