using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using LKS_DEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static LKS_DEC.Data;
using static LKS_DEC.DopFunc;
using static LKS_DEC.MetaDan_fuction;
using static LKS_DEC.DopFunc_Postr_graf;
using static LKS_DEC.Vse_Classi;
using System.Reflection;


namespace LKS_DEC
{
    public class Sem_Postr_Graff
    {
        public class Potrebit_Graff
        {
            public FamilyInstance fam_potr;

            public XYZ Ust_Potr;


            public Potrebit_Graff(FamilyInstance _fam_potr)
            {
                fam_potr = _fam_potr;

                Ust_Potr = null;

                try
                {
                    Ust_Potr = GetFirstConnectorLocation(fam_potr);
                }
                catch { }
            }           
        }

        public class Provod_Graff
        {
            public FamilyInstance Pr_family_ins;

            public List<Konec_Provod_Graff> Sp_Point = new List<Konec_Provod_Graff>();

            public int typ_svobodn_konc;

            public List<Provod_Graff> Prisoed_Pr = new List<Provod_Graff>();

            public List<Provod_Graff> Prisoed_Pr_Zapasn = new List<Provod_Graff>();

            public List<Stoyak> Prisoed_Stoyak = new List<Stoyak>();

            public int typ_provod_ili_stoyak = 0; // если 1 то стояк

            //public Provod_Graff(FamilyInstance _Pr_family_ins)  // Скорее всего это устарело
            //{
            //    Pr_family_ins = _Pr_family_ins;

            //    XYZ p1 = ZeroZCoordinatePoint((Pr_family_ins.Location as LocationCurve).Curve.GetEndPoint(0));

            //    XYZ p2 = ZeroZCoordinatePoint((Pr_family_ins.Location as LocationCurve).Curve.GetEndPoint(1));

            //    Konec_Provod_Graff kon_p1 = new Konec_Provod_Graff(Pr_family_ins, p1);

            //    Konec_Provod_Graff kon_p2 = new Konec_Provod_Graff(Pr_family_ins, p2);

            //    Sp_Point = new List<Konec_Provod_Graff> { kon_p1, kon_p2 };
            //}

            public Provod_Graff(FamilyInstance _Pr_family_ins, List<Potrebit_Graff> SpPotrebitGr , List<Stoyak> Spst)
            {
                Pr_family_ins = _Pr_family_ins;

                XYZ p1 = ZeroZCoordinatePoint((Pr_family_ins.Location as LocationCurve).Curve.GetEndPoint(0));

                XYZ p2 = ZeroZCoordinatePoint((Pr_family_ins.Location as LocationCurve).Curve.GetEndPoint(1));

                Konec_Provod_Graff kon_p1 = new Konec_Provod_Graff(Pr_family_ins, p1 , SpPotrebitGr,  Spst);

                Konec_Provod_Graff kon_p2 = new Konec_Provod_Graff(Pr_family_ins, p2 ,  SpPotrebitGr,  Spst);

                Sp_Point = new List<Konec_Provod_Graff> { kon_p1, kon_p2 };

            }

            public Provod_Graff(FamilyInstance _Pr_family_ins)
            {
                Pr_family_ins = _Pr_family_ins;

                typ_provod_ili_stoyak = 1;
            }

            public int sodergit_li_elem(FamilyInstance fam_isk)
            {
                int flag = 0;

                foreach (var kon in Sp_Point)
                {
                    FamilyInstance famprov = null;

                    try
                    {
                        famprov = kon.Fam_podkluch;
                    }

                    catch { } 
                    
                    if (famprov != null)
                    {
                        if (famprov.Id.IntegerValue == fam_isk.Id.IntegerValue)
                        {
                            flag = 1;
                        }
                    }
                }

                return flag;
            }

            public void Uzn_Podkl_Pr(List<Provod_Graff> NachViborkaProvodov)
            {
                List<Provod_Graff> sp_itog = new List<Provod_Graff>();

                Prisoed_Pr.Clear(); 

                foreach (var kon in Sp_Point)
                {
                    try
                    {
                        kon.Uzn_Podkl_Pr(NachViborkaProvodov);

                        List<Provod_Graff> sp1 = kon.Prov_graff_k_koncu;

                        if (sp1.Count > 0)
                        {
                            sp_itog.AddRange(sp1);
                        }
                    }
                    catch { }   
                }

                Prisoed_Pr = sp_itog;

                Zapis_V_Zapasn();
            }

            public void Uzn_Podkl_Pr_Po_vidu(Graff_Plan_View graff_Plan_View )
            {
                List<Provod_Graff> NachViborkaProvodov = graff_Plan_View.SpProvGr;

                List<Provod_Graff> sp_itog = new List<Provod_Graff>();

                Prisoed_Pr.Clear();

                foreach (var kon in Sp_Point)
                {
                    try
                    {
                        kon.Uzn_Podkl_Pr(NachViborkaProvodov);

                        List<Provod_Graff> sp1 = kon.Prov_graff_k_koncu;

                        if (sp1.Count > 0)
                        {
                            sp_itog.AddRange(sp1);
                        }
                    }
                    catch { }

                    //try
                    //{
                    //    kon.Uzn_Podkl_Konci_Cerez_Stoyak();

                    //    List<Provod_Graff> sp2 = kon.Prov_graff_k_koncu;

                    //    if (sp2.Count > 0)
                    //    {
                    //        sp_itog.AddRange(sp2);
                    //    }
                    //}
                    //catch { }
                }

                Prisoed_Pr = sp_itog;

                Zapis_V_Zapasn();
            }

            public void Uzn_Podkl_Pr_S_Ucetom_StoYak()
            {
                int flag = 0;

                foreach (var kon in Sp_Point)
                {
                    try
                    {
                        if (kon.Podkl_Stoyak != null)
                        {
                            kon.Uzn_Podkl_Konci_Cerez_Stoyak();

                            List<Provod_Graff> sp2 = kon.Prov_graff_k_koncu;

                            if (sp2.Count > 0)
                            {
                                Prisoed_Pr.AddRange(sp2);
                            }

                            flag = 1;   
                        }
                    }
                    catch { }

                }

                if (flag == 1)
                {
                    Zapis_V_Zapasn();
                }
            }


            public void Zapis_V_Zapasn()
            {
                Prisoed_Pr_Zapasn.Clear();

                if (Prisoed_Pr.Count > 0)
                {
                    foreach (var pr in Prisoed_Pr)
                    {
                        Prisoed_Pr_Zapasn.Add(pr);
                    }

                }        
            }

            public void Zapis_IZ_Zapasn()
            {
                Prisoed_Pr.Clear();

                if (Prisoed_Pr_Zapasn.Count > 0)
                {
                    foreach (var pr in Prisoed_Pr_Zapasn)
                    {
                        Prisoed_Pr.Add(pr);
                    }

                }
            }

            public void Uzn_Stoyak()
            {
                foreach (Konec_Provod_Graff kon in Sp_Point)
                {
                    if (kon.Podkl_Stoyak != null)
                    {
                        Prisoed_Stoyak.Add(kon.Podkl_Stoyak);
                    }                
                }        
            
            }
        }

        public class Konec_Provod_Graff
        {
            public FamilyInstance provod_graff_Host;

            public XYZ point;

            public List<Konec_Provod_Graff> Konci_graff_k_koncu = new List<Konec_Provod_Graff>();

            public List<Provod_Graff> Prov_graff_k_koncu = new List<Provod_Graff> ();

            public int typ_konec_podkl = 0;

            public FamilyInstance Fam_podkluch = null;

            public Stoyak Podkl_Stoyak = null;

            //public Konec_Provod_Graff(FamilyInstance _provod_graff_Host, XYZ _point)
            //{
            //    provod_graff_Host = _provod_graff_Host;

            //    point = _point;

            //    Uzn_Potr();
            //}

            public Konec_Provod_Graff(FamilyInstance _provod_graff_Host, XYZ _point , List<Potrebit_Graff> SpPotrebitGr, List<Stoyak> Spst)
            {
                provod_graff_Host = _provod_graff_Host;

                point = _point;

                Uzn_Potr_Na_Vide( SpPotrebitGr);

                Uzn_Stoyak_Na_Vide( Spst);
            }


            //public void Uzn_Potr()
            //{
            //    Fam_podkluch = null;

            //    foreach (Potrebit_Graff fam in D_vse_fam_Potrebit_na_Vide)
            //    {
            //        if (IsEqualTo_Point_Con(point, fam.Ust_Potr))
            //        {

            //            if (IsPointInElem(fam.fam_potr, point))
            //            {
            //                Fam_podkluch = fam.fam_potr;

            //                break;
            //            }
            //        }
            //    }
            //}

            public void Uzn_Potr_Na_Vide(List<Potrebit_Graff> SpPotrebitGr)
            {
                Fam_podkluch = null;                

                foreach ( var fam in SpPotrebitGr)
                {                   

                    if (IsEqualTo_Point_Con(point, fam.Ust_Potr ))
                    {

                        if (IsPointInElem(fam.fam_potr, point))
                        {
                            Fam_podkluch = fam.fam_potr;

                            break;
                        }
                    }
                }
            }


            public void Uzn_Stoyak_Na_Vide(List<Stoyak> Spst)
            {
                Podkl_Stoyak = null;

                foreach (Stoyak st in Spst)
                {
                    if  (IsEqualTo_Point_Stoyak( point, st.famins))
                    {
                        Podkl_Stoyak = st;
                        break;  
                    }
                }
            }           


            public void Uzn_Podkl_Konci_NaVide(Graff_Plan_View graff_Plan_View)
            {
                Konci_graff_k_koncu.Clear();

                if (Fam_podkluch == null)
                {
                    foreach (Provod_Graff fam in graff_Plan_View.SpProvGr)
                    {
                        if (fam.Pr_family_ins.Id.IntegerValue != provod_graff_Host.Id.IntegerValue)
                        {
                            var spkonc = fam.Sp_Point;

                            foreach (var kon in spkonc)
                            {
                                if (kon.Fam_podkluch == null)
                                {
                                    if (IsEqualTo_Point(point, kon.point))
                                    {
                                        Konci_graff_k_koncu.Add(kon);
                                    }
                                }
                            }
                        }
                    }
                }

                else
                {
                    foreach (Provod_Graff fam in graff_Plan_View.SpProvGr)
                    {
                        if (fam.Pr_family_ins.Id.IntegerValue != provod_graff_Host.Id.IntegerValue)
                        {
                            var spkonc = fam.Sp_Point;

                            foreach (var kon in spkonc)
                            {
                                if (kon.Fam_podkluch != null)
                                {
                                    if (Fam_podkluch.Id.IntegerValue == kon.Fam_podkluch.Id.IntegerValue)
                                    {
                                        Konci_graff_k_koncu.Add(kon);
                                    }
                                }
                            }
                        }
                    }
                }

                if ((Konci_graff_k_koncu.Count == 0) && (Fam_podkluch == null))
                {
                    typ_konec_podkl = 0;
                }

                if (Konci_graff_k_koncu.Count == 1)
                {
                    typ_konec_podkl = 1;
                }

                if (Konci_graff_k_koncu.Count > 1)
                {
                    typ_konec_podkl = 2;
                }

                if ((Konci_graff_k_koncu.Count == 0) && (Fam_podkluch != null))  // оконечка
                {
                    typ_konec_podkl = 3;
                }
            }

            public void Uzn_Podkl_Konci_Cerez_Stoyak()
            {
                if (Podkl_Stoyak != null)
                {
                    var sp_st = Podkl_Stoyak.SpSvyaz;

                    if (sp_st.Count > 0)
                    {
                        foreach (var st1 in sp_st)
                        {
                            foreach (var pr in D_vse_provoda_vse_vidi)
                            {
                                foreach (var kon in pr.Sp_Point)
                                {
                                    if (kon.Podkl_Stoyak != null)
                                    {
                                        if (st1.famins.Id.IntegerValue == kon.Podkl_Stoyak.famins.Id.IntegerValue)
                                        {
                                            Konci_graff_k_koncu.Add(kon);
                                            Prov_graff_k_koncu.Add(pr);
                                        }
                                    }
                                }
                            }
                        }                    
                    }
                }            
            }


            //public void Uzn_Podkl_Konci()
            //{
            //    Konci_graff_k_koncu.Clear();

            //    if (Fam_podkluch == null)
            //    {
            //        foreach (Provod_Graff fam in D_vse_provoda)
            //        {
            //            if (fam.Pr_family_ins.Id.IntegerValue != provod_graff_Host.Id.IntegerValue)
            //            {
            //                var spkonc = fam.Sp_Point;

            //                foreach (var kon in spkonc)
            //                {
            //                    if (kon.Fam_podkluch == null)
            //                    {
            //                        if (IsEqualTo_Point(point, kon.point))
            //                        {
            //                            Konci_graff_k_koncu.Add(kon);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    else
            //    {
            //        foreach (Provod_Graff fam in D_vse_provoda)
            //        {
            //            if (fam.Pr_family_ins.Id.IntegerValue != provod_graff_Host.Id.IntegerValue)
            //            {
            //                var spkonc = fam.Sp_Point;

            //                foreach (var kon in spkonc)
            //                {
            //                    if (kon.Fam_podkluch != null)
            //                    {
            //                        if (Fam_podkluch.Id.IntegerValue == kon.Fam_podkluch.Id.IntegerValue)
            //                        {
            //                            Konci_graff_k_koncu.Add(kon);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    if ((Konci_graff_k_koncu.Count == 0) && (Fam_podkluch == null))
            //    {
            //        typ_konec_podkl = 0;
            //    }

            //    if (Konci_graff_k_koncu.Count == 1)
            //    {
            //        typ_konec_podkl = 1;
            //    }

            //    if (Konci_graff_k_koncu.Count > 1) 
            //    {
            //        typ_konec_podkl = 2;
            //    }

            //    if ((Konci_graff_k_koncu.Count == 0) && (Fam_podkluch != null))  // оконечка
            //    {
            //        typ_konec_podkl = 3;
            //    }

                
            //}


            public void Uzn_Podkl_Konci_vsech_vidov()
            {                
                        if ((Konci_graff_k_koncu.Count == 0) && (Fam_podkluch == null))
                        {
                            typ_konec_podkl = 0;
                        }

                        if (Konci_graff_k_koncu.Count == 1)
                        {
                            typ_konec_podkl = 1;
                        }

                        if (Konci_graff_k_koncu.Count > 1)
                        {
                            typ_konec_podkl = 2;
                        }

                        if ((Konci_graff_k_koncu.Count == 0) && (Fam_podkluch != null))  // оконечка
                        {
                            typ_konec_podkl = 3;
                        }
            }


            public void Uzn_Podkl_Konci_na_Vide( Graff_Plan_View graff_Plan_View )
            {
                Konci_graff_k_koncu.Clear();

                if (Fam_podkluch == null)
                {
                    foreach (Provod_Graff fam in graff_Plan_View.SpProvGr)
                    {
                        if (fam.Pr_family_ins.Id.IntegerValue != provod_graff_Host.Id.IntegerValue)
                        {
                            var spkonc = fam.Sp_Point;

                            foreach (var kon in spkonc)
                            {
                                if (kon.Fam_podkluch == null)
                                {
                                    if (IsEqualTo_Point(point, kon.point))
                                    {
                                        Konci_graff_k_koncu.Add(kon);
                                    }
                                }
                            }
                        }
                    }
                }

                else
                {
                    foreach (Provod_Graff fam in graff_Plan_View.SpProvGr)
                    {
                        if (fam.Pr_family_ins.Id.IntegerValue != provod_graff_Host.Id.IntegerValue)
                        {
                            var spkonc = fam.Sp_Point;

                            foreach (var kon in spkonc)
                            {
                                if (kon.Fam_podkluch != null)
                                {
                                    if (Fam_podkluch.Id.IntegerValue == kon.Fam_podkluch.Id.IntegerValue)
                                    {
                                        Konci_graff_k_koncu.Add(kon);
                                    }
                                }
                            }
                        }
                    }
                }

                if ((Konci_graff_k_koncu.Count == 0) && (Fam_podkluch == null))
                {
                    typ_konec_podkl = 0;
                }

                if (Konci_graff_k_koncu.Count == 1)
                {
                    typ_konec_podkl = 1;
                }

                if (Konci_graff_k_koncu.Count > 1)
                {
                    typ_konec_podkl = 2;
                }

                if ((Konci_graff_k_koncu.Count == 0) && (Fam_podkluch != null))  // оконечка
                {
                    typ_konec_podkl = 3;
                }


            }


            public void Uzn_Podkl_Pr( List<Provod_Graff> NachViborkaProvodov)
            {
                if (NachViborkaProvodov.Count > 0)
                {
                    foreach (var kon in Konci_graff_k_koncu)
                    {
                        var fam = kon.provod_graff_Host;

                        foreach (var pr in NachViborkaProvodov)
                        {
                            if (fam.Id.IntegerValue == pr.Pr_family_ins.Id.IntegerValue)
                            {
                                Prov_graff_k_koncu.Add(pr);
                            }                        
                        }
                    }              
                }              
            }

            //public void Uzn_Podkl_Pr_Po_Vidu(Graff_Plan_View graff_Plan_View)
            //{
            //    List<Provod_Graff> NachViborkaProvodov = graff_Plan_View.SpProvGr;

            //    if (NachViborkaProvodov.Count > 0)
            //    {
            //        foreach (var kon in Konci_graff_k_koncu)
            //        {
            //            var fam = kon.provod_graff_Host;

            //            foreach (var pr in NachViborkaProvodov)
            //            {
            //                if (fam.Id.IntegerValue == pr.Pr_family_ins.Id.IntegerValue)
            //                {
            //                    Prov_graff_k_koncu.Add(pr);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public class Uzel_Provod_Graff
        {       

            public List<Konec_Provod_Graff> provod_graff_konnect = new List<Konec_Provod_Graff>();

            public Uzel_Provod_Graff()
            {
                 
            }
        }

        public class Cepochka_Provodov_Graff
        {
            public List<Provod_Graff> Sp_Pr_Cep = new List<Provod_Graff>();

            public Konec_Provod_Graff point_kr1 = null;
            public Konec_Provod_Graff point_kr2 = null;

            public int Index; // Это индекс в исходном списке

            public int typ_cepochki = 0; // Это тип. Если цепочка упирается в потребитель то 1  Если цепочка находится между тройниками то 2

            public Cepochka_Provodov_Graff()
            {

            }
            public void NaitiKonci()
            {
                point_kr1 = null;
                point_kr2 = null;

                if (Sp_Pr_Cep.Count > 1)
                {
                     Provod_Graff elgr_nach = Sp_Pr_Cep[0];

                     var kon01 = elgr_nach.Sp_Point[0];
                     var kon02 = elgr_nach.Sp_Point[1];

                    if (kon01.typ_konec_podkl == 3)
                    {
                        point_kr1 = kon01;
                    }

                    if (kon02.typ_konec_podkl == 3)
                    {
                        point_kr1 = kon02;
                    }

                    if (point_kr1 == null)
                    {
                        {
                            var spp = kon01.Konci_graff_k_koncu;

                            if (spp.Count > 1)
                            {
                                point_kr1 = kon01;
                            }

                            else
                            {
                                point_kr1 = kon02;
                            }
                        }
                    }

                    Provod_Graff elgr_konechn = Sp_Pr_Cep.Last();

                    kon01 = elgr_konechn.Sp_Point[0];
                    kon02 = elgr_konechn.Sp_Point[1];

                    if (kon01.typ_konec_podkl == 3)
                    {
                        point_kr2 = kon01;
                    }

                    if (kon02.typ_konec_podkl == 3)
                    {
                        point_kr2 = kon02;
                    }


                    if (point_kr2 == null)
                    {
                        {
                            var spp = kon01.Konci_graff_k_koncu;

                            if (spp.Count > 1)
                            {
                                point_kr2 = kon01;
                            }

                            else
                            {
                                point_kr2 = kon02;
                            }
                        }
                    }
                }

                if (Sp_Pr_Cep.Count == 1)
                {
                    point_kr1 = Sp_Pr_Cep[0].Sp_Point[0];

                    point_kr2 = Sp_Pr_Cep[0].Sp_Point[1];
                }
            }

            public void Reverse_Cep()
            {
                Sp_Pr_Cep.Reverse();

                NaitiKonci();
            }

            public void Povernut_nacalom_V_Potrebit()
            {
                if ((point_kr1.typ_konec_podkl != 3) && (point_kr2.typ_konec_podkl == 3))
                {
                    Reverse_Cep(); 
                }
            }
        }

        public class Stoyak
        {
            public int flag_uch_prich = 0; // 1 уходит 2 приходит 3 уходит приходит  

            public FamilyInstance famins = null;

            public string NazvIdentIf = D_Pust_Znach; // Идентификатор для связи           

            public List<Stoyak> SpSvyaz = new List<Stoyak>(); // Список связанных семейств стояков

            public List<Stoyak> SpSvyazVse = new List<Stoyak>(); // Список всех стояков связанных

            public ViewPlan RoditViewPlan = null;

            public List<ViewPlan> SPViewPlan = new List<ViewPlan>();

            public List<ViewPlan> VSESPViewPlan = new List<ViewPlan>();

            public List<Konec_Provod_Graff> Sp_Konec = new List<Konec_Provod_Graff>();

            public List<Provod_Graff> Sp_Provod = new List<Provod_Graff>();

            public List<Provod_Graff> Vse_Sp_Provod = new List<Provod_Graff>();

            public List<ElectricalSystem> SP_ElSystStoyak = new List<ElectricalSystem>();

            public double otm_plan_st = 0;

            public Level level_st = null;

            public Stoyak(FamilyInstance _famins)
            {
                famins = _famins;

                Nach_dan();               
            }

            public void Nach_dan()
            {
                NazvIdentIf = famins.LookupParameter(D_NazvPar_Ident).AsString();

                ElementId ownerViewId = famins.OwnerViewId;

                // Проверяем, что ID вида действителен
                if (ownerViewId != ElementId.InvalidElementId)
                {
                    // Получаем элемент вида
                    Element viewElement = doc.GetElement(ownerViewId);

                    // Проверяем, является ли элемент видом типа ViewPlan
                    if (viewElement is ViewPlan)
                    {
                        RoditViewPlan = (ViewPlan)viewElement;
                    }
                }

                string imatyp = famins.Symbol.Name;

                if (imatyp.Contains(D_NazvTyp_Sroyak_Uchodit))
                {
                    flag_uch_prich = 1;
                }

                if (imatyp.Contains(D_NazvTyp_Sroyak_Prichodit))
                {
                    flag_uch_prich = 2;
                }

                if (imatyp.Contains(D_NazvTyp_Sroyak_Prichodit_Uchodit))
                {
                    flag_uch_prich = 3;
                }

                level_st = GetAssociatedLevel(RoditViewPlan);

                if (level_st != null)
                {
                    otm_plan_st = Math.Round(level_st.Elevation * 0.3048 , 2);
                } 

                nayti_ceps();
            }

            public void Dop_rash()
            {
                if (D_Vse_StoYak.Count == 0)
                {
                    NaytiVseStoyaki();
                }

                if (D_Vse_StoYak.Count > 0)
                {
                    foreach (var st in D_Vse_StoYak)
                    {
                        if (famins.Id.IntegerValue == st.famins.Id.IntegerValue)
                        {
                            SpSvyaz = st.SpSvyaz;
                            SpSvyazVse = st.SpSvyazVse;
                            SPViewPlan = st.SPViewPlan;
                            VSESPViewPlan = st.VSESPViewPlan;
                        }
                    }
                }                
            }


            public void nayti_ceps()
            {
                Graf_Infa_Cep graf_Infa_Cep = new Graf_Infa_Cep(famins);

                SP_ElSystStoyak = graf_Infa_Cep.Vse_systems;
            }

            private static Level GetAssociatedLevel(ViewPlan viewPlan)
            {
                Level associatedLevel = null;

                // Проверяем, что переданный ViewPlan не равен null
                if (viewPlan != null)
                {
                    // Получение связанного уровня
                    associatedLevel = viewPlan.GenLevel;
                }

                // Возвращаем уровень
                return associatedLevel;               

            }

        }

        public class Graff_Plan_View
        {
            public ViewPlan viewPlan = null;

            public List<FamilyInstance> SpPotrFamIns = new List<FamilyInstance>();

            public List<FamilyInstance> SpProvFamIns = new List<FamilyInstance>();

            // public List<FamilyInstance> SpStoyakFamIns = new List<FamilyInstance>();

            public List<Potrebit_Graff> SpPotrGr = new List<Potrebit_Graff>();

            public List<Provod_Graff> SpProvGr = new List<Provod_Graff>();

            public List<Stoyak> SPstoyak = new List<Stoyak>();

            public Graff_Plan_View(ViewPlan _viewPlan)
            {
                viewPlan = _viewPlan;

                try
                {

                    FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, viewPlan.Id);

                    // Фильтруем только FamilyInstance
                    SpProvFamIns = allElementsInView
                        .OfClass(typeof(FamilyInstance))
                        .Cast<FamilyInstance>()
                        .Where(fi => fi.Symbol.Family.Name == D_nazv_sem_Provoda).ToList();
                }
                catch  { }

                try
                {
                    // Собираем все элементы на активном виде
                    FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, viewPlan.Id);

                    // Фильтруем только FamilyInstance
                    SpPotrFamIns = allElementsInView
                        .OfClass(typeof(FamilyInstance))
                        .Cast<FamilyInstance>()
                        .Where(fi => fi.Category.Id.IntegerValue == (int)BuiltInCategory.OST_ElectricalEquipment).ToList();


                    if (SpPotrFamIns.Count > 0)
                    {

                        SpPotrFamIns = SpPotrFamIns
                                                .Where(fi =>
                                                {
                                                    // Получить коннекторы FamilyInstance

                                                    ConnectorSet connectors = (fi.MEPModel as ElectricalEquipment)?.ConnectorManager?.Connectors;

                                                    // Проверить, есть ли коннекторы и не пустой ли это список
                                                    return connectors != null && connectors.Size > 0; // Если разъемы существуют
                                                })
                                                .ToList();
                    }
                } catch { }
               

                try
                {
                    SpPotrGr = GetVisibleFamilyInstances_Potrebit_na_Vide(viewPlan);
                }
                catch { }

                try
                {
                    SPstoyak = GetVisibleFamilyInstances_Stoyak_na_Vide(viewPlan);
                }
                catch { }

                try
                {
                    SpProvGr = GetVisibleFamilyInstances_Provoda_Na_Vide(SpProvFamIns, SpPotrGr, SPstoyak);
                }
                catch { }



            }
        }

    }
}
