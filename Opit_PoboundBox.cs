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
using System.Linq;
using static LKS_DEC.Vse_Classi;
using static LKS_DEC.MetaDan_fuction;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Xml.Linq;
using static LKS_DEC.Sem_Postr_Graff;
using static LKS_DEC.DopFunc_Postr_graf;

namespace LKS_DEC.Buttons
{
    [Transaction(TransactionMode.Manual)]

    internal class Opit_PoboundBox : IExternalCommand
    {
        public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
        {
            load(commandData);

            Create_schema_vsech();

            Autodesk.Revit.DB.View view = doc.ActiveView;

            List<ViewPlan> SpPlan = new List<ViewPlan>();
            SpPlan.Add(view as ViewPlan);

            FamilyInstance Fam1  = doc.GetElement(new ElementId(2350341)) as FamilyInstance;

            FamilyInstance Fam2 = doc.GetElement(new ElementId(2350356)) as FamilyInstance;

           // Vse_Prov_and_Stoyak();

            Uzn_VSEGrVidi_Provod_Graf(SpPlan);

            List < List < Provod_Graff >> ddd = Nayti_Cepochki_Vegdu_2_El_na_nesk_vidov(Fam1, Fam2);


            TaskDialog.Show("sss", ddd.Count.ToString());

            TaskDialog.Show("sss", ddd[0].Count.ToString());
            TaskDialog.Show("sss", ddd[1].Count.ToString());
            TaskDialog.Show("sss", ddd[2].Count.ToString());

            // FamilyInstance fi = element as FamilyInstance;

            // List<Line> SpLine = new List<Line>();


            // try
            // {
            //     SpLine = GetAnnotationLines(fi);
            // }
            // catch 
            // { 

            // }


            // if (SpLine.Count < 4)
            // {
            //     SpLine = NaytyBonBox_iz_El(fi);
            // }


            //FamilyInstance fi2 = doc.GetElement(new ElementId(2300286)) as FamilyInstance;

            //Provod_Graff pr = new Provod_Graff(fi2);

            //var Kon1 = pr.Sp_Point[0];
            //var Kon2 = pr.Sp_Point[1];


            // if (SpLine.Count == 4)
            // {               



            //     bool b2 =  IsPointInElem(fi, Kon2.point);

            //     if (b2)
            //     {
            //         TaskDialog.Show("Точка 2", " Точка 2 Подключена");

            //     }

            //     else
            //     {

            //         TaskDialog.Show("Точка 2", " Точка 2 НЕЕЕ подключена");

            //     }


            // }           

            // using (Transaction trans = new Transaction(doc, "Create bounding box rectangle"))
            // {
            //     trans.Start();

            //     // Рабочая плоскость текущего вида
            //     SketchPlane sketchPlane = SketchPlane.Create(doc, view.GenLevel.Id);

            //     // Создаем линии на плане
            //     DetailCurve curveBottom = doc.Create.NewDetailCurve(view, SpLine[0]);
            //     DetailCurve curveTop = doc.Create.NewDetailCurve(view, SpLine[1]);
            //     DetailCurve curveLeft = doc.Create.NewDetailCurve(view, SpLine[2]);
            //     DetailCurve curveRight = doc.Create.NewDetailCurve(view, SpLine[3]);

            //     trans.Commit();
            // }

            return Result.Succeeded;
        }

       

    }

}



