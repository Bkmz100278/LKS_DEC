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
using static LKS_DEC.DopFunc2;
using Autodesk.Revit.DB.Electrical;
using System.Linq;
using static LKS_DEC.Vse_Classi;
using static LKS_DEC.MetaDan_fuction;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Xml.Linq;
using Autodesk.Revit.DB.Events;
//
namespace LKS_DEC
{
    public class MyEventHandlerLKC 
    {       

        public class MyEventHandler1 : IExternalEventHandler // событие 
        {
            public event EventHandler<TestEventArgs> WorkContextRevitFinish;

            public void OnWorkContextRevitFinish(object param)
            {
                if (WorkContextRevitFinish != null && param != null)
                {
                    WorkContextRevitFinish(this, new TestEventArgs(param));
                }
            }

            public void Execute(UIApplication app)
            {
                using (Transaction transaction = new Transaction(app.ActiveUIDocument.Document)) //Создаем транзакцию
                {                    

                    Sozdanie_new_Cepi(D_Sp_Potr, D_Pan, D_Typ_podkl, D_Typ_System, D_Ima_Cepi);

                    OnWorkContextRevitFinish("работа успешно выполнена и транзакция завершена");
                }
            }

            public string GetName()
            {
                return "My Event Handler";
            }
        }

        public class TestEventArgs : EventArgs
        {
            public object Parameter { get; }

            //передаем любые данные
            public TestEventArgs(object _param)
            {
                Parameter = _param;
            }
        }
       

        public class MyEventHandler2 : IExternalEventHandler // событие 
        {
            public event EventHandler<TestEventArgs> WorkContextRevitFinish;

            public void OnWorkContextRevitFinish(object param)
            {
                if (WorkContextRevitFinish != null && param != null)
                {
                    WorkContextRevitFinish(this, new TestEventArgs(param));
                }
            }

            public void Execute(UIApplication app)
            {
                using (Transaction transaction = new Transaction(app.ActiveUIDocument.Document)) //Создаем транзакцию
                {
                   

                    Redakt_Cepi();

                    OnWorkContextRevitFinish("работа успешно выполнена и транзакция завершена");
                }
            }

            public string GetName()
            {
                return "My Event Handler";
            }
        }

        public class MyEventHandler3 : IExternalEventHandler  // событие 
        {
            public event EventHandler<TestEventArgs> WorkContextRevitFinish;

            public void OnWorkContextRevitFinish(object param)
            {
                if (WorkContextRevitFinish != null && param != null)
                {
                    WorkContextRevitFinish(this, new TestEventArgs(param));
                }
            }

            public void Execute(UIApplication app)
            {             

                Prisvoene_Nomerov_v_Element();

                OnWorkContextRevitFinish("работа успешно выполнена и транзакция завершена");
            }

            public string GetName()
            {
                return "My Event Handler3";
            }
        }


        public class MyEventHandler4 : IExternalEventHandler  // событие 
        {
            public event EventHandler<TestEventArgs> WorkContextRevitFinish;

            public void OnWorkContextRevitFinish(object param)
            {
                if (WorkContextRevitFinish != null && param != null)
                {
                    WorkContextRevitFinish(this, new TestEventArgs(param));
                }
            }

            public void Execute(UIApplication app)
            {
                Prisvoene_Nomerov_v_Cepy();

                OnWorkContextRevitFinish("работа успешно выполнена и транзакция завершена");
            }

            public string GetName()
            {
                return "My Event Handler3";
            }
        }


        public class MyEventHandler5 : IExternalEventHandler // событие разбитие провода
        {
            public event EventHandler<TestEventArgs> WorkContextRevitFinish;

            public void OnWorkContextRevitFinish(object param)
            {
                if (WorkContextRevitFinish != null && param != null)
                {
                    WorkContextRevitFinish(this, new TestEventArgs(param));
                }
            }

            public void Execute(UIApplication app)
            {
                using (Transaction transaction = new Transaction(app.ActiveUIDocument.Document)) //Создаем транзакцию
                {
                    Razbitie_elGraff(); 

                    OnWorkContextRevitFinish("работа успешно выполнена и транзакция завершена");
                }
            }

            public string GetName()
            {
                return "My Event Handler";
            }
        }

        public class MyEventHandler6 : IExternalEventHandler // событие удаления групп
        {
            public event EventHandler<TestEventArgs> WorkContextRevitFinish;

            public void OnWorkContextRevitFinish(object param)
            {
                if (WorkContextRevitFinish != null && param != null)
                {
                    WorkContextRevitFinish(this, new TestEventArgs(param));
                }
            }

            public void Execute(UIApplication app)
            {
                using (Transaction transaction = new Transaction(app.ActiveUIDocument.Document)) //Создаем транзакцию
                {
                    Udalene_Cep(D_SpEls_na_Numerac_Dla_Els);

                    OnWorkContextRevitFinish("работа успешно выполнена и транзакция завершена");
                }
            }

            public string GetName()
            {
                return "My Event Handler";
            }
        }


        public class MyEventHandler7 : IExternalEventHandler // событие чистки всех элементов графа в модели
        {
            public event EventHandler<TestEventArgs> WorkContextRevitFinish;

            public void OnWorkContextRevitFinish(object param)
            {
                if (WorkContextRevitFinish != null && param != null)
                {
                    WorkContextRevitFinish(this, new TestEventArgs(param));
                }
            }

            public void Execute(UIApplication app)
            {
                using (Transaction transaction = new Transaction(app.ActiveUIDocument.Document)) //Создаем транзакцию
                {
                    Chistka_Modely_ot_nesuch_grupp();

                    OnWorkContextRevitFinish("работа успешно выполнена и транзакция завершена");
                }
            }

            public string GetName()
            {
                return "My Event Handler";
            }
        }

    }
}
