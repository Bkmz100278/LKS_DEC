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
using static LKS_DEC.MyEventHandlerLKC;

namespace LKS_DEC.Buttons
{
    [Transaction(TransactionMode.Manual)]

    internal class Command_Glavnaya : IExternalCommand
    {
        public Result Execute(
       ExternalCommandData commandData,
       ref string message,
       ElementSet elements)

        { 
            load(commandData);
            Create_schema_vsech();

            ////////////////////////////////////////////////////////////////////

            Chistka_Modely_ot_nesuch_grupp(); // чистка графа всей модели от несуществующих групп

            ////////////////////////////////////////////////////////////////////

            D_handler1 = new MyEventHandler1();
            D_exEvent1 = ExternalEvent.Create(D_handler1);            

            D_handler2 = new MyEventHandler2();
            D_exEvent2 = ExternalEvent.Create(D_handler2);

            D_handler3 = new MyEventHandler3();
            D_exEvent3 = ExternalEvent.Create(D_handler3);

            D_handler4 = new MyEventHandler4();
            D_exEvent4 = ExternalEvent.Create(D_handler4);

            D_handler5 = new MyEventHandler5();
            D_exEvent5 = ExternalEvent.Create(D_handler5);

            D_handler6 = new MyEventHandler6();
            D_exEvent6 = ExternalEvent.Create(D_handler6);

            D_handler7 = new MyEventHandler7();
            D_exEvent7 = ExternalEvent.Create(D_handler7);

            ////////////////////////////////////////////////////////////////////

            Form_Glavnoe_Okno form_Glavnoe_Okno = new Form_Glavnoe_Okno();

            form_Glavnoe_Okno.Show();        

            /////////////////////////////////////////////////////////////////////////////

            return Result.Succeeded;
        }
    }
}
