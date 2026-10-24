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
using System.ComponentModel;

namespace LKS_DEC
{
    public class DopFunc_Postr_graf
    {

        public static List<Provod_Graff> GetVisibleFamilyInstances_Provoda_Na_Vide(List<FamilyInstance> SpPr, List<Potrebit_Graff> SpPotrebitGr, List<Stoyak> Spst)
        {
            List<Provod_Graff> vse_provoda = new List<Provod_Graff>();

            try
            {

                // Проверяем каждый элемент на видимость в текущем виде.
                foreach (var instance in SpPr)
                {
                    try
                    {
                        vse_provoda.Add(new Provod_Graff(instance, SpPotrebitGr, Spst));
                    }
                    catch { }
                }
            }

            catch { }

            return vse_provoda;
        }

        public static List<Potrebit_Graff> GetVisibleFamilyInstances_Potrebit_na_Vide(ViewPlan viewPlan)
        {
            List<Potrebit_Graff> vse_fam_Potrebit_na_Vide = new List<Potrebit_Graff>();
            List<FamilyInstance> SpPotr = new List<FamilyInstance>();

            try
            {
                // Собираем все элементы на активном виде
                FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, viewPlan.Id);

                // Фильтруем только FamilyInstance
                SpPotr = allElementsInView
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(fi => fi.Category.Id.IntegerValue == (int)BuiltInCategory.OST_ElectricalEquipment).ToList();


                if (SpPotr.Count > 0)
                {

                    List<FamilyInstance> SpPotrFiltered = SpPotr
                                            .Where(fi =>
                                            {
                                                // Получить коннекторы FamilyInstance

                                                ConnectorSet connectors = (fi.MEPModel as ElectricalEquipment)?.ConnectorManager?.Connectors;

                                                // Проверить, есть ли коннекторы и не пустой ли это список
                                                return connectors != null && connectors.Size > 0; // Если разъемы существуют
                                            })
                                            .ToList();
                    if (SpPotrFiltered.Count != 0)
                    {


                        foreach (var fam in SpPotrFiltered)
                        {
                            Potrebit_Graff pr_gr = new Potrebit_Graff(fam);

                            if (pr_gr.Ust_Potr != null)
                            {
                                vse_fam_Potrebit_na_Vide.Add(pr_gr);
                            }
                        }
                    }


                }
            }

            catch { }

            return vse_fam_Potrebit_na_Vide;

        }

        public static List<Stoyak> GetVisibleFamilyInstances_Stoyak_na_Vide(ViewPlan viewPlan)
        {
            List<Stoyak> SpSt = new List<Stoyak>();

            if (D_Vse_StoYak == null)
            {
                NaytiVseStoyaki();
            }


            if (D_Vse_StoYak.Count == 0)
            {
                NaytiVseStoyaki();
            }


            if (D_Vse_StoYak == null)
            {
                return SpSt;
            }

            if (D_Vse_StoYak.Count == 0)
            {
                return SpSt;
            }

            foreach (var st in D_Vse_StoYak)
            {
                if (st.RoditViewPlan.Id.IntegerValue == viewPlan.Id.IntegerValue)
                {
                    SpSt.Add(st);
                }

            }

            return SpSt;
        }



        //public static void GetVisibleFamilyInstances_Provoda()
        //{
        //    D_vse_provoda = new List<Provod_Graff>();

        //    List<FamilyInstance> SpPr = new List<FamilyInstance>();

        //    try
        //    {
        //        // Собираем все элементы на активном виде
        //        FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id);

        //        // Фильтруем только FamilyInstance
        //        SpPr = allElementsInView
        //            .OfClass(typeof(FamilyInstance))
        //            .Cast<FamilyInstance>()
        //            .Where(fi => fi.Symbol.Family.Name == D_nazv_sem_Provoda).ToList();


        //        // Проверяем каждый элемент на видимость в текущем виде.
        //        foreach (var instance in SpPr)
        //        {
        //            try
        //            {
        //                D_vse_provoda.Add(new Provod_Graff(instance));
        //            }
        //            catch { }
        //        }
        //    }

        //    catch { }
        //}

        //public static void GetVisibleFamilyInstances_Potrebit()
        //{

        //    //D_vse_fam_Potrebit_na_Vide

        //    List<FamilyInstance> SpPotr = new List<FamilyInstance>();

        //    D_vse_fam_Potrebit_na_Vide = new List<Potrebit_Graff>();

        //    try
        //    {
        //        // Собираем все элементы на активном виде
        //        FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id);

        //        // Фильтруем только FamilyInstance
        //        SpPotr = allElementsInView
        //            .OfClass(typeof(FamilyInstance))
        //            .Cast<FamilyInstance>()
        //            .Where(fi => fi.Category.Id.IntegerValue == (int)BuiltInCategory.OST_ElectricalEquipment).ToList();


        //        if (SpPotr.Count > 0)
        //        {

        //            List<FamilyInstance> SpPotrFiltered = SpPotr
        //                                    .Where(fi =>
        //                                    {
        //                                        // Получить коннекторы FamilyInstance

        //                                        ConnectorSet connectors = (fi.MEPModel as ElectricalEquipment)?.ConnectorManager?.Connectors;

        //                                        // Проверить, есть ли коннекторы и не пустой ли это список
        //                                        return connectors != null && connectors.Size > 0; // Если разъемы существуют
        //                                    })
        //                                    .ToList();
        //            if (SpPotrFiltered.Count != 0)
        //            {


        //                foreach (var fam in SpPotrFiltered)
        //                {
        //                    Potrebit_Graff pr_gr = new Potrebit_Graff(fam);

        //                    if (pr_gr.Ust_Potr != null)
        //                    {
        //                        D_vse_fam_Potrebit_na_Vide.Add(pr_gr);
        //                    }
        //                }
        //            }


        //        }
        //    }

        //    catch { }

        //}

        public static XYZ ZeroZCoordinatePoint(XYZ original)
        {
            return new XYZ(original.X, original.Y, 0);
        }


        public static bool IsEqualTo_Point(XYZ point1, XYZ point2)
        {

            XYZ point11 = ZeroZCoordinatePoint(point1);
            XYZ point22 = ZeroZCoordinatePoint(point2);


            // Вычисление расстояния между точками
            double distance = point11.DistanceTo(point22);

            // Проверка на соответствие допуску
            return distance <= D_toler_rav_point;
        }


        public static bool IsEqualTo_Point_Con(XYZ point1, XYZ point_con)
        {
            XYZ point11 = ZeroZCoordinatePoint(point1);
            XYZ point22 = ZeroZCoordinatePoint(point_con);


            // Вычисление расстояния между точками
            double distance = point11.DistanceTo(point22);

            bool ss = (distance <= D_toler_tocka_kon);

            // Проверка на соответствие допуску
            return ss;
        }

        public static bool IsEqualTo_Point_Stoyak(XYZ point1, FamilyInstance FAMst)
        {
            XYZ point11 = ZeroZCoordinatePoint(point1);
            XYZ point22 = ZeroZCoordinatePoint((FAMst.Location as LocationPoint).Point);


            // Вычисление расстояния между точками
            double distance = point11.DistanceTo(point22);

            bool ss = (distance <= D_toler_tocka_stoyak);

            // Проверка на соответствие допуску
            return ss;
        }

        public static XYZ GetFirstConnectorLocation(FamilyInstance familyInstance)
        {
            // Проверяем, есть ли MEPModel для FamilyInstance
            MEPModel mepModel = familyInstance.MEPModel;
            if (mepModel == null)
                throw new InvalidOperationException("Элемент не имеет MEP модели.");

            // Получаем менеджер разъемов (ConnectorManager)
            ConnectorManager connectorManager = mepModel.ConnectorManager;
            if (connectorManager == null)
                throw new InvalidOperationException("Элемент не имеет Manager разъемов.");

            // Получаем набор разъемов (ConnectorSet)
            ConnectorSet connectors = connectorManager.Connectors;
            if (connectors == null || connectors.Size == 0)
                throw new InvalidOperationException("Элемент не содержит разъемов.");

            // Перебираем разъемы
            foreach (Connector connector in connectors)
            {
                try
                {
                    return ZeroZCoordinatePoint(connector.Origin);
                }
                catch
                {
                    continue;
                }


                // Координаты разъема

            }

            // Если по какой-то причине разъёмы не найдены
            return null;
        }

        //public static void uznaem_typ_podkl_konc()
        //{
        //    foreach (var prov in D_vse_provoda)
        //    {
        //        foreach (var kon in prov.Sp_Point)
        //        {
        //            kon.Uzn_Podkl_Konci();
        //        }
        //    }
        //}

        public static void uznaem_typ_podkl_konc_dla_vsech_vidov()
        {
            foreach (var prov in D_vse_provoda_vse_vidi)
            {

                foreach (var kon in prov.Sp_Point)
                {
                    kon.Uzn_Podkl_Konci_vsech_vidov();
                }
            }
        }



        public static void uznaem_typ_podkl_konc_na_vide(Graff_Plan_View graff_Plan_View)
        {
            foreach (var prov in graff_Plan_View.SpProvGr)
            {
                foreach (var kon in prov.Sp_Point)
                {
                    kon.Uzn_Podkl_Konci_na_Vide(graff_Plan_View);
                }
            }
        }


        public static List<Provod_Graff> UdalIzSp(List<Provod_Graff> spNach, Provod_Graff elUdal)
        {
            return spNach
                .Where(prov => prov.Pr_family_ins.Id.IntegerValue != elUdal.Pr_family_ins.Id.IntegerValue)
                .ToList();
        }


        public static Provod_Graff Naiti_Prov_v_SP(List<Provod_Graff> spNach, FamilyInstance el_isk)
        {
            Provod_Graff prov_isk = null;

            foreach (var prov in spNach)
            {
                if (prov.Pr_family_ins.Id.IntegerValue == el_isk.Id.IntegerValue)
                {
                    prov_isk = prov;
                    break;
                }

            }

            return prov_isk;
        }


        public static List<Cepochka_Provodov_Graff> UdalIzSp(List<Cepochka_Provodov_Graff> spNach, Cepochka_Provodov_Graff elUdal)
        {
            return spNach
                .Where(prov => prov.Index != elUdal.Index)
                .ToList();
        }


        public static List<Provod_Graff> naitinach_ikonec_cep(List<Provod_Graff> Sp_Vsech_elgr, FamilyInstance fam)
        {
            List<Provod_Graff> rezSP = new List<Provod_Graff>();


            foreach (var el in Sp_Vsech_elgr)
            {
                if (el.sodergit_li_elem(fam) == 1)
                {
                    rezSP.Add(el);
                }
            }


            return rezSP;
        }

        //public static void Orabotat_Prisoedinenia()
        //{
        //    List <Provod_Graff> rab_SP = new List<Provod_Graff>();

        //    foreach ( var pr in D_vse_provoda)
        //    {
        //        pr.Uzn_Podkl_Pr(D_vse_provoda);
        //    }

        //    foreach (var pr in D_vse_provoda)
        //    {
        //        var sp = pr.Prisoed_Pr;

        //        List<Provod_Graff> sp2 = new List<Provod_Graff>();

        //        foreach (var pr2 in sp)
        //        {
        //            foreach (var pr3 in D_vse_provoda)
        //            {
        //                if (pr2.Pr_family_ins.Id.IntegerValue == pr3.Pr_family_ins.Id.IntegerValue)
        //                {
        //                   sp2.Add(pr3);
        //                }
        //            }               
        //        }

        //        pr.Prisoed_Pr = sp2;
        //    }
        //}


        public static void Orabotat_Prisoedinenia_na_vide(Graff_Plan_View graff_Plan_View)
        {


            List<Provod_Graff> rab_SP = new List<Provod_Graff>();

            foreach (var pr in graff_Plan_View.SpProvGr)
            {
                pr.Uzn_Podkl_Pr_Po_vidu(graff_Plan_View);
            }



            foreach (var pr in graff_Plan_View.SpProvGr)
            {
                var sp = pr.Prisoed_Pr;

                List<Provod_Graff> sp2 = new List<Provod_Graff>();

                foreach (var pr2 in sp)
                {
                    foreach (var pr3 in graff_Plan_View.SpProvGr)
                    {
                        if (pr2.Pr_family_ins.Id.IntegerValue == pr3.Pr_family_ins.Id.IntegerValue)
                        {
                            sp2.Add(pr3);
                        }
                    }
                }

                pr.Prisoed_Pr = sp2;
            }
        }

        //public static void Nach_dan_dla_postr_Grafa() // начальные данные для построения графа
        //{
        //    GetVisibleFamilyInstances_Potrebit();

        //    GetVisibleFamilyInstances_Provoda();

        //    int flag = 0;

        //    while (flag == 0)
        //    {
        //        uznaem_typ_podkl_konc();

        //        List<Provod_Graff> SpPr = new List<Provod_Graff>();

        //        foreach (var pr in D_vse_provoda)
        //        {
        //            if ((pr.Sp_Point[0].typ_konec_podkl > 0) && (pr.Sp_Point[1].typ_konec_podkl > 0))
        //            {
        //                SpPr.Add(pr);
        //            }
        //        }

        //        if (SpPr.Count < D_vse_provoda.Count)
        //        {
        //            D_vse_provoda = SpPr;

        //        }

        //        else
        //        {
        //            break;
        //        }
        //    }

        //    Orabotat_Prisoedinenia(); // Здесь выясним присоединенные провода к каждому проводу.

        //}


        //public static void Nach_dan_dla_postr_Grafa_na_vsech_vidach( ) // начальные данные для построения графа
        //{
        //   // GetVisibleFamilyInstances_Potrebit();

        //   //  GetVisibleFamilyInstances_Provoda();        


        //    int flag = 0;

        //    while (flag == 0)
        //    {
        //        uznaem_typ_podkl_konc_na_vide(graff_Plan_View);

        //        List<Provod_Graff> SpPr = new List<Provod_Graff>();

        //        foreach (var pr in graff_Plan_View.SpProvGr)
        //        {
        //            if ((pr.Sp_Point[0].typ_konec_podkl > 0) && (pr.Sp_Point[1].typ_konec_podkl > 0))
        //            {
        //                SpPr.Add(pr);
        //            }
        //        }

        //        if (SpPr.Count < D_vse_provoda.Count)
        //        {
        //            graff_Plan_View.SpProvGr = SpPr;

        //        }

        //        else
        //        {
        //            break;
        //        }
        //    }

        //    Orabotat_Prisoedinenia_na_vide(graff_Plan_View); // Здесь выясним присоединенные провода к каждому проводу.
        //}




        public static List<Provod_Graff> Videlit_Nuzn_Podcepochku(List<Provod_Graff> ischodnaya_cep, FamilyInstance Fa2)
        {
            List<Provod_Graff> isk_cep = new List<Provod_Graff>();

            List<Provod_Graff> cep_rab = new List<Provod_Graff>();

            foreach (var pr in ischodnaya_cep)
            {
                cep_rab.Add(pr);

                int flag3 = 0;

                foreach (var poin in pr.Sp_Point)
                {
                    FamilyInstance fam = poin.Fam_podkluch;

                    if (fam != null)
                    {
                        if (fam.Id.IntegerValue == Fa2.Id.IntegerValue)
                        {
                            flag3 = 1;
                            break;
                        }
                    }
                }

                if (flag3 == 1)
                {
                    isk_cep = cep_rab;
                    break;
                }
            }

            return isk_cep;

        }


        //public static List<Provod_Graff> Nayti_Cepochku_Vegdu_2_El(FamilyInstance Fa1 , FamilyInstance Fa2)
        //{

        //    List<Provod_Graff> isk_cep = new List<Provod_Graff>();

        //    List<Provod_Graff> connect_ELs_nach = naitinach_ikonec_cep(D_vse_provoda, Fa1);


        //    foreach (Provod_Graff ell in connect_ELs_nach)
        //    {

        //        if (isk_cep.Count > 0)
        //        {
        //            break;
        //        }

        //        List<List<Provod_Graff>> VR_SPcep_podkl = new List<List<Provod_Graff>>();
        //        List<Provod_Graff> SPtriniki = new List<Provod_Graff>(); ////////////////////// 
        //        List<Provod_Graff> SPeltriniki = new List<Provod_Graff>();
        //        List<Provod_Graff> cep_podkl = new List<Provod_Graff>();

        //        Provod_Graff el_n = ell;

        //        foreach (var prov in connect_ELs_nach)

        //            if (prov.Pr_family_ins.Id.IntegerValue != el_n.Pr_family_ins.Id.IntegerValue)

        //            {
        //                SPeltriniki.Add(prov);

        //            }

        //        int flag_postr_gr = 0;

        //        while (flag_postr_gr == 0)

        //        {
        //            if (isk_cep.Count > 0)
        //            {
        //                break;
        //            }

        //            List<Provod_Graff> connect_ELs = el_n.Prisoed_Pr;

        //            if (SPeltriniki.Any())

        //            {
        //                List<Provod_Graff> connect_ELs1 = new List<Provod_Graff>();

        //                foreach (Provod_Graff podkl_el in connect_ELs)
        //                {
        //                    int flag1 = 0;

        //                    foreach (Provod_Graff podkl_el1 in SPeltriniki)
        //                    {
        //                        if (podkl_el.Pr_family_ins.Id == podkl_el1.Pr_family_ins.Id)

        //                        {
        //                            flag1 = 1;
        //                        }
        //                    }

        //                    if (flag1 == 0)
        //                    {
        //                        connect_ELs1.Add(podkl_el);
        //                    }
        //                }

        //                connect_ELs = connect_ELs1;
        //            }


        //            if (connect_ELs.Count == 1)
        //            {
        //                cep_podkl.Add(el_n);
        //                SPeltriniki.Add(el_n);
        //                el_n = connect_ELs[0];
        //            }


        //            if (connect_ELs.Count > 1)
        //            {
        //                foreach (var pr in connect_ELs)
        //                {

        //                    List<Provod_Graff> ELs_Els = new List<Provod_Graff>();

        //                    foreach (var pr2 in pr.Prisoed_Pr)
        //                    {
        //                        int rav = 0;

        //                        foreach (var pr3 in connect_ELs)
        //                        {

        //                            if (pr2.Pr_family_ins.Id.IntegerValue == pr3.Pr_family_ins.Id.IntegerValue)
        //                            {
        //                                rav = 1;
        //                                break;
        //                            }
        //                        }

        //                        if (rav == 0)
        //                        {
        //                            ELs_Els.Add(pr2);


        //                        }
        //                    }


        //                    pr.Prisoed_Pr = ELs_Els;
        //                }

        //                SPtriniki.Add(el_n);
        //                SPeltriniki.Add(el_n);
        //                SPeltriniki.Add(connect_ELs[0]);


        //                List<Provod_Graff> cep_podkl1 = new List<Provod_Graff>();

        //                foreach (Provod_Graff el in cep_podkl)
        //                {
        //                    cep_podkl1.Add(el);

        //                }

        //                VR_SPcep_podkl.Add(cep_podkl1);
        //                cep_podkl.Add(el_n);
        //                el_n = connect_ELs[0];

        //            }

        //            if ((connect_ELs.Count == 0) || (connect_ELs == null))

        //            {
        //                cep_podkl.Add(el_n);

        //                // SPcep_podkl.Add(cep_podkl);

        //                isk_cep = Videlit_Nuzn_Podcepochku(cep_podkl, Fa2);

        //                if (isk_cep.Count > 0)
        //                {
        //                    break;
        //                }

        //                if
        //                    (VR_SPcep_podkl.Count != 0)
        //                {

        //                    el_n = SPtriniki[0];
        //                    cep_podkl = new List<Provod_Graff>();

        //                    foreach (Provod_Graff el in VR_SPcep_podkl[0])
        //                    {
        //                        cep_podkl.Add(el);
        //                    }

        //                    SPtriniki.RemoveAt(0);
        //                    VR_SPcep_podkl.RemoveAt(0);

        //                }

        //                else
        //                {
        //                    flag_postr_gr = 1;
        //                    break;
        //                }
        //            }
        //        }
        //    }           

        //    foreach (var pr in D_vse_provoda)
        //    {
        //        pr.Zapis_IZ_Zapasn();
        //    }

        //    return isk_cep;
        //}


        //public static List<List<Provod_Graff>> Nayti_Vse_Cepochku_Ravnoznacn(List<FamilyInstance> Fams)  //     
        //{
        //    ///////////////////////////////////////////////////////////////////////////////////

        //    Nach_dan_dla_postr_Grafa(); // получаем все начальные данные для построения графа с текщего вида

        //    ///////////////////////////////////////////////////////////////////////////////////////
        //    List <List< Provod_Graff >> Vse_Cep = new List<List<Provod_Graff>> ();

        //    int i = 0;

        //    while (i < Fams.Count-1) 
        //    {
        //        FamilyInstance Fa1 = Fams[i];

        //        FamilyInstance Fa2 = Fams[i+1];

        //        try
        //        {
        //            List<Provod_Graff> cep = Nayti_Cepochku_Vegdu_2_El(Fa1, Fa2);

        //            if (cep.Count > 0)
        //            {
        //                Vse_Cep.Add(cep);
        //            }
        //        }
        //        catch { }

        //        i++;

        //        if (i >= (Fams.Count - 1))
        //        {
        //            break;
        //        }
        //    }

        //    return Vse_Cep;

        //}

        public static List<List<Provod_Graff>> Nayti_Vse_Cepochku_Ravnoznacn_na_vse_vidi(List<FamilyInstance> Fams, List<ViewPlan> SpPlan)
        {
            List<List<Provod_Graff>> Vse_Cep = new List<List<Provod_Graff>>();

            Uzn_VSEGrVidi_Provod_Graf(SpPlan);

            int i = 0;

            while (i < Fams.Count - 1)
            {
                FamilyInstance Fa1 = Fams[i];

                FamilyInstance Fa2 = Fams[i + 1];

                try
                {
                    List<Provod_Graff> cep = Nayti_Cepochku_Vegdu_2_El_na_nesk_vidov(Fa1, Fa2);  // Nayti_Cepochku_Vegdu_2_El(Fa1, Fa2);

                    if (cep.Count > 0)
                    {
                        Vse_Cep.Add(cep);
                    }

                    else
                    {
                        Vse_Cep = new List<List<Provod_Graff>>();
                        return Vse_Cep;
                    }


                }
                catch { }

                i++;

                if (i >= (Fams.Count - 1))
                {
                    break;
                }
            }

            return Vse_Cep;

        }

        public static List<Provod_Graff> Nayti_Unicaln_El(List<List<Provod_Graff>> Vse_cep_gr)
        {
            // Используем HashSet для контроля уникальности на основании Id элементов
            HashSet<int> uniqueIds = new HashSet<int>();
            List<Provod_Graff> SpItog = new List<Provod_Graff>();

            foreach (var sp in Vse_cep_gr)
            {
                foreach (var pr in sp)
                {
                    if (uniqueIds.Add(pr.Pr_family_ins.Id.IntegerValue)) // Добавляет только уникальные элементы
                    {
                        SpItog.Add(pr);
                    }
                }
            }

            return SpItog;
        }

        public static List<Tuple<Provod_Graff, int>> Nayti_Unicaln_El_S_Colichestvom(List<List<Provod_Graff>> Vse_cep_gr) // Полчаем уникальные элементы графа и сколько раз они повторяются
        {
            return Vse_cep_gr
                .SelectMany(sp => sp) // Разворачиваем вложенные списки в один список
                .GroupBy(pr => pr.Pr_family_ins.Id.IntegerValue) // Группируем элементы по уникальному ключу
                .Select(g => new Tuple<Provod_Graff, int>(g.First(), g.Count())) // Создаём Tuple (элемент, количество повторений)
                .ToList(); // Преобразуем в список
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // Методы касаемые определения принадлежности семейства к графу. Подходит ли конец провода к потребителю

        public static bool IsPointInElem(FamilyInstance fi, XYZ point) // Главная функция
        {
            bool rez = false;

            try
            {
                List<Line> SpLine = new List<Line>();

                try
                {
                    SpLine = GetAnnotationLines(fi);
                }
                catch
                { }

                if (SpLine.Count < 4)
                {
                    SpLine = NaytyBonBox_iz_El(fi);
                }

                rez = IsPointInsideProjectedContour(SpLine, point);
            }
            catch { }

            return rez;
        }


        public static List<Line> NaytyBonBox_iz_El(FamilyInstance fi)
        {

            Autodesk.Revit.DB.View view = doc.ActiveView;

            BoundingBoxXYZ bbox = fi.get_BoundingBox(view);

            Transform transform = doc.ActiveView.CropBox.Transform;

            List<Line> SpLine = new List<Line>();

            XYZ min = transform.OfPoint(bbox.Min);
            XYZ max = transform.OfPoint(bbox.Max);

            Line line1 = Line.CreateBound(new XYZ(min.X, min.Y, min.Z), new XYZ(max.X, min.Y, min.Z));
            Line line2 = Line.CreateBound(new XYZ(min.X, max.Y, min.Z), new XYZ(max.X, max.Y, min.Z));
            Line line3 = Line.CreateBound(new XYZ(max.X, max.Y, min.Z), new XYZ(min.X, max.Y, min.Z));
            Line line4 = Line.CreateBound(new XYZ(max.X, min.Y, min.Z), new XYZ(max.X, max.Y, min.Z));

            SpLine.Add(line1);
            SpLine.Add(line2);
            SpLine.Add(line3);
            SpLine.Add(line4);

            return SpLine;
        }


        public static List<Line> GetAnnotationLines(FamilyInstance fi)
        {
            double MillimetersToFeet = 0.00328084;

            List<Line> lines = new List<Line>();

            // Получение трансформации семейства
            Transform transform = fi.GetTransform();

            // Извлечение параметров ∆X и ∆Y
            double deltaX = 5000 * MillimetersToFeet - GetParameterValue(fi, "∆X"); // Вычитаем базовое значение 5000, чтобы получить реальное смещение
            double deltaY = GetParameterValue(fi, "∆Y") - 5000 * MillimetersToFeet;


            // Центр аннотации в локальной системе координат
            XYZ annotationCenterLocal = new XYZ(deltaX, deltaY, 0);

            // Преобразование центра аннотации в мировую систему координат
            XYZ annotationCenterGlobal = transform.OfPoint(annotationCenterLocal);

            // Определение угла поворота семейства
            double rotationAngle = GetRotationAngle(transform.BasisX);

            // Создание квадрата 500x500 мм вокруг центра
            double halfSideLength = 250 * MillimetersToFeet; // Половина стороны квадрата в футах

            // Вершины квадрата в локальной системе координат
            XYZ[] verticesLocal = new XYZ[]
            {
            new XYZ(-halfSideLength, -halfSideLength, 0),
            new XYZ(-halfSideLength, halfSideLength, 0),
            new XYZ(halfSideLength, halfSideLength, 0),
            new XYZ(halfSideLength, -halfSideLength, 0)
            };

            // Поворот вершин квадрата параллельно углу поворота семейства
            for (int i = 0; i < verticesLocal.Length; i++)
            {
                verticesLocal[i] = RotatePoint(verticesLocal[i], rotationAngle);
            }

            // Перенос вершин квадрата в мировую систему координат
            for (int i = 0; i < verticesLocal.Length; i++)
            {
                verticesLocal[i] = verticesLocal[i] + annotationCenterGlobal;
            }

            // Получение значения параметра "УГО_Угол поворота" (в градусах)
            double ugoRotationRadians = (Math.PI / 2) - GetParameterValue(fi, "УГО_Угол поворота");


            // Дополнительный поворот вершин вокруг центра аннотации (УГО_Угол поворота)
            for (int i = 0; i < verticesLocal.Length; i++)
            {
                verticesLocal[i] = RotatePointAroundCenter(verticesLocal[i], annotationCenterGlobal, ugoRotationRadians);
            }


            // Создание линий квадрата
            lines.Add(Line.CreateBound(verticesLocal[0], verticesLocal[1]));
            lines.Add(Line.CreateBound(verticesLocal[1], verticesLocal[2]));
            lines.Add(Line.CreateBound(verticesLocal[2], verticesLocal[3]));
            lines.Add(Line.CreateBound(verticesLocal[3], verticesLocal[0]));

            return lines;
        }

        // Дополнительный метод для поворота точки вокруг заданного центра
        private static XYZ RotatePointAroundCenter(XYZ point, XYZ center, double angle)
        {
            // Смещаем точку так, чтобы центр стал началом координат
            XYZ shiftedPoint = point - center;

            // Поворачиваем точку вокруг начала координат
            XYZ rotatedPoint = RotatePoint(shiftedPoint, angle);

            // Возвращаем точку обратно в исходную систему координат
            return rotatedPoint + center;
        }

        private static double GetParameterValue(FamilyInstance fi, string paramName)
        {
            // Получение параметра по имени
            Parameter param = fi.LookupParameter(paramName);
            if (param != null && param.HasValue)
            {
                return param.AsDouble(); // Значение параметра в футах
            }
            throw new InvalidOperationException($"Параметр {paramName} не найден.");
        }

        private static double GetRotationAngle(XYZ basisX)
        {
            // Угол поворота в радианах относительно оси X
            double angle = Math.Atan2(basisX.Y, basisX.X);
            return angle;
        }

        private static XYZ RotatePoint(XYZ point, double angle)
        {
            // Поворот точки вокруг начала координат
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            double xNew = point.X * cos - point.Y * sin;
            double yNew = point.X * sin + point.Y * cos;
            return new XYZ(xNew, yNew, point.Z);
        }

        public static bool IsPointInsideProjectedContour(List<Line> spLines, XYZ point)
        {
            // Проецируем точку на плоскость Z = 0
            XYZ projectedPoint = new XYZ(point.X, point.Y, 0);

            // Создаём список точек, образующих контур, проецируя их на Z = 0
            List<XYZ> contourPoints = new List<XYZ>();
            foreach (Line line in spLines)
            {
                contourPoints.Add(new XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, 0));
            }

            // Проверяем, находится ли точка внутри проекции контура
            return IsPointInPolygon(projectedPoint, contourPoints);
        }

        private static bool IsPointInPolygon(XYZ point, List<XYZ> polygon)
        {
            int intersections = 0;
            int count = polygon.Count;

            for (int i = 0; i < count; i++)
            {
                XYZ vertex1 = polygon[i];
                XYZ vertex2 = polygon[(i + 1) % count];

                // Проверяем, если ребро пересекает горизонтальную линию через точку
                if ((vertex1.Y > point.Y) != (vertex2.Y > point.Y))
                {
                    double atX = vertex1.X +
                                 (point.Y - vertex1.Y) * (vertex2.X - vertex1.X) / (vertex2.Y - vertex1.Y);
                    if (point.X < atX)
                    {
                        intersections++;
                    }
                }
            }

            // Если количество пересечений нечётное — точка внутри
            return intersections % 2 != 0;
        }

        // Конец раздела методов касаемых определения принадлежности семейства к графу. Подходит ли конец провода к потребителю
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void NaytiVseStoyaki()  //    D_Vse_StoYak 
        {
            List<Stoyak> SpOkonch = new List<Stoyak>();

            List<FamilyInstance> FamsVse_StoYak_Fams = new List<FamilyInstance>();

            // 1. Поиск всех семейств с именем "ДСК1_ЭЛУ_СС_Направление кабеля"
            var familyInstances = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(fi => fi.Symbol.FamilyName == D_NazvSem_Sroyak)
                    .ToList();

            foreach (Element element in familyInstances)
            {
                FamilyInstance familyInstance = element as FamilyInstance;
                if (familyInstance == null)
                    continue;

                // Получаем ID связанного вида
                ElementId ownerViewId = familyInstance.OwnerViewId;

                // Проверяем, что ID вида действителен
                if (ownerViewId != ElementId.InvalidElementId)
                {
                    // Получаем элемент вида
                    Element viewElement = doc.GetElement(ownerViewId);

                    // Проверяем, является ли элемент видом типа ViewPlan
                    if (viewElement is ViewPlan)
                    {
                        string zn_par = familyInstance.LookupParameter(D_NazvPar_Ident).AsString();

                        if ((zn_par != D_Pust_Znach) && (!string.IsNullOrEmpty(zn_par)))
                        {
                            FamsVse_StoYak_Fams.Add(familyInstance);
                        }
                    }
                }
            }

            List<Stoyak> spstoyakPredvar = new List<Stoyak>();

            if (FamsVse_StoYak_Fams.Count > 0)
            {
                foreach (var fam in FamsVse_StoYak_Fams)
                {
                    spstoyakPredvar.Add(new Stoyak(fam));
                }
            }

            if (spstoyakPredvar.Count > 0)
            {
                foreach (var stoyak1 in spstoyakPredvar)
                {
                    stoyak1.SpSvyaz = new List<Stoyak>();

                    stoyak1.SPViewPlan = new List<ViewPlan>();

                    stoyak1.SpSvyazVse = new List<Stoyak>();

                    foreach (var stoyak2 in spstoyakPredvar)
                    {
                        if ((stoyak1.famins.Id.IntegerValue != stoyak2.famins.Id.IntegerValue) && (stoyak1.NazvIdentIf == stoyak2.NazvIdentIf))
                        {
                            stoyak1.SpSvyaz.Add(stoyak2);
                            stoyak1.SpSvyazVse.Add(stoyak2);

                            ViewPlan vp = stoyak2.RoditViewPlan;

                            int flag = 0;

                            foreach (var vp1 in stoyak1.SPViewPlan)
                            {
                                if (vp1.Id.IntegerValue == vp.Id.IntegerValue)
                                {
                                    flag = 1;
                                    break;
                                }
                            }

                            if (flag == 0)
                            {
                                stoyak1.SPViewPlan.Add(vp);
                            }

                        }
                    }
                }

                List<ViewPlan> VseVidi = new List<ViewPlan>();

                foreach (var stoyak1 in spstoyakPredvar)
                {
                    foreach (var vpp in stoyak1.SPViewPlan)
                    {
                        VseVidi.Add(vpp);
                    }
                }

                VseVidi = VseVidi
               .GroupBy(view => view.Id.IntegerValue) // Группируем по IntegerValue ID
               .Select(group => group.First())        // Берем первый элемент каждой группы
               .ToList();

                List<(ViewPlan VP, List<Stoyak> SPST)> SP_po_vidam = new List<(ViewPlan VP, List<Stoyak> SPST)>();

                foreach (var vp in VseVidi)
                {
                    List<Stoyak> spst = new List<Stoyak>();

                    foreach (var stoyak1 in spstoyakPredvar)
                    {
                        if (stoyak1.RoditViewPlan.Id.IntegerValue == vp.Id.IntegerValue)
                        {
                            spst.Add(stoyak1);
                        }
                    }

                    SP_po_vidam.Add((vp, spst));
                }

                List<(Stoyak st, List<Stoyak> SpSt)> Spstst = new List<(Stoyak st, List<Stoyak> SpSt)>();

                foreach (var stoyak1 in spstoyakPredvar)
                {
                    List<Stoyak> spnavide = new List<Stoyak>();

                    foreach (var sppv in SP_po_vidam)
                    {
                        if ((stoyak1.RoditViewPlan.Id.IntegerValue == sppv.VP.Id.IntegerValue))
                        {
                            spnavide = sppv.SPST;

                            foreach (var stsv in stoyak1.SpSvyaz)
                            {
                                spnavide.RemoveAll(x => x.famins.Id.IntegerValue == stsv.famins.Id.IntegerValue);

                                spnavide.Add(stsv);
                            }

                            spnavide.RemoveAll(x => x.famins.Id.IntegerValue == stoyak1.famins.Id.IntegerValue);
                        }
                    }

                    Spstst.Add((stoyak1, spnavide));
                }



                foreach (var stoyak_sp in Spstst)
                {
                    List<Stoyak> spst1 = new List<Stoyak>();
                    List<Stoyak> spst2 = new List<Stoyak>();

                    foreach (var stt in stoyak_sp.SpSt)
                    {
                        spst1.Add(stt);
                        spst2.Add(stt);
                    }

                    int flag = 0;

                    int ch = 1;

                    while (flag == 0)
                    {
                        foreach (var sttt in spst1)
                        {
                            foreach (var stoyak_sp2 in Spstst)
                            {
                                if (stoyak_sp.st.famins.Id.IntegerValue != stoyak_sp2.st.famins.Id.IntegerValue)
                                {
                                    if ((sttt.famins.Id.IntegerValue == stoyak_sp2.st.famins.Id.IntegerValue))
                                    {
                                        foreach (var sttt2 in stoyak_sp2.SpSt)
                                        {
                                            spst2.Add(sttt2);
                                        }
                                    }
                                }
                            }

                            spst2 = spst2
                            .GroupBy(st => st.famins.Id.IntegerValue) // Группируем по Famins.Id.IntegerValue
                            .Select(group => group.First()) // Берем первый элемент из каждой группы
                            .ToList();
                        }

                        if (spst1.Count == spst2.Count)
                        {
                            flag = 1;
                            break;
                        }

                        ch++;
                        if (ch > 50000)
                        {
                            break;
                        }

                        spst1.Clear();

                        foreach (var stt in spst2)
                        {
                            spst1.Add(stt);
                        }
                    }

                    spst2.RemoveAll(x => x.famins.Id.IntegerValue == stoyak_sp.st.famins.Id.IntegerValue);

                    List<ViewPlan> SpPlan = new List<ViewPlan>();

                    foreach (var st222 in spst2)
                    {
                        SpPlan.Add(st222.RoditViewPlan);
                    }

                    SpPlan = SpPlan
                            .GroupBy(pl => pl.Id.IntegerValue) // Группируем по Famins.Id.IntegerValue
                            .Select(group => group.First()) // Берем первый элемент из каждой группы
                            .ToList();

                    stoyak_sp.st.SpSvyazVse = spst2;

                    stoyak_sp.st.VSESPViewPlan = SpPlan;

                    SpOkonch.Add(stoyak_sp.st);

                }
            }

            D_Vse_StoYak = SpOkonch;
        }


        //public static List<Graff_Plan_View> D_Vse_Obrabat_Vidi = new List<Graff_Plan_View>();

        //public static List<Provod_Graff> D_vse_provoda_vse_vidi = new List<Provod_Graff>();

        public static void Uzn_VSEGrVidi_Provod_Graf(List<ViewPlan> vsePlani)
        {
            if (vsePlani == null)
            {
                throw new ArgumentNullException(nameof(vsePlani), "Список ViewPlan не может быть null.");
            }


            // Проверка: Оставляем только уникальные ViewPlan по el.Id.IntegerValue
            vsePlani = vsePlani
           .GroupBy(v => v.Id.IntegerValue)
           .Select(g => g.First())
           .ToList();



            // Инициализация списков
            var dVseObrabotkaVidi = new List<Graff_Plan_View>();
            var dVseProvodaVseVidi = new List<Provod_Graff>();

            // Заполняем список Graff_Plan_View и обрабатываем их
            foreach (var viewPlan in vsePlani)
            {
                var graffPlanView = new Graff_Plan_View(viewPlan);

                uznaem_typ_podkl_konc_na_vide(graffPlanView);
                Orabotat_Prisoedinenia_na_vide(graffPlanView);


                dVseObrabotkaVidi.Add(graffPlanView);

                // Извлекаем список проводов и добавляем в общий список, если он не пустой
                if (graffPlanView.SpProvGr.Any())
                {
                    dVseProvodaVseVidi.AddRange(graffPlanView.SpProvGr);
                }
            }

            dVseProvodaVseVidi = dVseProvodaVseVidi
           .GroupBy(v => v.Pr_family_ins.Id.IntegerValue)
            .Select(g => g.First())
               .ToList();

            // Тут можно назначить результирующие списки в свойства или вернуть их, если нужно
            D_Vse_Obrabat_Vidi = dVseObrabotkaVidi;
            D_vse_provoda_vse_vidi = dVseProvodaVseVidi;



            foreach (var pr in D_vse_provoda_vse_vidi)
            {
                pr.Uzn_Podkl_Pr_S_Ucetom_StoYak();
            }

            uznaem_typ_podkl_konc_dla_vsech_vidov();

            D_vse_fam_Potrebit_na_vsech_Vidach = new List<Potrebit_Graff>();

            foreach (var vid in D_Vse_Obrabat_Vidi)
            {
                foreach (var potr in vid.SpPotrGr)
                {
                    D_vse_fam_Potrebit_na_vsech_Vidach.Add(potr);
                }
            }

            D_vse_fam_Potrebit_na_vsech_Vidach = D_vse_fam_Potrebit_na_vsech_Vidach
           .GroupBy(v => v.fam_potr.Id.IntegerValue)
            .Select(g => g.First())
               .ToList();




        }

        public static List<Provod_Graff> Nayti_Cepochku_Vegdu_2_El_na_nesk_vidov(FamilyInstance Fa1, FamilyInstance Fa2)
        {

            List<Provod_Graff> isk_cep = new List<Provod_Graff>();

            List<Provod_Graff> connect_ELs_nach = naitinach_ikonec_cep(D_vse_provoda_vse_vidi, Fa1);

            foreach (Provod_Graff ell in connect_ELs_nach)
            {

                if (isk_cep.Count > 0)
                {
                    break;
                }

                List<List<Provod_Graff>> VR_SPcep_podkl = new List<List<Provod_Graff>>();
                List<Provod_Graff> SPtriniki = new List<Provod_Graff>(); ////////////////////// 
                List<Provod_Graff> SPeltriniki = new List<Provod_Graff>();
                List<Provod_Graff> cep_podkl = new List<Provod_Graff>();

                Provod_Graff el_n = ell;

                foreach (var prov in connect_ELs_nach)
                {

                    if (prov.Pr_family_ins.Id.IntegerValue != el_n.Pr_family_ins.Id.IntegerValue)

                    {
                        SPeltriniki.Add(prov);

                    }
                }

                int flag_postr_gr = 0;

                while (flag_postr_gr == 0)

                {
                    if (isk_cep.Count > 0)
                    {
                        break;
                    }

                    List<Provod_Graff> connect_ELs = el_n.Prisoed_Pr;

                    if (SPeltriniki.Any())

                    {
                        List<Provod_Graff> connect_ELs1 = new List<Provod_Graff>();

                        foreach (Provod_Graff podkl_el in connect_ELs)
                        {
                            int flag1 = 0;

                            foreach (Provod_Graff podkl_el1 in SPeltriniki)
                            {
                                if (podkl_el.Pr_family_ins.Id == podkl_el1.Pr_family_ins.Id)

                                {
                                    flag1 = 1;
                                }
                            }

                            if (flag1 == 0)
                            {
                                connect_ELs1.Add(podkl_el);
                            }
                        }

                        connect_ELs = connect_ELs1;
                    }


                    if (connect_ELs.Count == 1)
                    {
                        cep_podkl.Add(el_n);
                        SPeltriniki.Add(el_n);
                        el_n = connect_ELs[0];
                    }


                    if (connect_ELs.Count > 1)
                    {
                        foreach (var pr in connect_ELs)
                        {

                            List<Provod_Graff> ELs_Els = new List<Provod_Graff>();

                            foreach (var pr2 in pr.Prisoed_Pr)
                            {
                                int rav = 0;

                                foreach (var pr3 in connect_ELs)
                                {

                                    if (pr2.Pr_family_ins.Id.IntegerValue == pr3.Pr_family_ins.Id.IntegerValue)
                                    {
                                        rav = 1;
                                        break;
                                    }
                                }

                                if (rav == 0)
                                {
                                    ELs_Els.Add(pr2);


                                }
                            }

                            pr.Prisoed_Pr = ELs_Els;
                        }

                        SPtriniki.Add(el_n);
                        SPeltriniki.Add(el_n);
                        SPeltriniki.Add(connect_ELs[0]);


                        List<Provod_Graff> cep_podkl1 = new List<Provod_Graff>();

                        foreach (Provod_Graff el in cep_podkl)
                        {
                            cep_podkl1.Add(el);
                        }

                        VR_SPcep_podkl.Add(cep_podkl1);
                        cep_podkl.Add(el_n);
                        el_n = connect_ELs[0];
                    }

                    if ((connect_ELs.Count == 0) || (connect_ELs == null))

                    {
                        cep_podkl.Add(el_n);

                        // SPcep_podkl.Add(cep_podkl);

                        isk_cep = Videlit_Nuzn_Podcepochku(cep_podkl, Fa2);

                        if (isk_cep.Count > 0)
                        {

                            break;
                        }

                        if
                            (VR_SPcep_podkl.Count != 0)
                        {

                            el_n = SPtriniki[0];
                            cep_podkl = new List<Provod_Graff>();

                            foreach (Provod_Graff el in VR_SPcep_podkl[0])
                            {
                                cep_podkl.Add(el);
                            }

                            SPtriniki.RemoveAt(0);
                            VR_SPcep_podkl.RemoveAt(0);

                        }

                        else
                        {
                            flag_postr_gr = 1;
                            break;
                        }
                    }
                }
            }

            foreach (var pr in D_vse_provoda_vse_vidi)
            {
                pr.Zapis_IZ_Zapasn();
            }

           
            if (isk_cep.Count > 0)
            {
               isk_cep = obrabotka_cepocki_na_stoyaki(isk_cep);              
            }          

            return isk_cep;
        }

        public static List<List<Provod_Graff>> Nayti_Cepochki_Vegdu_2_El_na_nesk_vidov(FamilyInstance Fa1, FamilyInstance Fa2) 
        {

            List<List<Provod_Graff>> isk_cepSP = new List<List<Provod_Graff>>();

            List<Provod_Graff> isk_cep = new List<Provod_Graff>();

            List<Provod_Graff> connect_ELs_nach = naitinach_ikonec_cep(D_vse_provoda_vse_vidi, Fa1);

            foreach (Provod_Graff ell in connect_ELs_nach)
            {
                //if (isk_cep.Count > 0)
                //{
                //    break;
                //}

                List<List<Provod_Graff>> VR_SPcep_podkl = new List<List<Provod_Graff>>();
                List<Provod_Graff> SPtriniki = new List<Provod_Graff>(); ////////////////////// 
                List<Provod_Graff> SPeltriniki = new List<Provod_Graff>();
                List<Provod_Graff> cep_podkl = new List<Provod_Graff>();

                Provod_Graff el_n = ell;

                foreach (var prov in connect_ELs_nach)
                {

                    if (prov.Pr_family_ins.Id.IntegerValue != el_n.Pr_family_ins.Id.IntegerValue)

                    {
                        SPeltriniki.Add(prov);

                    }
                }

                int flag_postr_gr = 0;

                while (flag_postr_gr == 0)

                {
                    //if (isk_cep.Count > 0)
                    //{
                    //    break;
                    //}

                    List<Provod_Graff> connect_ELs = el_n.Prisoed_Pr;

                    if (SPeltriniki.Any())

                    {
                        List<Provod_Graff> connect_ELs1 = new List<Provod_Graff>();

                        foreach (Provod_Graff podkl_el in connect_ELs)
                        {
                            int flag1 = 0;

                            foreach (Provod_Graff podkl_el1 in SPeltriniki)
                            {
                                if (podkl_el.Pr_family_ins.Id == podkl_el1.Pr_family_ins.Id)

                                {
                                    flag1 = 1;
                                }
                            }

                            if (flag1 == 0)
                            {
                                connect_ELs1.Add(podkl_el);
                            }
                        }

                        connect_ELs = connect_ELs1;
                    }


                    if (connect_ELs.Count == 1)
                    {
                        cep_podkl.Add(el_n);
                        SPeltriniki.Add(el_n);
                        el_n = connect_ELs[0];
                    }


                    if (connect_ELs.Count > 1)
                    {
                        foreach (var pr in connect_ELs)
                        {

                            List<Provod_Graff> ELs_Els = new List<Provod_Graff>();

                            foreach (var pr2 in pr.Prisoed_Pr)
                            {
                                int rav = 0;

                                foreach (var pr3 in connect_ELs)
                                {

                                    if (pr2.Pr_family_ins.Id.IntegerValue == pr3.Pr_family_ins.Id.IntegerValue)
                                    {
                                        rav = 1;
                                        break;
                                    }
                                }

                                if (rav == 0)
                                {
                                    ELs_Els.Add(pr2);


                                }
                            }

                            pr.Prisoed_Pr = ELs_Els;
                        }

                        SPtriniki.Add(el_n);
                        SPeltriniki.Add(el_n);
                        SPeltriniki.Add(connect_ELs[0]);


                        List<Provod_Graff> cep_podkl1 = new List<Provod_Graff>();

                        foreach (Provod_Graff el in cep_podkl)
                        {
                            cep_podkl1.Add(el);
                        }

                        VR_SPcep_podkl.Add(cep_podkl1);
                        cep_podkl.Add(el_n);
                        el_n = connect_ELs[0];
                    }

                    if ((connect_ELs.Count == 0) || (connect_ELs == null))

                    {
                        cep_podkl.Add(el_n);                        

                        isk_cep = Videlit_Nuzn_Podcepochku(cep_podkl, Fa2);

                        if (isk_cep.Count > 0)
                        {                   

                            List<Provod_Graff>  isk_cep11 = new List<Provod_Graff>();

                            if (isk_cep.Count > 0)
                            {
                                foreach (var ddd in isk_cep)
                                {
                                    isk_cep11.Add(ddd);
                                }                            
                            }

                            if (isk_cepSP.Count > 0)
                            {
                                var poslpr = isk_cepSP.Last();

                                int flag = 0;

                                if (poslpr.Count != isk_cep11.Count)
                                {
                                    flag = 1;
                                }

                                else
                                {

                                    for (int i = 0; i < poslpr.Count; i++)
                                    {


                                        var eli1 = poslpr[i];
                                        var eli2 = isk_cep11[i];



                                        if (eli1.Pr_family_ins.Id.IntegerValue != eli2.Pr_family_ins.Id.IntegerValue)
                                        {
                                            flag = 1;
                                            break;

                                        }
                                    }
                                }

                                if (flag == 1)
                                {
                                    isk_cepSP.Add(isk_cep11);
                                }

                            }

                            else 
                            {
                                isk_cepSP.Add(isk_cep11);
                            }                           

                            isk_cep.Clear();
                        }

                        

                        if
                            (VR_SPcep_podkl.Count != 0)
                        {

                            el_n = SPtriniki[0];
                            cep_podkl = new List<Provod_Graff>();

                            foreach (Provod_Graff el in VR_SPcep_podkl[0])
                            {
                                cep_podkl.Add(el);
                            }

                            SPtriniki.RemoveAt(0);
                            VR_SPcep_podkl.RemoveAt(0);

                        }

                        else
                        {
                            flag_postr_gr = 1;
                            break;
                        }
                    }
                }
            }

            foreach (var pr in D_vse_provoda_vse_vidi)
            {
                pr.Zapis_IZ_Zapasn();
            }


            //if (isk_cepSP.Count < 0)
            //{
            //    if (isk_cep.Count > 0)
            //    {
            //        isk_cep = obrabotka_cepocki_na_stoyaki(isk_cep);
            //    }
            //}

            return isk_cepSP;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// По поводу выбора видов

        public static List<ViewPlan> NaytiVdi(List<FamilyInstance> sp_fams, int flag_pereshet) //  int flag_pereshet если 0
                                                                                               //  то если все на одном виде то
                                                                                               //  не пытаться искать еще , а если 1 то пытаться
        {
            List<ViewPlan> SpViewRez = new List<ViewPlan>();

            bool Odin_Vid = true;

            // Шаг 1: Берем первый элемент из списка
            if (sp_fams == null || sp_fams.Count == 0)
            {
                return SpViewRez;
            }
            FamilyInstance firstElement = sp_fams[0];

            // Шаг 2: Проверяем, является текущий вид ViewPlan
            // Autodesk.Revit.DB.View currentView = doc.ActiveView;
            Autodesk.Revit.DB.View selectedView = doc.ActiveView;

            if (selectedView is ViewPlan)
            {
                uidoc.ActiveView = selectedView;
            }

            else
            {
                selectedView = FindViewWithElement(firstElement);
                // currentView = FindViewWithElement(firstElement);

                uidoc.ActiveView = selectedView;
            }

            if (selectedView != null)
            {
                if (selectedView is ViewPlan)
                {
                    // Шаг 7: Проверяем наличие всех остальных элементов из списка на этом виде
                    bool allElementsVisible = sp_fams.All(element => IsElementVisibleOnView(element, selectedView));

                    if (allElementsVisible)
                    {
                        Odin_Vid = true;
                    }
                    else
                    {
                        Odin_Vid = false;
                    }
                }

                else
                {
                    selectedView = null;
                }
            }

            else
            {
                return SpViewRez;
            }

            if ((selectedView != null) && (Odin_Vid) && (flag_pereshet == 0))
            {
                SpViewRez.Add(selectedView as ViewPlan);
            }

            if (((selectedView != null) && (!Odin_Vid)) || ((selectedView != null) && (Odin_Vid) && (flag_pereshet == 1)))
            {

                if (D_Vse_StoYak == null)
                {
                    NaytiVseStoyaki();
                }

                if (D_Vse_StoYak.Count < 1)
                {
                    NaytiVseStoyaki();
                }

                if (D_Vse_StoYak.Count > 0)
                {
                    foreach (var st in D_Vse_StoYak)
                    {
                        if (st.RoditViewPlan.Id.IntegerValue == selectedView.Id.IntegerValue)
                        {
                            SpViewRez.Add(selectedView as ViewPlan);

                            foreach (var vv in st.VSESPViewPlan)
                            {
                                SpViewRez.Add(vv);
                            }
                        }
                    }
                }

                else
                {
                    return SpViewRez;
                }
            }

            SpViewRez = SpViewRez
           .GroupBy(el => el.Id.IntegerValue) // Группируем по IntegerValue Id элемента
           .Select(group => group.First())   // Берем первый элемент каждой группы
           .ToList();                       // Преобразуем результат снова в список


            return SpViewRez;

        }

        /// <summary>
        /// Проверяет, видим ли элемент на указанном виде.
        /// </summary>
        public static bool IsElementVisibleOnView(FamilyInstance element, Autodesk.Revit.DB.View view)
        {
            // Собираем все элементы в активном виде
            FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, view.Id);

            // Проверяем, содержит ли коллекция элемент по его ID


            // Убедимся, что элемент присутствует в фильтрованной коллекции
            bool isVisible = allElementsInView.Any(el => el.Id.IntegerValue == element.Id.IntegerValue);


            return isVisible;
        }

        /// <summary>
        /// Ищет вид типа ViewPlan, на котором элемент виден.
        /// </summary>
        public static Autodesk.Revit.DB.View FindViewWithElement(FamilyInstance element)
        {
            IList<UIView> openUIViews = uidoc.GetOpenUIViews();

            // Переходим в открытые UI-виде и извлекаем соответствующие View (модели в документах)
            foreach (UIView uiView in openUIViews)
            {
                // Получаем идентификатор открытого вида
                ElementId viewId = uiView.ViewId;

                // Получаем представление (View) по его ElementId
                Autodesk.Revit.DB.View openView = doc.GetElement(viewId) as Autodesk.Revit.DB.View;

                if (openView != null) // Если удалось получить View (проверка на null)
                {
                    // Проверяем видимость элемента на этом виде
                    if (IsElementVisibleOnView(element, openView))
                    {
                        return openView; // Если найден нужный вид, возвращаем его
                    }
                }
            }

            // Получаем все виды типа ViewPlan
            ICollection<Autodesk.Revit.DB.View> views = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .Cast<Autodesk.Revit.DB.View>()
                .ToList();

            // Проверяем наличие элемента на каждом виде
            foreach (Autodesk.Revit.DB.View viewPlan in views)
            {
                if (IsElementVisibleOnView(element, viewPlan))
                {
                    return viewPlan;
                }
            }

            return null;
        }


        public static List<Provod_Graff> obrabotka_cepocki_na_stoyaki (List<Provod_Graff> CepochkaGraf)
        {
            List < Provod_Graff > newCepochkaGraf = new List < Provod_Graff >();

            List <Stoyak> SP_St = new List <Stoyak>();

            foreach (Provod_Graff prov in CepochkaGraf)
            {
                newCepochkaGraf.Add(prov);

                prov.Uzn_Stoyak();

               // TaskDialog.Show("Есть такой", prov.Prisoed_Stoyak.Count.ToString());

                try
                {
                    if ((prov.Prisoed_Stoyak.Count > 0) && (prov.Prisoed_Pr.Count > 0))
                    {
                        foreach (Stoyak st1 in prov.Prisoed_Stoyak)
                        {
                            foreach (Provod_Graff prov2 in prov.Prisoed_Pr)
                            {
                                if (prov2.Prisoed_Stoyak.Count > 0)
                                {
                                    prov2.Uzn_Stoyak();

                                    foreach (Stoyak st2 in prov2.Prisoed_Stoyak)
                                    {
                                        if ((st1.NazvIdentIf == st2.NazvIdentIf) && (st1.NazvIdentIf != D_Pust_Znach) && (!string.IsNullOrEmpty(st1.NazvIdentIf)))
                                        {
                                            int flag1 = 0;
                                            int flag2 = 0;

                                            foreach (Stoyak st3 in SP_St)
                                            {
                                                if (st1.famins.Id == st3.famins.Id)
                                                {
                                                    flag1 = 1;
                                                }

                                                if (st2.famins.Id == st3.famins.Id)
                                                {
                                                    flag2 = 1;
                                                }
                                            }                                            


                                            if ((flag1 == 0) && (flag2 == 0))
                                            {
                                                List<Stoyak> sppstt = new List<Stoyak>();

                                                sppstt.Add(st1);
                                                sppstt.Add(st2);

                                                if (D_Vse_StoYak.Count == 0)
                                                {
                                                    NaytiVseStoyaki();
                                                }

                                                Stoyak Stnign = null;
                                                Stoyak STverchn = null;

                                                int flag_ch = 0;

                                                if (st1.otm_plan_st == st2.otm_plan_st)
                                                {
                                                    flag_ch = 1;
                                                }

                                                if (flag_ch == 0)
                                                {
                                                    if (st1.otm_plan_st <= st2.otm_plan_st)
                                                    {
                                                        Stnign = st1;
                                                        STverchn = st2;
                                                    }

                                                    else
                                                    {
                                                        Stnign = st2;
                                                        STverchn = st1;

                                                    }

                                                    List<Stoyak> spst = new List<Stoyak>();
                                                    {
                                                        foreach (Stoyak stt in D_Vse_StoYak)
                                                        {
                                                            if ((stt.famins.Id.IntegerValue != st1.famins.Id.IntegerValue)
                                                                && (stt.famins.Id.IntegerValue != st2.famins.Id.IntegerValue)
                                                                && (st1.NazvIdentIf == stt.NazvIdentIf))
                                                            {
                                                                spst.Add(stt);
                                                            }
                                                        }

                                                        if (spst.Count > 0)
                                                        {
                                                            foreach (Stoyak stt1 in spst)
                                                            {
                                                                if ((stt1.otm_plan_st >= Stnign.otm_plan_st) && (stt1.otm_plan_st <= STverchn.otm_plan_st))
                                                                {
                                                                    sppstt.Add(stt1);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                foreach (Stoyak stt11 in sppstt)
                                                {
                                                    Provod_Graff PrSt = new Provod_Graff(stt11.famins);

                                                    newCepochkaGraf.Add(PrSt);

                                                    SP_St.Add(stt11);
                                                }
                                            }

                                            if ((flag1 == 0) && (flag2 != 0))
                                            {
                                                Provod_Graff PrSt = new Provod_Graff(st1.famins);

                                                newCepochkaGraf.Add(PrSt);

                                                SP_St.Add(st1);
                                            }

                                            if ((flag1 != 0) && (flag2 == 0))
                                            {
                                                Provod_Graff PrSt = new Provod_Graff(st2.famins);

                                                newCepochkaGraf.Add(PrSt);

                                                SP_St.Add(st2);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                catch { }                            
            }

            return newCepochkaGraf;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////    





        

    }
}
