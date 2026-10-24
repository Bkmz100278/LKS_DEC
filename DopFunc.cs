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
using System.Xml.Linq;
using System.Windows.Forms;
using static LKS_DEC.Sem_Postr_Graff;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LKS_DEC
{
    public class DopFunc
    {
        public static void Sozdanie_new_Cepi(List<FamilyInstance> Sp_Potr, FamilyInstance Pan, string Typ_podkl, string imasystemi_podkl, string Marker_Cepi) // Это новое...
        {

            if ((Sp_Potr == null) || (Pan == null))
            {
                return;
            }

            if (Sp_Potr.Count == 0)
            {
                return;
            }

            string zn_komplect = Pan.LookupParameter(D_Ima_Par_Kompl).AsString();

            MarkerCep markc_kon = new MarkerCep();
            MarkerCep markc_nach = new MarkerCep();

            ElectricalSystemType elsysttyp = D_revitSystemsDict[imasystemi_podkl];

            List<Connector> SpCon = new List<Connector>();

            if (Sp_Potr.Count > 0)
            {
                for (int i = 0; i < Sp_Potr.Count; i++)
                {
                    try
                    {
                        Connector con = naiti_con(Sp_Potr[i], elsysttyp);

                        if (con != null)
                        {
                            SpCon.Add(con);
                        }

                    }
                    catch { }
                }
            }

            if (Typ_podkl == D_Pust_Znach)
            {
                markc_kon.Sposob = markc_nach.Sposob;
            }

            if (!D_str_var_SP.Contains(markc_kon.Sposob))
            {
                markc_kon.Sposob = D_str_var_SP[0];
            }

            markc_kon.Naim = Marker_Cepi;

            int Flag_na_postr = 0;

            if ((SpCon.Count == 1) && (Flag_na_postr == 0))

            {
                Flag_na_postr = 1;

                List<ElectricalSystem> SP_cep = new List<ElectricalSystem>();
                List<FamilyInstance> SP_Fam = new List<FamilyInstance>();

                markc_kon.Nomer = 1;

                markc_kon.Zapakovka();

                using (Transaction tx = new Transaction(doc, "Create Fire Alarm Circuit"))
                {
                    tx.Start();

                    ElectricalSystem circuit = ElectricalSystem.Create(SpCon[0], elsysttyp);

                    circuit.SelectPanel(Pan);

                    SchemaSetData_univers_posp(circuit as Element, MetaData_for_Cep_Dan, markc_kon.Stroca_Itog);

                    zapis_nom_con_v_Fam(SpCon[0], 1);

                    SP_cep.Add(circuit);

                    SP_Fam.Add(Sp_Potr[0]);

                    tx.Commit();
                }

                if (SP_cep.Count > 0)
                {
                    List<ElementId> SpNaVidel = new List<ElementId>();

                    for (int i = 0; i < SP_cep.Count; i++)
                    {
                        ElSet ElSet_nach = new ElSet(SP_cep[i]);

                        // ElSet_nach.Podkl_Cepi_Graf();

                        pust_nazv_cepi(SP_cep[i], zn_komplect);

                        ElSet_nach.Podkl_Cepi_Graf_S_Vidami();

                        SpNaVidel.Add(SP_cep[i].Id);
                    }

                    uidoc.Selection.SetElementIds(SpNaVidel);
                }

            }

            if ((Typ_podkl == D_str_var_podkl_kaskad) && (Flag_na_postr == 0))
            {
                Flag_na_postr = 1;

                List<ElectricalSystem> SP_cep = new List<ElectricalSystem>();

                using (Transaction tx = new Transaction(doc, "Create Fire Alarm Circuit"))
                {
                    tx.Start();

                    List<FamilyInstance> SP_Fam = new List<FamilyInstance>();

                    ElectricalSystem circuit = ElectricalSystem.Create(SpCon[0], elsysttyp);

                    zapis_nom_con_v_Fam(SpCon[0], 1);

                    markc_kon.Nomer = 1;
                    markc_kon.Zapakovka();

                    SchemaSetData_univers_posp(circuit as Element, MetaData_for_Cep_Dan, markc_kon.Stroca_Itog);

                    SP_cep.Add(circuit);

                    SP_Fam.Add(Sp_Potr[0]);

                    for (int i = 1; i < SpCon.Count; i++)

                        try
                        {
                            circuit = ElectricalSystem.Create(SpCon[i], elsysttyp);

                            zapis_nom_con_v_Fam(SpCon[i], i + 1);

                            markc_kon.Nomer = i + 1;
                            markc_kon.Zapakovka();

                            SchemaSetData_univers_posp(circuit as Element, MetaData_for_Cep_Dan, markc_kon.Stroca_Itog);

                            SP_cep.Add(circuit);

                            SP_Fam.Add(SpCon[i].Owner as FamilyInstance);
                        }
                        catch { }

                    try
                    {
                        SP_cep[0].SelectPanel(Pan);
                    }
                    catch { }

                    for (int i = 1; i < SP_cep.Count; i++)
                    {
                        try
                        {
                            SP_cep[i].SelectPanel(SP_Fam[i - 1]);
                        }
                        catch { }
                    }

                    tx.Commit();
                }

                if (SP_cep.Count > 0)
                {
                    List<ElementId> SpNaVidel = new List<ElementId>();

                    for (int i = 0; i < SP_cep.Count; i++)
                    {
                        ElSet ElSet_nach = new ElSet(SP_cep[i]);

                        pust_nazv_cepi(ElSet_nach.el_syst, zn_komplect);

                        ElSet_nach.Podkl_Cepi_Graf_S_Vidami();

                        SpNaVidel.Add(SP_cep[i].Id);
                    }

                    uidoc.Selection.SetElementIds(SpNaVidel);
                }

                // pust_nazv_cepi(SP_cep , zn_komplect);
            }

            if ((Typ_podkl == D_str_var_podkl_posled) && (Flag_na_postr == 0))
            {
                Flag_na_postr = 1;

                ElectricalSystem circuit = null;

                using (Transaction tx = new Transaction(doc, "Create Fire Alarm Circuit"))
                {
                    tx.Start();

                    circuit = ElectricalSystem.Create(SpCon[0], elsysttyp);

                    zapis_nom_con_v_Fam(SpCon[0], 1);

                    markc_kon.Nomer = 1;
                    markc_kon.Zapakovka();

                    SchemaSetData_univers_posp(circuit as Element, MetaData_for_Cep_Dan, markc_kon.Stroca_Itog);

                    if (SpCon.Count > 0)
                    {
                        for (int i = 1; i < SpCon.Count; i++)
                        {
                            try
                            {
                                ConnectorSet connectorSet = new ConnectorSet();

                                connectorSet.Insert(SpCon[i]);

                                circuit.Add(connectorSet);

                                zapis_nom_con_v_Fam(SpCon[i], i + 1);
                            }
                            catch { }
                        }

                        circuit.SelectPanel(Pan);

                    }

                    tx.Commit();
                }

                pust_nazv_cepi(circuit, zn_komplect);

                ElSet elSet = new ElSet(circuit);

                // elSet.Podkl_Cepi_Graf();  Подключения графа

                elSet.Podkl_Cepi_Graf_S_Vidami();


                D_Rabocia_Gruppa_El = elSet;
            }

            if ((Typ_podkl == D_str_var_podkl_zvezda) && (Flag_na_postr == 0))
            {
                Flag_na_postr = 1;

                List<ElectricalSystem> SP_cep = new List<ElectricalSystem>();

                using (Transaction tx = new Transaction(doc, "Create Fire Alarm Circuit"))
                {
                    tx.Start();

                    ElectricalSystem circuit = ElectricalSystem.Create(SpCon[0], elsysttyp);

                    zapis_nom_con_v_Fam(SpCon[0], 1);

                    markc_kon.Nomer = 1;
                    markc_kon.Zapakovka();

                    SchemaSetData_univers_posp(circuit as Element, MetaData_for_Cep_Dan, markc_kon.Stroca_Itog);

                    circuit.SelectPanel(Pan);

                    SP_cep.Add(circuit);

                    if (SpCon.Count > 0)
                    {

                        for (int i = 1; i < SpCon.Count; i++)
                        {
                            markc_kon.Nomer = i + 1;
                            markc_kon.Zapakovka();

                            circuit = ElectricalSystem.Create(SpCon[i], elsysttyp);

                            zapis_nom_con_v_Fam(SpCon[i], i + 1);

                            SchemaSetData_univers_posp(circuit as Element, MetaData_for_Cep_Dan, markc_kon.Stroca_Itog);

                            circuit.SelectPanel(Pan);

                            SP_cep.Add(circuit);
                        }

                        circuit.SelectPanel(Pan);

                    }

                    tx.Commit();
                }

                if (SP_cep.Count > 0)
                {
                    List<ElementId> SpNaVidel = new List<ElementId>();

                    for (int i = 0; i < SP_cep.Count; i++)
                    {
                        ElSet ElSet_nach = new ElSet(SP_cep[i]);

                        pust_nazv_cepi(ElSet_nach.el_syst, zn_komplect);

                        ElSet_nach.Podkl_Cepi_Graf_S_Vidami();

                        SpNaVidel.Add(SP_cep[i].Id);
                    }

                    // pust_nazv_cepi(SP_cep, zn_komplect);

                    uidoc.Selection.SetElementIds(SpNaVidel);


                }
            }


        }

        public static void Redakt_Cepi() // Редактирование цепи
        {
            List<FamilyInstance> Sp_Potr = D_Sp_Potr;

            ElSet ElSet_nach = D_Rabocia_Gruppa_El;

            ElectricalSystem el_syst = ElSet_nach.el_syst;

            Chistka_Cepi(Sp_Potr, ElSet_nach);   // Это обнуление цепи и подключение основных потребителей           

            /////////////////////////////////////////////////////////////////////////////////////

            ElSet ElSet_nach11 = new ElSet(el_syst);

            ElSet_nach11.Podkl_Cepi_Graf_S_Vidami();

            D_Rabocia_Gruppa_El = ElSet_nach11;
        }

        public static List<FamilyInstance> GetNonIntersectingElements(List<FamilyInstance> Sp_Potr, List<FamilyInstance> ListPotr1) // Получаем список элементов из ListPotr1 которые не входят в Sp_Potr 
        {
            // Создаем HashSet для хранения IntegerValue из Sp_Potr
            HashSet<int> potrIds = new HashSet<int>(Sp_Potr.Select(instance => instance.Id.IntegerValue));

            // Создаем список для результата
            List<FamilyInstance> result = new List<FamilyInstance>();

            // Проходим по ListPotr1 и проверяем наличие каждого элемента в potrIds
            foreach (var instance in ListPotr1)
            {
                // Проверяем, есть ли IntegerValue в HashSet
                if (!potrIds.Contains(instance.Id.IntegerValue))
                {
                    result.Add(instance); // Добавляем в результат, если элемент отсутствует
                }
            }

            return result; // Возвращаем список с результатом
        }

        public static List<FamilyInstance> GetUniqueElements(List<FamilyInstance> Sp_Potr, List<FamilyInstance> ListPotr1)  // Получаем список элементов из Sp_Potr которые не входят в ListPotr1
        {
            // Создаем HashSet для хранения ID элементов из ListPotr1
            HashSet<int> potrIds = new HashSet<int>(ListPotr1.Select(instance => instance.Id.IntegerValue));

            // Создаем список для хранения уникальных элементов из Sp_Potr
            List<FamilyInstance> result = new List<FamilyInstance>();

            // Перебираем элементы из Sp_Potr
            foreach (var instance in Sp_Potr)
            {
                // Если ID текущего элемента из Sp_Potr отсутствует в ListPotr1
                if (!potrIds.Contains(instance.Id.IntegerValue))
                {
                    result.Add(instance); // Добавляем элемент в результат
                }
            }

            return result; // Возвращаем результирующий список
        }

        /// /////////////////////////////////////////////////////////////////////////////

        public static void pust_nazv_cepi(ElectricalSystem els, string zn_komplect) // Присваивание новой цепи
        {

            string zn = els.LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).AsString();

            if (string.IsNullOrEmpty(zn) || (zn == ""))
            {

                using (Transaction tx = new Transaction(doc, "Create Fire Alarm Circuit"))
                {
                    tx.Start();

                    string Nom_caep = els.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();

                    string Nom_caep_itog = "НЕНАЗНАЧ_" + Nom_caep;

                    els.LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).Set(Nom_caep_itog);

                    els.LookupParameter(D_Ima_Par_Kompl).Set(zn_komplect);

                    tx.Commit();
                }
            }

            else
            {
                using (Transaction tx = new Transaction(doc, "Create Fire Alarm Circuit"))
                {
                    tx.Start();

                    els.LookupParameter(D_Ima_Par_Kompl).Set(zn_komplect);

                    tx.Commit();
                }

            }

        }

        public static void pust_nazv_cepi(List<ElectricalSystem> SPels, string zn_komplect) // Присваивание новой цепи
        {
            try
            {
                using (Transaction tx = new Transaction(doc, "Присоение пустого значения цепи"))

                {
                    tx.Start();

                    foreach (ElectricalSystem els in SPels)
                    {
                        string zn = els.LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).AsString();

                        if (string.IsNullOrEmpty(zn) || (zn == ""))
                        {

                            string Nom_caep = els.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();

                            string Nom_caep_itog = "НЕНАЗНАЧ_" + Nom_caep;

                            els.LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).Set(Nom_caep_itog);

                            els.LookupParameter(D_Ima_Par_Kompl).Set(zn_komplect);
                        }

                        else
                        {
                            els.LookupParameter(D_Ima_Par_Kompl).Set(zn_komplect);
                        }
                    }

                    tx.Commit();
                }
            }
            catch { }
        }


        /// /////////////////////////////////////////////////////////////////////////////        

        public static (Connector, ElectricalSystemType?) NaitiCon1El(FamilyInstance fam)  // Найти коннектор нжного типа у элемента вместе с типом    
        {
            (Connector, ElectricalSystemType?) result = (null, null);

            try
            {
                List<Connector> SpConEl = naiti_SP_con_u_El(fam, 1);

                if (SpConEl.Count == 0)
                {
                    return result;
                }


                foreach (var con in SpConEl)
                {
                    try
                    {

                        foreach (var syst in D_SPelectricalSystemType)
                        {
                            if (con.ElectricalSystemType.ToString() == syst.ToString())
                            {
                                ElectricalSystem els = Naiti_Syst_El_u_Con(con);

                                if (els == null)
                                {
                                    result = (con, con.ElectricalSystemType);
                                    return result;
                                }
                            }
                        }
                    }
                    catch { }
                }

            }
            catch { }

            return result;
        }

        //public static Connector naiti_con(FamilyInstance fam, ElectricalSystemType elsyst_typ)  // Найти коннектор нужного типа у элемента   
        //{
        //    Connector conisk = null;           

        //    try
        //    {
        //        List<Connector> SpConEl = naiti_SP_con_u_El(fam, 1 );

        //        if (SpConEl.Count == 0)
        //        {
        //            return conisk;
        //        }

        //        foreach (var con in SpConEl)
        //        {
        //            try
        //            {
        //                ElectricalSystem els = Naiti_Syst_El_u_Con(con);

        //                if (els == null)
        //                {
        //                    foreach (var syst in D_SPelectricalSystemType)
        //                    {

        //                        if (con.ElectricalSystemType.ToString() == syst.ToString())
        //                        {
        //                            conisk = con;
        //                            return conisk;
        //                        }
        //                    }
        //                }
        //            }
        //            catch { }
        //        }
        //    }
        //    catch { }


        //    return conisk;

        //}


        public static Connector naiti_con(FamilyInstance fam, ElectricalSystemType elsyst_typ)  // Найти коннектор нужного типа у элемента   
        {
            Connector conisk = null;

            try
            {
                List<Connector> SpConEl = naiti_SP_con_u_El(fam, 1);

                if (SpConEl.Count == 0)
                {
                    return conisk;
                }

                foreach (var con in SpConEl)
                {
                    try
                    {
                        var type = con.ElectricalSystemType;

                        if (type.ToString() == elsyst_typ.ToString())
                        {

                            conisk = con;
                            return conisk;

                        }
                    }
                    catch { }
                }
            }
            catch { }

            return conisk;

        }


        /// /////////////////////////////////////////////////////////////////////////////

        public static List<FamilyInstance> dopolnSP(List<int> SpInt)  // Чистка от нулевых значений
        {
            List<FamilyInstance> SPrez = new List<FamilyInstance>();

            try
            {
                SPrez = SpInt
               .Select(x => dopoln(x)) // Преобразуем число в FamilyInstance
               .Where(familyInstance => familyInstance != null) // Фильтруем только ненулевые значения
               .ToList(); // Преобразуем в список

            }

            catch { }

            return SPrez;
        }


        public static Connector naiti_con_chern(int nom, string naiti_syst)
        {
            Connector conisk = null;

            FamilyInstance el_chit = doc.GetElement(new ElementId(nom)) as FamilyInstance;

            if (el_chit == null)
            {
                return conisk;
            }

            var sp_cons = el_chit.MEPModel.ConnectorManager.Connectors;

            if (sp_cons == null)
            {
                return conisk;
            }

            foreach (var con in sp_cons)
            {
                try
                {
                    var nazv1 = (con as Connector).ElectricalSystemType.ToString();

                    if (nazv1 == naiti_syst)
                    {
                        conisk = (con as Connector);
                        break;
                    }
                }
                catch { }
            }

            return conisk;

        }

        public static FamilyInstance dopoln(int nom) // Элемент по номеру  
        {
            FamilyInstance rez = null;

            try
            {
                rez = doc.GetElement(new ElementId(nom)) as FamilyInstance;
            }

            catch { }

            return rez;
        }

        ////////////////////////////////////////////////////////////////////////////////////////

        public static void Otkl_El(FamilyInstance fam)  // Отключение элемента от всех систем  
        {

            try
            {
                var sp_cons = fam.MEPModel.ElectricalSystems;

                List<ElectricalSystem> SpElSyst = new List<ElectricalSystem>();

                foreach (var syst1 in sp_cons)
                {
                    ElectricalSystem syst = syst1 as ElectricalSystem;

                    if (syst != null)
                    {
                        SpElSyst.Add(syst);
                    }
                }


                ICollection<ElementId> elementIds = new List<ElementId> { fam.Id };



                foreach (var syst in SpElSyst)
                {
                    try
                    {

                        using (Transaction tx = new Transaction(doc, "Отключение коннектора от цепи"))
                        {
                            tx.Start();

                            doc.Delete(syst.Id);

                            tx.Commit();
                        }

                    }
                    catch { }
                }
            }
            catch { }
        }

        public static void DisconnectConnector(Connector connector) // отключение коннектора    
        {

            ElectricalSystem electricalSystem = null;

            try
            {
                electricalSystem = connector.MEPSystem as ElectricalSystem;
            }
            catch { }

            if (electricalSystem == null)
            {
                return;
            }

            using (Transaction tx = new Transaction(doc, "Отключение коннектора от цепи"))
            {
                tx.Start();

                try
                {
                    ConnectorSet conset = new ConnectorSet();

                    conset.Insert(connector);

                    // Отключить коннектор от электрической цепи
                    electricalSystem.Remove(conset);

                }
                catch
                {

                }
                tx.Commit();
            }
        }

        public static List<Element> getElements(Selection sel) // Список элементов из выбраных   
        {
            List<Element> selectedElements = new List<Element>();

            List<ElementId> ids = sel.GetElementIds().ToList();

            foreach (ElementId id in ids)
            {
                selectedElements.Add(doc.GetElement(id));
            }

            return selectedElements;
        }

        public static ElectricalSystem Naiti_Syst_El_u_Con(Connector connach)  // найти цепь у коннектора  
        {

            ElectricalSystem els_isk = null;

            try
            {
                var cccset = connach.AllRefs;

                foreach (var cc in cccset)
                {
                    Connector connector = cc as Connector;

                    ElectricalSystem els1 = null;

                    try
                    {
                        els1 = connector.Owner as ElectricalSystem;
                    }

                    catch { }

                    if (els1 != null)
                    {
                        els_isk = els1;

                        break;

                    }
                }
            }
            catch { }

            return els_isk;

        }

        public static List<Connector> Naiti_Con_u_Syst_El(ElectricalSystem el_syst, FamilyInstance fam) // создать список коннекторов нужной цепи у элемента
        {
            List<Connector> Con_iskSP = null;

            try
            {
                Con_iskSP = Sozdat_Slovar_ElectricalSystem(fam)[el_syst.Id.IntegerValue.ToString()];

            }
            catch { }

            return Con_iskSP;

        }

        /// ///////////////////////////////////////////////////////////////////////////////////////////

        public static void Otkluch_Ot_cepi(List<FamilyInstance> Spfam, ElectricalSystem el_syst)  // отключение от цепи списка элементов  
        {

            int count_vse_con_cep = 0;

            int count_vse_con_cep_potr = 0;

            foreach (Connector connector in el_syst.ConnectorManager.Connectors)
            {
                count_vse_con_cep++;
            }

            if (count_vse_con_cep > 1)
            {
                ConnectorSet connectorSet = new ConnectorSet();

                foreach (FamilyInstance fam in Spfam)
                {
                    List<Connector> Con_sp = new List<Connector>();

                    try
                    {
                        Con_sp = Naiti_Con_u_Syst_El(el_syst, fam);
                    }
                    catch { }

                    if (Con_sp.Count > 0)
                    {
                        count_vse_con_cep_potr = Con_sp.Count;

                        foreach (Connector con in Con_sp)
                        {
                            connectorSet.Insert(con);
                        }
                    }
                }

                if (count_vse_con_cep_potr < count_vse_con_cep - 1)
                {
                    try
                    {
                        using (Transaction tx = new Transaction(doc, "Отключение коннекторов о цепи"))
                        {
                            tx.Start();

                            el_syst.Remove(connectorSet);

                            tx.Commit();
                        }
                    }
                    catch { }
                }

                else
                {
                    try
                    {
                        using (Transaction tx = new Transaction(doc, "Удаление цепи"))
                        {
                            tx.Start();

                            doc.Delete(el_syst.Id);

                            tx.Commit();
                        }
                    }
                    catch { }
                }
            }
        }

        public static List<Connector> naiti_SP_con_u_El(FamilyInstance fam, int flag_podkl) // список коннекторов по флагу  
        {

            // flag_podkl  0 значит все коннекторы  1 значит коннекторы свободные 2 значит коннекторы подключенные

            List<Connector> SpConEl = new List<Connector>();

            try
            {
                var sp_cons = fam.MEPModel.ConnectorManager.Connectors;

                if (sp_cons == null)
                {
                    return SpConEl;
                }

                foreach (var con1 in sp_cons)
                {
                    Connector con = con1 as Connector;

                    if (flag_podkl == 0)
                    {
                        SpConEl.Add(con);
                    }

                    else
                    {

                        ElectricalSystem els = Naiti_Syst_El_u_Con(con);

                        if (flag_podkl == 1)
                        {

                            if (els == null)
                            {
                                SpConEl.Add(con);
                            }
                        }

                        if (flag_podkl == 2)
                        {

                            if (els != null)
                            {
                                SpConEl.Add(con);
                            }
                        }
                    }
                }
            }
            catch { }

            return SpConEl;
        }

        public static Dictionary<string, List<Connector>> Sozdat_Slovar_ElectricalSystem(FamilyInstance fam) // словарь цепь и список коннекторов у элемента   
        {
            // Инициализация словаря
            Dictionary<string, List<Connector>> slovar = new Dictionary<string, List<Connector>>();

            // Получаем список всех коннекторов у переданного семейства
            List<Connector> connectors = naiti_SP_con_u_El(fam, 0);

            List<(Connector, ElectricalSystem)> sp_con_mass = new List<(Connector, ElectricalSystem)>();

            if (connectors.Count > 0)
            {
                try
                {
                    // Перебираем каждый коннектор
                    foreach (Connector connector in connectors)
                    {
                        // Находим электрическую систему у текущего коннектора
                        ElectricalSystem electricalSystem = Naiti_Syst_El_u_Con(connector);

                        if (electricalSystem != null)
                        {
                            sp_con_mass.Add((connector, electricalSystem));

                        }

                    }

                    if (sp_con_mass.Count > 0)
                    {

                        slovar = sp_con_mass
                        .GroupBy(item => item.Item2.Id.IntegerValue.ToString()) // Группируем по Id ElectricalSystem
                        .ToDictionary(
                             group => group.Key, // Ключ — строковое представление IntegerValue Id
                             group => group.Select(item => item.Item1).ToList() // Значение — это список Connector
                         );
                    }
                }

                catch { }
            }

            if (slovar.Count == 0)
            {
                slovar = null;
            }

            // Возвращаем итоговый словарь
            return slovar;
        }

        public static void Udal_Cep_Vmeste_s_docernimi(ElectricalSystem el_syst)  // Удалить цепь с дочерними 
        {

        }

        public static List<ElectricalSystem> GetAllDependentSystems(ElectricalSystem el_syst) // Узнать родительскую цепь   
        {
            // Лист для хранения всех связанных цепей.
            List<ElectricalSystem> dependentSystems = new List<ElectricalSystem>();

            // Множество Id для отслеживания обработанных цепей и предотвращения повторной обработки.
            HashSet<int> processedSystemIds = new HashSet<int>();

            // Вспомогательная функция для рекурсивного поиска зависимых цепей.
            void FindDependentSystems(ElectricalSystem system)
            {
                // Определяем панель, питающую данную цепь (BaseEquipment).
                Element baseEquipment = system.BaseEquipment;

                // Если нет панели, то выходим (это конец цепочки).
                if (baseEquipment == null) return;

                // Найти все цепи, подключенные к этой панели.
                var connectedSystemsCollector = new FilteredElementCollector(doc)
                    .OfClass(typeof(ElectricalSystem))
                    .Cast<ElectricalSystem>()
                    .Where(s => s.BaseEquipment?.Id == baseEquipment.Id);

                // Обрабатываем каждую найденную цепь.
                foreach (var dependentSystem in connectedSystemsCollector)
                {
                    // Получаем уникальный идентификатор цепи.
                    int systemId = dependentSystem.Id.IntegerValue;

                    // Проверяем, не добавлена ли цепь уже в список с помощью HashSet (по Id).
                    if (!processedSystemIds.Contains(systemId))
                    {
                        // Добавляем цепь в список и в HashSet.
                        dependentSystems.Add(dependentSystem);
                        processedSystemIds.Add(systemId);

                        // Рекурсивно ищем зависимые цепи для найденной цепи.
                        FindDependentSystems(dependentSystem);
                    }
                }
            }

            // Начинаем поиск зависимых цепей с исходной цепи.
            FindDependentSystems(el_syst);

            // Возвращаем полный список всех зависимых цепей.
            return dependentSystems;
        }

        public static List<ElectricalSystem> VseCepi() // Все цепи  
        {
            return new FilteredElementCollector(doc)
                    .OfClass(typeof(ElectricalSystem))
                    .Cast<ElectricalSystem>().ToList();
        }

        public static List<ElSet> Naiti_chaild_Cep(ElSet ElSet_nach, List<ElSet> Vse_Els) // Найти дочерние цепи  
        {
            List<ElSet> ChildeSp = new List<ElSet>();

            try
            {
                if (ElSet_nach.ListPotr.Count > 0)
                {
                    foreach (var potr in ElSet_nach.ListPotr)
                    {
                        foreach (ElSet el_cep in Vse_Els)
                        {
                            if (el_cep.BaseEq.Id.IntegerValue == potr.Id.IntegerValue)
                            {
                                ChildeSp.Add(el_cep);
                            }
                        }
                    }
                }
            }
            catch { }

            return ChildeSp;
        }

        public static List<ElSet> FindAllChildren(ElSet parent, List<ElSet> allSets)
        {
            // Список для хранения всех дочерних элементов
            List<ElSet> children = new List<ElSet>();

            try
            {
                // Находим дочерние элементы для текущего родителя
                List<ElSet> currentChildren = Naiti_chaild_Cep(parent, allSets);

                // Если найдены дочерние элементы
                if (currentChildren.Any())
                {
                    // Добавляем в общий список
                    children.AddRange(currentChildren);

                    // Для каждого найденного дочернего элемента запускаем рекурсию
                    foreach (var child in currentChildren)
                    {
                        children.AddRange(FindAllChildren(child, allSets));
                    }
                }
            }
            catch
            {
                // Обрабатываем любые исключения, если они появляются
            }

            return children;
        }

        public static List<ElSet> FindAllChildren_itog(ElSet parent, List<ElSet> allSets)
        {

            List<ElSet> elSets = new List<ElSet>();

            try
            {

                elSets = FindAllChildren(parent, allSets);

                elSets.Add(parent);
            }

            catch { }


            return elSets;
        }

        public static List<ElSet> GetConnectedSystems(ElectricalSystem el_syst, List<ElSet> vseEls)
        {
            // Хранение уже обработанных цепей
            HashSet<int> processedSystems = new HashSet<int>();

            // Итоговый список зависимых цепей
            List<ElSet> resultChain = new List<ElSet>();

            // Очередь на обработку цепей
            Queue<ElSet> systemsToProcess = new Queue<ElSet>();

            // Ищем стартовую цепь
            ElSet startElSet = vseEls.FirstOrDefault(x => x.el_syst.Id.IntegerValue == el_syst.Id.IntegerValue);

            if (startElSet == null)
            {
                throw new Exception("Стартовая цепь не найдена!");
            }

            systemsToProcess.Enqueue(startElSet);
            processedSystems.Add(startElSet.el_syst.Id.IntegerValue);

            // Пока есть цепи в очереди
            while (systemsToProcess.Count > 0)
            {
                ElSet currentElSet = systemsToProcess.Dequeue();

                // Добавляем текущую цепь в результирующий список
                resultChain.Add(currentElSet);

                // === Поиск дочерних цепей (зависимых потребителей) ===
                foreach (var potr in currentElSet.ListPotr)
                {
                    ElSet childChain = vseEls.FirstOrDefault(x => x.BaseEq != null && x.BaseEq.Id.IntegerValue == potr.Id.IntegerValue);
                    if (childChain != null && !processedSystems.Contains(childChain.el_syst.Id.IntegerValue))
                    {
                        systemsToProcess.Enqueue(childChain);
                        processedSystems.Add(childChain.el_syst.Id.IntegerValue);
                    }
                }

                // === Поиск родительских цепей (чья панель является потребителем текущей) ===
                if (currentElSet.BaseEq != null)
                {
                    ElSet parentChain = vseEls.FirstOrDefault(x => x.ListPotr.Any(p => p.Id.IntegerValue == currentElSet.BaseEq.Id.IntegerValue));
                    if (parentChain != null && !processedSystems.Contains(parentChain.el_syst.Id.IntegerValue))
                    {
                        systemsToProcess.Enqueue(parentChain);
                        processedSystems.Add(parentChain.el_syst.Id.IntegerValue);
                    }
                }
            }

            return resultChain;
        }

        public static List<ElectricalSystem> Poisk_Cep_Pomarkeru(string Marker)
        {
            List<ElectricalSystem> sp_isk = new List<ElectricalSystem>();

            foreach (var cep in D_VseElectricalSystem)
            {
                string cep_mark = D_Pust_Znach;

                string naim = D_Pust_Znach;

                try
                {
                    cep_mark = ReadDataFromElement_posp(cep as Element, MetaData_for_Cep_Dan);

                    MarkerCep markerCep = new MarkerCep(cep_mark);

                    naim = markerCep.Naim;
                }
                catch { }

                if ((naim != D_Pust_Znach) && (!string.IsNullOrEmpty(naim)))
                {
                    if (naim == Marker)
                    {
                        sp_isk.Add(cep);
                    }
                }
            }

            return sp_isk;
        }

        public static MarkerCep UdalCepiPoMarkeru(string Marker_Cepi) // Спорная штука. Тут имеется ввиду что удаляем старые сети с тем же маркером. Но перед удалением записываем ее данные
        {
            MarkerCep markc_nach = new MarkerCep();

            if (Marker_Cepi != D_Pust_Znach)
            {
                List<ElectricalSystem> electricalSystems = Poisk_Cep_Pomarkeru(Marker_Cepi);

                if (electricalSystems.Count > 0)
                {
                    markc_nach = new MarkerCep(electricalSystems[0]);

                }

                using (Transaction tx = new Transaction(doc, "Create Fire Alarm Circuit"))
                {
                    tx.Start();

                    foreach (var childe in electricalSystems)
                    {
                        doc.Delete(childe.Id);
                    }

                    tx.Commit();
                }
            }

            return markc_nach;

        }

        public static int naity_nom_v_cep(Connector con)

        {
            string Razd_Glob = D_Razd_Glob;

            int rez = 0;

            try
            {
                FamilyInstance fam = con.Owner as FamilyInstance;

                string dan = D_Pust_Znach;

                int idcon = con.Id;

                try
                {
                    dan = ReadDataFromElement_posp(fam as Element, MetaData_for_Elem_NomCEP);
                }
                catch { }

                if ((dan != D_Pust_Znach) && (!string.IsNullOrEmpty(dan)))
                {
                    try
                    {
                        List<string> Sp_opis = dan.Split(new[] { Razd_Glob }, StringSplitOptions.None).ToList();

                        foreach (string s in Sp_opis)
                        {
                            OpisanCon opcon = new OpisanCon(s);

                            if (opcon.Id_Con == idcon)
                            {
                                rez = opcon.NomV_Cepi; break;
                            }
                        }
                    }
                    catch { }
                }
            }

            catch { }

            return rez;
        }

        public static void zapis_nom_con_v_Fam(Connector con, int nomer_prisv)
        {
            string Razd_Glob = D_Razd_Glob;

            int flaag = 0;

            string rez_na_prisv = D_Pust_Znach;

            FamilyInstance fam = con.Owner as FamilyInstance;

            string dan = D_Pust_Znach;

            int idcon = con.Id;

            OpisanCon opisanCon = new OpisanCon();

            opisanCon.Id_Con = idcon;

            opisanCon.NomV_Cepi = nomer_prisv;

            opisanCon.Zapakovka();

            string str_ops = opisanCon.Stroca_Itog_Con;

            try
            {
                dan = ReadDataFromElement_posp(fam as Element, MetaData_for_Elem_NomCEP);  // MetaData_for_Elem_NomCEP              

            }
            catch { }


            if ((dan != D_Pust_Znach) && (!string.IsNullOrEmpty(dan)))
            {
                try
                {
                    List<string> Sp_opis = dan.Split(new[] { Razd_Glob }, StringSplitOptions.None).ToList();

                    List<string> Sp_opis1 = new List<string>();

                    foreach (string s in Sp_opis)
                    {
                        OpisanCon opcon = new OpisanCon(s);

                        if (opcon.Id_Con == idcon)
                        {
                            if (opcon.NomV_Cepi != opisanCon.NomV_Cepi)
                            {
                                Sp_opis1.Add(str_ops);
                                flaag = 1;
                            }


                        }

                        else
                        {
                            Sp_opis1.Add(s);
                        }
                    }

                    if (flaag == 0)
                    {
                        Sp_opis1.Add(str_ops);

                        flaag = 1;
                    }

                    rez_na_prisv = string.Join(Razd_Glob, Sp_opis1);

                }
                catch { }
            }

            else
            {
                flaag = 1;

                rez_na_prisv = str_ops;
            }

            if (flaag == 1)
            {
                SchemaSetData_univers_posp(fam as Element, MetaData_for_Elem_NomCEP, rez_na_prisv); //MetaData_for_Elem_NomCEP
            }
        }

        public static string FormatNumber(int number, int minDigits)
        {
            // Формируем строку формата с минимальным количеством цифр
            string format = "D" + minDigits;

            // Возвращаем число, отформатированное с учетом разряда
            return number.ToString(format);
        }

        public static string ProcessAndCombine(string input, string delimiter)
        {
            // Разделение строки на сегменты
            List<string> segments = input.Split(new[] { delimiter }, StringSplitOptions.None).ToList();

            // Удаление пробелов в начале и конце каждого элемента
            var trimmedItems = segments.Select(item => item.Trim());

            // Удаление дубликатов и сортировка
            var uniqueSortedList = trimmedItems
                .Distinct() // Убираем дубликаты
                .OrderBy(x => x) // Сортируем в алфавитном порядке
                .ToList(); // Преобразуем обратно в список

            // Объединение элементов списка без разделителя
            return string.Concat(uniqueSortedList);
        }

        public static List<FamilyInstance> GetSortedSpPotr(List<FamilyInstance> spPotr, FamilyInstance pan)
        {
            // Проверка входных параметров
            if (spPotr == null || spPotr.Count == 0)
            {
                throw new ArgumentNullException(nameof(spPotr), "Список Sp_Potr не может быть пустым или null");
            }

            if (pan == null)
            {
                throw new ArgumentNullException(nameof(pan), "Панель Pan не может быть null");
            }

            // Получение точки расположения панели
            LocationPoint panLocation = pan.Location as LocationPoint;
            if (panLocation == null)
            {
                throw new InvalidOperationException("Панель должна иметь расположение типа LocationPoint");
            }

            XYZ panPoint = panLocation.Point;

            // Проверяем, что все элементы Sp_Potr имеют LocationPoint
            foreach (var familyInstance in spPotr)
            {
                if (!(familyInstance.Location is LocationPoint))
                {
                    throw new InvalidOperationException($"Элемент семейства ID: {familyInstance.Id} не имеет точки расположения (LocationPoint).");
                }
            }

            // Сортировка Sp_Potr на основе расстояния до панели
            var sortedSpPotr = spPotr
                .OrderBy(fi =>
                {
                    // Получение точки расположения каждого семейства
                    LocationPoint fiLocation = fi.Location as LocationPoint;
                    return fiLocation.Point.DistanceTo(panPoint);
                })
                .ToList();

            return sortedSpPotr;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Chistka_Cepi(List<FamilyInstance> Sp_Potr, ElSet ElSet_nach)
        {
            int fl = 0;

            List<FamilyInstance> ListPotr1 = ElSet_nach.ListPotr;

            FamilyInstance firstElement = Sp_Potr[0];

            if (ListPotr1.Any(potr => potr.Id.IntegerValue == firstElement.Id.IntegerValue))  // Выяснили есть ли такой элемент в цепи.
            {
                fl = 1;
            }

            if (fl == 0)
            {

                ElectricalSystemType elsysttyp = D_revitSystemsDict[D_Typ_System];

                Connector con = naiti_con(firstElement, elsysttyp);

                ConnectorSet connectorSet = new ConnectorSet();

                connectorSet.Insert(con);

                using (Transaction tx = new Transaction(doc, "Подключаем первый элемент"))
                {
                    tx.Start();

                    ElSet_nach.el_syst.Add(connectorSet);

                    tx.Commit();
                }

            }


            ConnectorSet SpNaotkluchenie = new ConnectorSet();

            ICollection<ElementId> SpelementIdsNaotkluchenie = new List<ElementId>();

            ConnectorSet conectorSet_prom1 = ElSet_nach.el_syst.ConnectorManager.Connectors;

            foreach (Connector con in conectorSet_prom1)
            {
                El_Con elcon = new El_Con(con);

                FamilyInstance hos1 = elcon.famPriemn_Host;

                if ((hos1.Id.IntegerValue != ElSet_nach.BaseEq.Id.IntegerValue) && (hos1.Id.IntegerValue != firstElement.Id.IntegerValue))
                {
                    SpelementIdsNaotkluchenie.Add(hos1.Id);
                }
            }

            if (SpelementIdsNaotkluchenie.Count > 0)
            {
                using (Transaction tx = new Transaction(doc, "Удалить элементы из цепи"))
                {
                    tx.Start();

                    ElSet_nach.el_syst.Remove(SpelementIdsNaotkluchenie);

                    tx.Commit();
                }
            }

            if (Sp_Potr.Count > 1)
            {
                ConnectorSet conectorSet1 = new ConnectorSet();

                for (int i = 1; i < Sp_Potr.Count; i++)
                {

                    ElectricalSystemType elsysttyp = D_revitSystemsDict[D_Typ_System];

                    Connector con = naiti_con(Sp_Potr[i], elsysttyp);

                    if (con != null)
                    {
                        conectorSet1.Insert(con);
                    }
                }

                if (!conectorSet1.IsEmpty)
                {
                    using (Transaction tx = new Transaction(doc, "Добавить остальные элементы цепи"))
                    {
                        tx.Start();

                        ElSet_nach.el_syst.Add(conectorSet1);

                        tx.Commit();
                    }
                }
            }

            ElSet_nach = new ElSet(ElSet_nach.el_syst);

            List<FamilyInstance> Sp_Potr1 = ElSet_nach.ListPotr;

            int naznachit_nom = 1;

            for (int i = 0; i < Sp_Potr1.Count; i++)
            {
                foreach (Connector con in ElSet_nach.SpCon)
                {
                    if (con.Owner.Id.IntegerValue == Sp_Potr1[i].Id.IntegerValue)
                    {
                        zapis_nom_con_v_Fam(con, naznachit_nom);
                        naznachit_nom++;
                        break;
                    }
                }
            }

            string strid = string.Empty;

            try
            {
                strid = ElSet_nach.el_syst.Id.ToString();
            }
            catch { }

            if (string.IsNullOrEmpty(strid) || (strid == ""))
            {
                return;
            }

            Chistka_Grafa_ot_Id_STR( new List<string> { strid });            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Udalene_Cep  (List<ElectricalSystem> Sp_Ceps)
        {
            if (Sp_Ceps == null)
            {
                return;            
            }

            if (Sp_Ceps.Count == 0)
            {
                return;
            }

            List<string> stringssp_ids = new List<string>();

            List<ElementId> SpElId = new List<ElementId>();

            foreach (var els in Sp_Ceps)
            {
                string strid = string.Empty;
                ElementId elid = null;

                try 
                {
                    strid = els.Id.IntegerValue.ToString();
                    elid = els.Id;
                }
                catch { }

                if (!string.IsNullOrEmpty(strid)) 
                {
                    stringssp_ids.Add(strid);  
                }

                if (elid != null)
                {
                    SpElId.Add(elid);
                }               
            }

            if (SpElId.Count > 0)
            {

                using (Transaction tx = new Transaction(doc, "Удаление цепей"))
                {
                    tx.Start();

                        doc.Delete(SpElId);

                    tx.Commit();
                }
            }

            if (stringssp_ids.Count > 0)
            {
                Chistka_Grafa_ot_Id_STR(stringssp_ids);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Vse_Prov_and_Stoyak()
        {
            D_All_Provod_and_Stoyak = new List<FamilyInstance>();

            FilteredElementCollector allElementsDoc1 = new FilteredElementCollector(doc);

            // Фильтруем только FamilyInstance
            List<FamilyInstance> SpCab = allElementsDoc1
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(fi => fi.Symbol.Family.Name == D_nazv_sem_Provoda).ToList();

            FilteredElementCollector allElementsDoc2 = new FilteredElementCollector(doc);

            List<FamilyInstance> SpSt = allElementsDoc2
               .OfClass(typeof(FamilyInstance))
               .Cast<FamilyInstance>()
               .Where(fi => fi.Symbol.Family.Name == D_NazvSem_Sroyak).ToList();

            List<FamilyInstance> combinedList = new List<FamilyInstance>();

            if ((SpCab == null) && (SpSt != null))
            {
                combinedList = SpSt;
            }

            if ((SpCab != null) && (SpSt == null))
            {
                combinedList = SpCab;
            }

            if ((SpCab != null) && (SpSt != null))
            {
                combinedList = SpCab.Concat(SpSt).ToList();
            }

            D_All_Provod_and_Stoyak = combinedList;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Chistka_Modely_ot_nesuch_grupp()  // Чистка модели от удаленных групп
        {
            Vse_Prov_and_Stoyak();

            if (D_All_Provod_and_Stoyak.Count == 0)
            { return; }

            List<Graf_Infa_Cep> SP_k_prisv = new List<Graf_Infa_Cep>();
            
            List<Graf_Infa_Cep> Sp_bez_gr = new List<Graf_Infa_Cep>();

            foreach (FamilyInstance fam in D_All_Provod_and_Stoyak)
            {               

                Graf_Infa_Cep graf_Infa_Cep = new Graf_Infa_Cep(fam);
                graf_Infa_Cep.nayty_Stroca_Itog_Graf_Infa_v_Param();

                if (graf_Infa_Cep.Flag_na_redaktir == 1)
                {
                    graf_Infa_Cep.nayty_Stroca_Itog_Graf_Infa_v_Param();

                    SP_k_prisv.Add(graf_Infa_Cep);
                }

                if (string.IsNullOrEmpty(graf_Infa_Cep.Stroca_Itog_Graf_Infa_Glob))
                {
                    if (!string.IsNullOrEmpty(graf_Infa_Cep.Znach_par) && !(graf_Infa_Cep.Znach_par == ""))
                    {
                        Sp_bez_gr.Add(graf_Infa_Cep);
                    }
                
                }
            }

            if (SP_k_prisv.Count != 0)
            {
                using (Transaction tx = new Transaction(doc, "Удаление информации о цепях из графа"))
                {
                    tx.Start();

                    foreach (Graf_Infa_Cep graf_Infa_Cep in SP_k_prisv)
                    {
                        graf_Infa_Cep.redactir_spisvoen1(1);
                    }

                    tx.Commit();
                }
            }

            if (Sp_bez_gr.Count != 0)
            {
                using (Transaction tx = new Transaction(doc, "Удаление значений параметров у проводов без внутреннего присвоения"))
                {
                    tx.Start();

                    foreach (Graf_Infa_Cep graf_Infa_Cep in Sp_bez_gr)
                    {
                        graf_Infa_Cep.famHost.LookupParameter(graf_Infa_Cep.Naim_Par).Set(string.Empty); 
                    }

                    tx.Commit();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Chistka_Grafa_ot_Id_STR(List <string> SPidstr)
        {        

            if (SPidstr == null)
            { return; }

            if (SPidstr.Count == 0 )
            { return; }

            Vse_Prov_and_Stoyak();

            if (D_All_Provod_and_Stoyak.Count == 0)
            { return; }

            //FilteredElementCollector allElementsDoc1 = new FilteredElementCollector(doc);

            //// Фильтруем только FamilyInstance
            //List<FamilyInstance> SpCab = allElementsDoc1
            //    .OfClass(typeof(FamilyInstance))
            //    .Cast<FamilyInstance>()
            //    .Where(fi => fi.Symbol.Family.Name == D_nazv_sem_Provoda).ToList();

            //FilteredElementCollector allElementsDoc2 = new FilteredElementCollector(doc);

            //List<FamilyInstance> SpSt = allElementsDoc2
            //   .OfClass(typeof(FamilyInstance))
            //   .Cast<FamilyInstance>()
            //   .Where(fi => fi.Symbol.Family.Name == D_NazvSem_Sroyak).ToList();

            //List<FamilyInstance> combinedList = new List<FamilyInstance>();

            //if ((SpCab == null) && (SpSt != null))
            //{
            //    combinedList = SpSt;
            //}

            //if ((SpCab != null) && (SpSt == null))
            //{
            //    combinedList = SpCab;
            //}

            //if ((SpCab != null) && (SpSt != null))
            //{
            //    combinedList = SpCab.Concat(SpSt).ToList();
            //}

            //if (combinedList.Count == 0)
            //{
            //    return;
            //}            

            Dictionary<string, List<FamilyInstance>> Dict_po_str_na_udal = new Dictionary<string, List<FamilyInstance>>();           

            foreach (string idstr in SPidstr)
            {
                List<FamilyInstance> combinedList11 = new List<FamilyInstance>();

                foreach (FamilyInstance ff in D_All_Provod_and_Stoyak)
                {
                    string str = ReadDataFromElement_posp(ff, MetaData_for_ElGraff_Data);

                    if (string.IsNullOrEmpty(str) || (str == ""))
                    {
                        continue;
                    }

                    if (str.Contains(idstr))
                    {
                        combinedList11.Add(ff);
                    }
                }

                if (combinedList11.Count != 0)
                {
                    Dict_po_str_na_udal.Add(idstr, combinedList11);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////

            if (Dict_po_str_na_udal.Count == 0)
            {
                return;
            }

            foreach (var dic in Dict_po_str_na_udal)
            {
                string key = dic.Key;

                using (Transaction tx = new Transaction(doc, "Удаление информации о цепях из графа"))
                {
                    tx.Start();

                    foreach (FamilyInstance fam in dic.Value)
                    {
                        try
                        {

                            Graf_Infa_Cep graf_Infa_Cep = new Graf_Infa_Cep(fam);

                            graf_Infa_Cep.Udalenie(1, key);
                        }
                        catch { }
                    }

                    tx.Commit();
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////       

        public static void Prisvoene_Nomerov_v_Element()
        {
            if (D_SpFam_na_Numerac_Dla_Potr.Count == 0)
            {
                return;
            }

            List<string> SP_Nom = D_zagatovka_Numerac_Potr_dla_prisv.uznat_znach_Stroca_Itog_V_Elem();

            using (Transaction t = new Transaction(doc, "Назначение марке группы"))
            {
                t.Start();

                for (int i = 0; i < D_SpFam_na_Numerac_Dla_Potr.Count; i++)
                {
                    try
                    {
                        string strnom = SP_Nom[i];

                        D_SpFam_na_Numerac_Dla_Potr[i].LookupParameter(D_Ima_Poryadk_Nomer).Set(strnom);
                    }
                    catch { }
                }

                t.Commit();
            }
        }

        public static void Prisvoene_Nomerov_v_Cepy()
        {

            if (D_SpEls_na_Numerac_Dla_Els.Count == 0)
            {
                return;
            }

            List<string> SP_Nom = D_zagatovka_Numerac_Cepey_dla_prisv.uznat_znach_Stroca_Itog_V_Cepach();

            using (Transaction t = new Transaction(doc, "Назначение марке группы"))
            {
                t.Start();

                for (int i = 0; i < D_SpEls_na_Numerac_Dla_Els.Count; i++)
                {
                    try
                    {

                        string strnom = SP_Nom[i];

                        D_SpEls_na_Numerac_Dla_Els[i].LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).Set(strnom);
                    }
                    catch { }
                }

                t.Commit();
            }

            using (Transaction t = new Transaction(doc, "Переименование групп в графе"))
            {
                t.Start();

                foreach (var el_syst in D_SpEls_na_Numerac_Dla_Els)
                {
                    ElSet elgr = new ElSet(el_syst);

                    List<Provod_Graff> sppr = elgr.SPprovod_Graffs;



                    if (sppr.Count > 0)
                    {
                        foreach (var prov in sppr)
                        {
                            Graf_Infa_Cep raf_Infa_Cep = new Graf_Infa_Cep(prov.Pr_family_ins, el_syst);

                            raf_Infa_Cep.redactir_spisvoen(1);
                        }
                    }

                }
                t.Commit();
            }
        }
    }
    
}
