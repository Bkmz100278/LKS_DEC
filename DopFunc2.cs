using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static LKS_DEC.Data;
using static LKS_DEC.Vse_Classi;
using static LKS_DEC.MetaDan_fuction;
using static LKS_DEC.DopFunc;
using System.Xml.Linq;

namespace LKS_DEC
{
    public class DopFunc2
    {
        public static List<Element> schit_videl_el( )

        {          

            List<Element> selectedElement = new List<Element>();

            // Получение объекта Selection
            Selection sel = uidoc.Selection;

            // Получение всех выделенных идентификаторов элементов
            ICollection<ElementId> selectedIds = sel.GetElementIds();

            if (selectedIds.Count == 0)
            {                
                return selectedElement;
            }          

            // Проход по всем идентификаторам и получение информации об элементах
            foreach (ElementId id in selectedIds)
            {
                Element element = doc.GetElement(id); // Получение элемента по его ID
                if (element != null)
                {                   
                    selectedElement.Add(element) ;
                }
            }
            return selectedElement;
        }

        public static List<FamilyInstance> schit_videl_el_fam()
        {
            List<FamilyInstance> selectedElement = new List<FamilyInstance>();

            // Получение объекта Selection
            Selection sel = uidoc.Selection;

            // Получение всех выделенных идентификаторов элементов
            ICollection<ElementId> selectedIds = sel.GetElementIds();

            if (selectedIds.Count == 0)
            {
                return selectedElement;
            }

            // Проход по всем идентификаторам и получение информации об элементах
            foreach (ElementId id in selectedIds)
            {
                Element element = doc.GetElement(id); // Получение элемента по его ID
                if (element != null)
                {
                    try
                    {
                        FamilyInstance fam = element as FamilyInstance;

                        if (fam != null)
                        {                         

                            if (fam.SuperComponent == null)
                            {
                                selectedElement.Add(fam);
                            }
                        }

                       
                    }
                    catch { }   
                }
            }

            selectedElement.RemoveAll(item => item == null);

            return selectedElement;

        }

        public static List<FamilyInstance> crenie_izapis_potrebit(string ima_syst )
        {
            ElectricalSystemType ElSysTyp = D_revitSystemsDict[ima_syst];

            List<FamilyInstance> SelFams_From = new List<FamilyInstance>();

            try
            {
                SelFams_From = schit_videl_el_fam();

                if (SelFams_From.Count == 0)
                {
                    return SelFams_From;
                }
            }
            catch { }

            List<El_Con> Sp_Cocectors = new List<El_Con>();

            List<FamilyInstance> SelFams_From1 = new List<FamilyInstance>();

            foreach (var fam in SelFams_From)
            {
                try
                {
                    El_Priemn_Fam el1 = new El_Priemn_Fam(fam);
                    List<El_Con> Spnerodklll = el1.Sp_Con_Ne_Podkl_k_Systeme(ElSysTyp.ToString());

                    if (Spnerodklll.Count > 0)
                    {
                        SelFams_From1.Add(fam);
                    }

                }

                catch { }
            }            

            return SelFams_From1;
        }

        public static FamilyInstance Vibor_pan()
        {

            FamilyInstance famisk = null;

            List<FamilyInstance> viborFams = schit_videl_el_fam();

            foreach (var familyInstance in viborFams)
            {
                try
                {
                    Category category = familyInstance.Category;

                    if (category != null && category.Id.IntegerValue == (int)BuiltInCategory.OST_ElectricalEquipment)
                    {
                        famisk = familyInstance;

                        break;
                    }
                }
                catch { }
            }

            return famisk;  
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Это что касается разбития провода на 2а

        private static (FamilyInstance fam, XYZ pt) PickFamilyOnCurveWithPoint(UIDocument uidoc, Autodesk.Revit.DB.Document doc, string prompt)
        {
            var sel = uidoc.Selection;
            Reference r = sel.PickObject(
                ObjectType.Element,
                new LineBasedFamilyFilter(doc, D_nazv_sem_Provoda),
                prompt);

            if (r == null) return (null, null);

            var el = doc.GetElement(r);
            var fam = el as FamilyInstance;
            if (fam == null) return (null, null);

            XYZ pt = r.GlobalPoint;
            return (fam, pt);
        }

        // Фильтр: берем только FamilyInstance с LocationCurve
        private class LineBasedFamilyFilter : ISelectionFilter
        {
            private readonly Autodesk.Revit.DB.Document _doc;
            private readonly string _familyName;

            public LineBasedFamilyFilter(Autodesk.Revit.DB.Document doc, string familyName)
            {
                _doc = doc;
                _familyName = familyName ?? string.Empty;
            }

            public bool AllowElement(Element elem)
            {
                var fi = elem as FamilyInstance;
                if (fi == null) return false;

                // Должен быть линейный экземпляр
                if (!(fi.Location is LocationCurve lc) || !(lc.Curve is Line)) return false;

                // Проверка имени семейства
                var fam = fi.Symbol?.Family;
                if (fam == null) return false;

                return string.Equals(fam.Name, _familyName, StringComparison.OrdinalIgnoreCase);
            }

            public bool AllowReference(Reference reference, XYZ position) => true;
        }

        private static XYZ ProjectPointOnSegment(XYZ a, XYZ b, XYZ p)
        {
            XYZ ab = b - a;
            XYZ ap = p - a;
            double abLen2 = ab.DotProduct(ab);
            if (abLen2 < 1e-12) return a;
            double t = ap.DotProduct(ab) / abLen2;
            // Обрезаем до [0,1]
            t = Math.Max(0.0, Math.Min(1.0, t));
            return a + t * ab;
        }

        private static bool IsPointStrictlyInsideSegment(XYZ a, XYZ b, XYZ p, double tol)
        {
            double len = (b - a).GetLength();
            double ap = (p - a).GetLength();
            double pb = (b - p).GetLength();
            // p внутри, если ap > tol и pb > tol, и ap + pb ≈ len
            return ap > tol && pb > tol && Math.Abs((ap + pb) - len) < 1e-6 * len;
        }

        // Снимок экземплярных параметров (кроме служебных/недоступных)
        private static List<(ElementId id, string name, StorageType st, object val)> SnapshotInstanceParameters(FamilyInstance fi)
        {
            var res = new List<(ElementId, string, StorageType, object)>();
            foreach (Parameter p in fi.Parameters)
            {
                if (p == null || p.IsReadOnly) continue;
                // Пропускаем типовые параметры
                if (p.Definition == null) continue;
                if (p.StorageType == StorageType.None) continue;

                object val = null;
                switch (p.StorageType)
                {
                    case StorageType.Double: val = p.AsDouble(); break;
                    case StorageType.Integer: val = p.AsInteger(); break;
                    case StorageType.String: val = p.AsString(); break;
                    case StorageType.ElementId: val = p.AsElementId(); break;
                }

                if (p.Definition.Name != "Марка")
                {
                    res.Add((p.Id, p.Definition.Name, p.StorageType, val));
                }
            }
            return res;
        }

        private static void ApplyInstanceParameters(FamilyInstance fi, List<(ElementId id, string name, StorageType st, object val)> snapshot)
        {
            foreach (var item in snapshot)
            {
                // Ищем параметр по имени, потому что ElementId параметра у нового экземпляра другой
                Parameter p = fi.LookupParameter(item.name);
                if (p == null || p.IsReadOnly) continue;
                try
                {
                    switch (item.st)
                    {
                        case StorageType.Double: p.Set((double)item.val); break;
                        case StorageType.Integer: p.Set((int)item.val); break;
                        case StorageType.String: p.Set((string)item.val ?? string.Empty); break;
                        case StorageType.ElementId:
                            var id = item.val as ElementId;
                            p.Set(id);
                            break;
                    }
                }
                catch
                {
                    // пропускаем, если параметр недоступен к записи
                }
            }
        }

        public static List<(List<string>, string)> Schit_meta_dan_el(Element el) // считываем все возможные значения метаданных
        {

            List<(List<string>, string)> sp_rez = new List<(List<string>, string)>();

            foreach (var metdan in All_Metadan_in_model)
            {
                string zn = ReadDataFromElement_posp(el, metdan);

                if (!string.IsNullOrEmpty(zn))
                {
                    sp_rez.Add((metdan, zn));
                }
            }

            return sp_rez;
        }

        public static void Prisvoen_meta_dan(Element el, List<(List<string>, string)> sp_metadan)
        {
            if (sp_metadan.Count == 0)
            {
                return;
            }

            foreach (var item in sp_metadan)
            {
                SchemaSetData_univers_posp(el, item.Item1, item.Item2);
            }
        }

        public static List<ElectricalSystem> GetElectricalSystemsForElement(Element elem)
        {
            var systems = new HashSet<ElectricalSystem>();

            // 2) Через коннекторы (универсально для MEP элементов)
            var connMgr = GetConnectorManager(elem);
            if (connMgr != null)
            {
                foreach (Connector c in connMgr.Connectors)
                {
                    if (c.Domain != Domain.DomainElectrical) continue;

                    // Прямо привязанные ElectricalSystem
                    if (c.Owner is ElectricalSystem esys1)
                    {
                        systems.Add(esys1);
                    }

                    // Иногда ElectricalSystem хранится как элемент-владелец сетки, доступный через AllRefs
                    foreach (Connector refc in c.AllRefs)
                    {
                        // У связанных коннекторов может быть владелец ElectricalSystem
                        if (refc.Owner is ElectricalSystem esys2) systems.Add(esys2);
                    }
                }
            }

            return systems.ToList();
        }

        // Вспомогательное: получить ConnectorManager для элемента, если есть
        public static ConnectorManager GetConnectorManager(Element e)
        {
            switch (e)
            {
                case FamilyInstance fi:
                    return fi.MEPModel?.ConnectorManager;
                case MEPCurve mc:
                    return mc.ConnectorManager;
                default:
                    return (e as MEPSystem)?.ConnectorManager; // для некоторых систем
            }
        }

        public static List<(ElectricalSystem, ConnectorSet)> Zagotovka_na_podkl_cep(Element el, List<ElectricalSystem> electricalSystems)
        {

            List<(ElectricalSystem, ConnectorSet)> Con_na_prisv = new List<(ElectricalSystem, ConnectorSet)>();

            if (electricalSystems == null)
            {
                return Con_na_prisv;
            }

            if (electricalSystems.Count == 0)
            {
                return Con_na_prisv;
            }


            foreach (var el_syst in electricalSystems)
            {
                try
                {

                    ElectricalSystemType systtyp = el_syst.SystemType;

                    ConnectorSet connectorSet = new ConnectorSet();
                    {
                        try
                        {
                            Connector con = naiti_con(el as FamilyInstance, systtyp); // D_SPelectricalSystemType["Пожарная сигнализация"] 

                            if (con != null)
                            {
                                connectorSet.Insert(con);

                                Con_na_prisv.Add((el_syst, connectorSet));
                            }
                        }
                        catch { }
                    }
                }
                catch { }


            }

            return Con_na_prisv;
        }

        public static void prisv_el_set(Element el, List<(ElectricalSystem, ConnectorSet)> sp_zag) // это внутри транзакции
        {
            if (sp_zag == null)
            {
                return;
            }

            if (sp_zag.Count == 0)
            {
                return;
            }

            foreach (var zag in sp_zag)
            {
                try
                {
                    zag.Item1.Add(zag.Item2);
                }
                catch { }
            }
        }

        public static void Razbitie_elGraff()
        {
            try
            {
                var pick = PickFamilyOnCurveWithPoint(uidoc, doc, "Выберите семейство-на-кривой и укажите точку на нем");

                if (pick.fam == null || pick.pt == null)
                    return;

                var fam = pick.fam;
                var p3 = pick.pt;

                var locCurve = fam.Location as LocationCurve;
                if (locCurve == null)
                {
                    TaskDialog.Show("Ошибка", "Выбранный элемент не имеет LocationCurve.");
                    return;
                }

                var curve = locCurve.Curve as Line;
                if (curve == null)
                {
                    TaskDialog.Show("Ошибка", "Поддерживается только линейная кривая (Line).");
                    return;
                }

                XYZ p1 = curve.GetEndPoint(0);
                XYZ p2 = curve.GetEndPoint(1);

                // Проекция точки p3 на линию p1-p2 (в плоскости уровня)
                XYZ p3proj = ProjectPointOnSegment(p1, p2, p3);

                // Проверка, что p3proj действительно внутри отрезка (не на концах)
                if (!IsPointStrictlyInsideSegment(p1, p2, p3proj, 1e-6))
                {
                    TaskDialog.Show("Сообщение", "Точка разбиения должна лежать на отрезке и не совпадать с его концами.");
                    return;
                }

                // Подготовим список копируемых экземплярных параметров
                var instParams = SnapshotInstanceParameters(fam);

                var sym = fam.Symbol;
                var levelId = fam.LevelId; // для размещения на том же уровне
                                           // var skPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, p1)); // если нужен рабочий план

                List<(List<string>, string)> schit_meta_dan_el = Schit_meta_dan_el(fam as Element);

                List<ElectricalSystem> sp_syst = GetElectricalSystemsForElement(fam as Element);

                FamilyInstance f1 = null;
                FamilyInstance f2 = null;

                using (var tx = new Transaction(doc, "Разбить семейство на два"))
                {
                    tx.Start();

                    // Удаляем исходный элемент
                    var oldId = fam.Id;
                    doc.Delete(oldId);

                    // Создаём две новые линии
                    Line l1 = Line.CreateBound(p1, p3proj);
                    Line l2 = Line.CreateBound(p3proj, p2);

                    // Вариант 1: создание по кривой, если семейство «line-based»
                    // Большинство line-based семейств размещаются через NewFamilyInstance(Line, FamilySymbol, Level, StructuralType)
                    // Если требуется рабочая плоскость уровня, используйте перегрузку с SketchPlane.


                    // Убедитесь, что тип активирован
                    if (!sym.IsActive) sym.Activate();

                    // Попытка через перегрузку с Level:
                    try
                    {
                        var level = doc.GetElement(levelId) as Level;
                        f1 = doc.Create.NewFamilyInstance(l1, sym, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        f2 = doc.Create.NewFamilyInstance(l2, sym, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    }
                    catch
                    {

                    }

                    if (f1 == null || f2 == null)
                    {
                        tx.RollBack();
                        TaskDialog.Show("Ошибка", "Не удалось создать новые экземпляры семейства по отрезкам.");
                        return;
                    }

                    // Копируем экземплярные параметры
                    ApplyInstanceParameters(f1, instParams);
                    ApplyInstanceParameters(f2, instParams);

                    // Присваиваем метаданные

                    Prisvoen_meta_dan(f1 as Element, schit_meta_dan_el);
                    Prisvoen_meta_dan(f2 as Element, schit_meta_dan_el);

                    tx.Commit();
                }

                List<(ElectricalSystem, ConnectorSet)> zagotovka_na_podkl_cep1 = Zagotovka_na_podkl_cep(f1 as Element, sp_syst);
                List<(ElectricalSystem, ConnectorSet)> zagotovka_na_podkl_cep2 = Zagotovka_na_podkl_cep(f2 as Element, sp_syst);


                using (var tx = new Transaction(doc, "Присвоение групп"))
                {
                    tx.Start();

                    prisv_el_set(f1 as Element, zagotovka_na_podkl_cep1);
                    prisv_el_set(f2 as Element, zagotovka_na_podkl_cep2);

                    tx.Commit();
                }

                string id1 = string.Empty;
                string id2 = string.Empty;

                try
                {
                    id1 = f1.Id.IntegerValue.ToString();
                    id2 = f2.Id.IntegerValue.ToString();
                }
                catch { }

                if (!string.IsNullOrEmpty(id1) && !string.IsNullOrEmpty(id2))
                {
                    TaskDialog.Show("Команда выполнена успешно", "Элемент графа разбит на 2 участка");
                }

                else
                {
                    TaskDialog.Show("Внимание.", "Команда НЕ выполнена.  Элемент графа НЕ разбит.");
                }

                return;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                return;
            }

        }





        /////////////////////////////////////////////////////////////////////////////////////////////////////////////



    }
}
