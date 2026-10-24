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

    internal class Command_Postr_Grafa : IExternalCommand
    {
       public Result Execute(
       ExternalCommandData commandData,
       ref string message,
       ElementSet elements)

       {
            load(commandData);

            int id1 = 1330152;

            int id2 = 2168485;

            int id3 = 2173970;            

            List<int> SpInt = new List<int> { id1, id2 , id3}; //id4, id5 };

            List<ViewPlan> SpPlan = new List<ViewPlan>();

            foreach (int id in SpInt) 
            {
                ViewPlan vp = doc.GetElement(new ElementId(id)) as ViewPlan;

                SpPlan.Add(vp);
            }

            // TaskDialog.Show("Количество видов изначально", SpPlan.Count.ToString());

            Uzn_VSEGrVidi_Provod_Graf(SpPlan);

           // TaskDialog.Show("Все провода", D_vse_provoda_vse_vidi.Count.ToString());


            // 2333704

            // 2333935


            //2333389
            //int id_isk = 2333059;

            //Provod_Graff prisk = null;


            //foreach (var pr in D_vse_provoda_vse_vidi)
            //{
            //    if (pr.Pr_family_ins.Id.IntegerValue == id_isk)
            //    {
            //        prisk = pr;
            //        break;
            //    }
            //}

            //var kon1 = prisk.Sp_Point[0];
            //var pr_pr1 = kon1.Prov_graff_k_koncu[0];
            //var kon11 = kon1.Konci_graff_k_koncu[0];
            //var pr_pr11 = kon11.provod_graff_Host;


            //var kon2 = prisk.Sp_Point[1];
            //var pr_pr2 = kon2.Prov_graff_k_koncu[0];
            //var kon21 = kon2.Konci_graff_k_koncu[0];
            //var pr_pr21 = kon21.provod_graff_Host;




            //TaskDialog.Show("Провод к концу1 ", pr_pr1.Pr_family_ins.Id.IntegerValue.ToString());

            //TaskDialog.Show("Провод к концу2 ", pr_pr2.Pr_family_ins.Id.IntegerValue.ToString());


            //TaskDialog.Show("Конец к концу1 ", pr_pr11.Id.IntegerValue.ToString());
            //TaskDialog.Show("Конец к концу2 ", pr_pr21.Id.IntegerValue.ToString());


            // TaskDialog.Show("Все провода", D_vse_fam_Potrebit_na_vsech_Vidach.Count.ToString()); 

            //D_vse_fam_Potrebit_na_vsech_Vidach

            // 2334223

            FamilyInstance Fa1 = doc.GetElement(new ElementId(2261618)) as FamilyInstance;
            FamilyInstance Fa2 = doc.GetElement(new ElementId(2334223)) as FamilyInstance;           

            var spgr = Nayti_Cepochku_Vegdu_2_El_na_nesk_vidov(Fa1, Fa2);

            TaskDialog.Show("Цепочка", spgr.Count.ToString());

            string str = string.Empty;

            foreach (var pr in spgr)
            {
                str = str + "\n;" + pr.Pr_family_ins.Id.ToString();
            }

            TaskDialog.Show("Цепочка", str);


            //List<Provod_Graff> connect_ELs_nach = naitinach_ikonec_cep(D_vse_provoda_vse_vidi, Fa1);

            // TaskDialog.Show("Начальный провод", connect_ELs_nach[0].Pr_family_ins.Id.IntegerValue.ToString());

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //List<Provod_Graff> isk_cep = new List<Provod_Graff>();

            //List<Provod_Graff> connect_ELs_nach = naitinach_ikonec_cep(D_vse_provoda_vse_vidi, Fa1);

            //foreach (Provod_Graff ell in connect_ELs_nach)
            //{

            //    if (isk_cep.Count > 0)
            //    {
            //        break;
            //    }

            //    List<List<Provod_Graff>> VR_SPcep_podkl = new List<List<Provod_Graff>>();
            //    List<Provod_Graff> SPtriniki = new List<Provod_Graff>(); ////////////////////// 
            //    List<Provod_Graff> SPeltriniki = new List<Provod_Graff>();
            //    List<Provod_Graff> cep_podkl = new List<Provod_Graff>();

            //    Provod_Graff el_n = ell;

            //    foreach (var prov in connect_ELs_nach)
            //    {

            //        if (prov.Pr_family_ins.Id.IntegerValue != el_n.Pr_family_ins.Id.IntegerValue)

            //        {
            //            SPeltriniki.Add(prov);

            //        }
            //    }

            //    int flag_postr_gr = 0;

            //    while (flag_postr_gr == 0)

            //    {
            //        if (isk_cep.Count > 0)
            //        {
            //            break;
            //        }

            //        List<Provod_Graff> connect_ELs = el_n.Prisoed_Pr;

            //        if (SPeltriniki.Any())

            //        {
            //            List<Provod_Graff> connect_ELs1 = new List<Provod_Graff>();

            //            foreach (Provod_Graff podkl_el in connect_ELs)
            //            {
            //                int flag1 = 0;

            //                foreach (Provod_Graff podkl_el1 in SPeltriniki)
            //                {
            //                    if (podkl_el.Pr_family_ins.Id == podkl_el1.Pr_family_ins.Id)

            //                    {
            //                        flag1 = 1;
            //                    }
            //                }

            //                if (flag1 == 0)
            //                {
            //                    connect_ELs1.Add(podkl_el);
            //                }
            //            }

            //            connect_ELs = connect_ELs1;
            //        }


            //        if (connect_ELs.Count == 1)
            //        {
            //            cep_podkl.Add(el_n);
            //            SPeltriniki.Add(el_n);
            //            el_n = connect_ELs[0];
            //        }


            //        if (connect_ELs.Count > 1)
            //        {
            //            foreach (var pr in connect_ELs)
            //            {

            //                List<Provod_Graff> ELs_Els = new List<Provod_Graff>();

            //                foreach (var pr2 in pr.Prisoed_Pr)
            //                {
            //                    int rav = 0;

            //                    foreach (var pr3 in connect_ELs)
            //                    {

            //                        if (pr2.Pr_family_ins.Id.IntegerValue == pr3.Pr_family_ins.Id.IntegerValue)
            //                        {
            //                            rav = 1;
            //                            break;
            //                        }
            //                    }

            //                    if (rav == 0)
            //                    {
            //                        ELs_Els.Add(pr2);


            //                    }
            //                }


            //                pr.Prisoed_Pr = ELs_Els;
            //            }

            //            SPtriniki.Add(el_n);
            //            SPeltriniki.Add(el_n);
            //            SPeltriniki.Add(connect_ELs[0]);


            //            List<Provod_Graff> cep_podkl1 = new List<Provod_Graff>();

            //            foreach (Provod_Graff el in cep_podkl)
            //            {
            //                cep_podkl1.Add(el);

            //            }

            //            VR_SPcep_podkl.Add(cep_podkl1);
            //            cep_podkl.Add(el_n);
            //            el_n = connect_ELs[0];

            //        }

            //        if ((connect_ELs.Count == 0) || (connect_ELs == null))

            //        {
            //            cep_podkl.Add(el_n);

            //            // SPcep_podkl.Add(cep_podkl);

            //            isk_cep = Videlit_Nuzn_Podcepochku(cep_podkl, Fa2);

            //            if (isk_cep.Count > 0)
            //            {
            //                break;
            //            }

            //            if
            //                (VR_SPcep_podkl.Count != 0)
            //            {

            //                el_n = SPtriniki[0];
            //                cep_podkl = new List<Provod_Graff>();

            //                foreach (Provod_Graff el in VR_SPcep_podkl[0])
            //                {
            //                    cep_podkl.Add(el);
            //                }

            //                SPtriniki.RemoveAt(0);
            //                VR_SPcep_podkl.RemoveAt(0);

            //            }

            //            else
            //            {
            //                flag_postr_gr = 1;
            //                break;
            //            }
            //        }
            //    }
            //}

            //foreach (var pr in D_vse_provoda_vse_vidi)
            //{
            //    pr.Zapis_IZ_Zapasn();
            //}

            //TaskDialog.Show("Цепочка", isk_cep.Count.ToString());

            //string str1 = string.Empty;

            //foreach (var pr in isk_cep)
            //{
            //    str1 = str1 + "\n;" + pr.Pr_family_ins.Id.ToString();
            //}

            //TaskDialog.Show("Цепочка", str1);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //TaskDialog.Show("Длина графа ", spgr.Count.ToString());

            //TaskDialog.Show ("Id 1го " , spgr[0].Pr_family_ins.Id.ToString());

            //TaskDialog.Show("Id 2го ", spgr[1].Pr_family_ins.Id.ToString());


            //foreach (var pr in D_vse_provoda_vse_vidi)
            //{
            //    if (pr.Pr_family_ins.Id.IntegerValue == 2334321)
            //    {
            //        List<string> SpStr = new List<string>();

            //        foreach (var podkl in pr.Prisoed_Pr)
            //        {
            //            SpStr.Add(podkl.Pr_family_ins.Id.IntegerValue.ToString());
            //        }

            //        string ss = string.Join(",", SpStr);

            //        TaskDialog.Show("Подключенные провода", ss);

            //        int i = 1;
            //        foreach (var kon in pr.Sp_Point)
            //        {
            //            var sp_prr = kon.Prov_graff_k_koncu;

            //            string strrrr = string.Empty;

            //            foreach (var prrr in sp_prr)
            //            {
            //                strrrr = strrrr + prrr.Pr_family_ins.Id.IntegerValue.ToString();
            //            }

            //            TaskDialog.Show("К концу", i.ToString() + " конец - " + strrrr);

            //            i++;

            //            TaskDialog.Show("Тип подключения", kon.typ_konec_podkl.ToString());

            //        }

            //    }

            //}



            //GetVisibleFamilyInstances_Potrebit();            

            //GetVisibleFamilyInstances_Provoda();         

            //int flag = 0;

            //while (flag == 0)
            //{
            //     uznaem_typ_podkl_konc();

            //     List<Provod_Graff> SpPr = new List<Provod_Graff>();

            //     foreach (var pr in D_vse_provoda)
            //     {
            //            if ((pr.Sp_Point[0].typ_konec_podkl > 0) && (pr.Sp_Point[1].typ_konec_podkl > 0))
            //            {
            //                SpPr.Add(pr);
            //            }
            //     }

            //     if (SpPr.Count < D_vse_provoda.Count)
            //     {
            //            D_vse_provoda = SpPr;

            //     }

            //     else 
            //     {
            //            break;
            //     }                    
            //}


            //Orabotat_Prisoedinenia(); // Здесь выясним присоединенные провода к каждому проводу.


            // 2270705 2275598

            //FamilyInstance Fa1 = doc.GetElement(new ElementId(2270718)) as FamilyInstance;
            //FamilyInstance Fa2 = doc.GetElement(new ElementId(2275598)) as FamilyInstance;
            //FamilyInstance Fa3 = doc.GetElement(new ElementId(2270748)) as FamilyInstance;

            //List <FamilyInstance> SpFam = new List <FamilyInstance>();

            //SpFam.Add(Fa1);

            //SpFam.Add(Fa2);

            //SpFam.Add(Fa3);

            //List<List<Provod_Graff>> sppceepp = Nayti_Vse_Cepochku_Ravnoznacn(SpFam);


            //TaskDialog.Show("Столько цепочек ", sppceepp.Count.ToString());

            //List<Provod_Graff> isk_cep1 = sppceepp[1];

            //if (isk_cep1.Count > 0)
            //{
            //    string str = string.Empty;

            //    foreach (var pr in isk_cep1)

            //    {
            //        str = str + pr.Pr_family_ins.Id.ToString() + "; ";
            //    }

            //    TaskDialog.Show("Элементы ", str);
            //}






            //List<Provod_Graff> isk_cep = new List<Provod_Graff>();

            //List<Provod_Graff> connect_ELs_nach = naitinach_ikonec_cep(D_vse_provoda, Fa1);


            //foreach (Provod_Graff ell in connect_ELs_nach)
            //{

            //    if (isk_cep.Count > 0)
            //    {
            //        break;
            //    }

            //    List<List<Provod_Graff>> VR_SPcep_podkl = new List<List<Provod_Graff>>();
            //    List<Provod_Graff> SPtriniki = new List<Provod_Graff>(); ////////////////////// 
            //    List<Provod_Graff> SPeltriniki = new List<Provod_Graff>();
            //    List<Provod_Graff> cep_podkl = new List<Provod_Graff>();

            //    Provod_Graff el_n = ell;

            //    foreach (var prov in connect_ELs_nach)

            //        if (prov.Pr_family_ins.Id.IntegerValue != el_n.Pr_family_ins.Id.IntegerValue)

            //        {
            //            SPeltriniki.Add(prov);

            //        }

            //    int flag_postr_gr = 0;

            //    while (flag_postr_gr == 0)

            //    {
            //        if (isk_cep.Count > 0)
            //        {
            //            break;
            //        }

            //        List<Provod_Graff> connect_ELs = el_n.Prisoed_Pr;

            //        if (SPeltriniki.Any())

            //        {
            //            List<Provod_Graff> connect_ELs1 = new List<Provod_Graff>();

            //            foreach (Provod_Graff podkl_el in connect_ELs)
            //            {
            //                int flag1 = 0;

            //                foreach (Provod_Graff podkl_el1 in SPeltriniki)
            //                {
            //                    if (podkl_el.Pr_family_ins.Id == podkl_el1.Pr_family_ins.Id)

            //                    {
            //                        flag1 = 1;
            //                    }
            //                }

            //                if (flag1 == 0)
            //                {
            //                    connect_ELs1.Add(podkl_el);
            //                }
            //            }

            //            connect_ELs = connect_ELs1;
            //        }


            //        if (connect_ELs.Count == 1)
            //        {
            //            cep_podkl.Add(el_n);
            //            SPeltriniki.Add(el_n);
            //            el_n = connect_ELs[0];
            //        }


            //        if (connect_ELs.Count > 1)
            //        {
            //            foreach (var pr in connect_ELs)
            //            {

            //                List<Provod_Graff> ELs_Els = new List<Provod_Graff>();

            //                foreach (var pr2 in pr.Prisoed_Pr)
            //                {
            //                    int rav = 0;

            //                    foreach (var pr3 in connect_ELs)
            //                    {

            //                        if (pr2.Pr_family_ins.Id.IntegerValue == pr3.Pr_family_ins.Id.IntegerValue)
            //                        {
            //                            rav = 1;
            //                            break;
            //                        }
            //                    }

            //                    if (rav == 0)
            //                    {
            //                        ELs_Els.Add(pr2);


            //                    }
            //                }


            //                pr.Prisoed_Pr = ELs_Els;
            //            }

            //            SPtriniki.Add(el_n);
            //            SPeltriniki.Add(el_n);
            //            SPeltriniki.Add(connect_ELs[0]);


            //            List<Provod_Graff> cep_podkl1 = new List<Provod_Graff>();

            //            foreach (Provod_Graff el in cep_podkl)
            //            {
            //                cep_podkl1.Add(el);

            //            }

            //            VR_SPcep_podkl.Add(cep_podkl1);
            //            cep_podkl.Add(el_n);
            //            el_n = connect_ELs[0];

            //        }

            //        if ((connect_ELs.Count == 0) || (connect_ELs == null))

            //        {
            //            cep_podkl.Add(el_n);

            //            // SPcep_podkl.Add(cep_podkl);

            //            isk_cep = Videlit_Nuzn_Podcepochku(cep_podkl, Fa2);

            //            if (isk_cep.Count > 0)
            //            {
            //                break;
            //            }

            //            if
            //                (VR_SPcep_podkl.Count != 0)
            //            {

            //                el_n = SPtriniki[0];
            //                cep_podkl = new List<Provod_Graff>();

            //                foreach (Provod_Graff el in VR_SPcep_podkl[0])
            //                {
            //                    cep_podkl.Add(el);
            //                }

            //                SPtriniki.RemoveAt(0);
            //                VR_SPcep_podkl.RemoveAt(0);

            //            }

            //            else
            //            {
            //                flag_postr_gr = 1;
            //                break;
            //            }

            //        }
            //    }
            //}


            //if (isk_cep1.Count > 0)
            //{                
            //        string str = string.Empty;

            //        foreach (var pr in isk_cep1)

            //        {
            //           str = str + pr.Pr_family_ins.Id.ToString() + "; ";
            //        }

            //        TaskDialog.Show("Элементы ", str);               
            //}

            //foreach (var pr in D_vse_provoda)             
            //{
            //    pr.Zapis_IZ_Zapasn();
            //}

            return Result.Succeeded;
       }
    }
}


