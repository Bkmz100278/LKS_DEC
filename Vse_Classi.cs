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
using static LKS_DEC.Sem_Postr_Graff;
using static LKS_DEC.DopFunc_Postr_graf;
using static LKS_DEC.Vse_Classi;

namespace LKS_DEC
{
    public class Vse_Classi
    {
        public class ElSet
        {
            public ElectricalSystem el_syst = null;

            public FamilyInstance BaseEq = null;

            public string imaBaseEq = D_Pust_Znach;

            public Connector BaseEqCon = null;

            public List<Connector> SpCon = new List<Connector>();

            public List<FamilyInstance> ListPotr = new List<FamilyInstance>();

            public List<El_Priemn_Fam> ListPotr_pr = new List<El_Priemn_Fam>();

            public string Kompl_marka = D_Pust_Znach;

            public string Nomer_Linii = D_Pust_Znach;

            public MarkerCep markerCep = null;

            public string TypSystemi = string.Empty;

            public Zagatovka_Numerac_Potr zagatovka_Numerac_Potr;

            public List<Provod_Graff> SPprovod_Graffs = new List<Provod_Graff>();

            public List<FamilyInstance> SPprovod_Graffs_asFam = new List<FamilyInstance>();

            public List<FamilyInstance> Sp_Stoyyak_asFam = new List<FamilyInstance>();

            public ElSet(ElectricalSystem _el_syst)
            {

                Kompl_marka = D_Pust_Znach;

                Nomer_Linii = D_Pust_Znach;

                el_syst = _el_syst;

                BaseEq = null;

                BaseEqCon = null;

                SpCon = new List<Connector>();

                ListPotr = new List<FamilyInstance>();

                SPprovod_Graffs = new List<Provod_Graff> ();

                try
                {
                    Kompl_marka = el_syst.LookupParameter(D_Ima_Par_Kompl).AsString();

                    if (string.IsNullOrEmpty(Kompl_marka))
                    {
                        Kompl_marka = D_Pust_Znach;
                    }
                }
                catch { }

                try
                {
                    Nomer_Linii = el_syst.LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).AsString();

                    if (string.IsNullOrEmpty(Nomer_Linii))
                    {
                        try
                        {
                            string Nom_caep = el_syst.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();

                            string Nom_caep_itog = "НЕНАЗНАЧ_" + Nom_caep;

                            Nomer_Linii = Nom_caep_itog;
                        }

                        catch
                        { }

                        if (string.IsNullOrEmpty(Nomer_Linii))
                        {
                            Nomer_Linii = D_Pust_Znach;
                        }
                    }
                }
                catch { }

                if (Nomer_Linii == D_Pust_Znach)
                {
                    try
                    {
                        Nomer_Linii = _el_syst.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();

                        if (string.IsNullOrEmpty(Nomer_Linii))
                        {
                            Nomer_Linii = D_Pust_Znach;
                        }
                    }

                    catch { }
                }

                try
                {
                    BaseEq = el_syst.BaseEquipment;
                }
                catch { }

                try
                {
                    BaseEqCon = el_syst.BaseEquipmentConnector;
                }
                catch { }

                try
                {
                    var ConMen = el_syst.ConnectorManager.Connectors;

                    foreach (Connector c in ConMen)
                    {
                        try
                        {
                            var hos = (c.Owner as FamilyInstance);            


                            if (!hos.Symbol.FamilyName.Contains(D_marker_kab_trass))
                            {
                                SpCon.Add(c);
                            }                            

                        }

                        catch 
                        {
                            try
                            {
                                ConnectorSet conset = c.AllRefs;

                                foreach (Connector CC in conset)
                                {
                                    var hos1 = CC.Owner as FamilyInstance;

                                    if (hos1 != null)
                                    {
                                        if (!hos1.Symbol.FamilyName.Contains(D_marker_kab_trass))
                                        {
                                            if (hos1.Id.IntegerValue != BaseEq.Id.IntegerValue)
                                            {
                                                SpCon.Add(CC);
                                            }
                                        }

                                        break;
                                    }
                                }
                            }

                            catch { }                       
                        
                        }
                    }
                }
                catch { }

                try
                {
                    var elems = el_syst.Elements;

                    foreach (Element e in elems)
                    {
                        string ima = (e as FamilyInstance).Symbol.FamilyName;

                        if (!ima.Contains(D_marker_kab_trass))
                        {
                            ListPotr.Add( e as FamilyInstance);

                        }

                        else
                        {
                            SPprovod_Graffs_asFam.Add(  e as FamilyInstance);
                        }
                    }
                }

                catch { }

                try
                {
                    ListPotr_pr = new List<El_Priemn_Fam>();

                    if (ListPotr.Count > 0)
                    {
                        foreach (var fam in ListPotr)
                        {
                            try
                            {
                                El_Priemn_Fam fam_pr = new El_Priemn_Fam(fam);

                                fam_pr.Nayti_Nom_V_Cepi(el_syst);

                                ListPotr_pr.Add(fam_pr);
                            }

                            catch { }
                        }
                    }

                    if (ListPotr_pr.Count > 0)
                    {
                        ListPotr_pr = ListPotr_pr.OrderBy(e => e.Nom_v_Cepi).ToList();

                        ListPotr.Clear();

                        foreach (var fampr in ListPotr_pr)
                        {
                            ListPotr.Add(fampr.famPriemn);
                        }
                    }
                }
                catch { }

                try
                {
                    markerCep = null;

                    string str_markerCep = ReadDataFromElement_posp(el_syst as Element, MetaData_for_Cep_Dan);

                    markerCep = new MarkerCep(str_markerCep);

                }
                catch { }

                try
                {
                    imaBaseEq = BaseEq.Symbol.LookupParameter(D_Ima_Oboznach).AsString() + "_" + BaseEq.LookupParameter(D_Ima_Poryadk_Nomer).AsString();
                }
                catch { }

                // TypSystemi

                try
                {
                    Parameter circuitTypeParam = el_syst.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_TYPE);
                    if (circuitTypeParam != null)
                    {
                        TypSystemi = circuitTypeParam.AsValueString();
                    }
                }
                catch { }
                

                if (D_Vse_StoYak.Count == 0)
                {
                    NaytiVseStoyaki();
                }

                if (D_Vse_StoYak.Count != 0)
                {
                    foreach (Stoyak st in D_Vse_StoYak)
                    {
                        st.nayti_ceps();

                        if (st.SP_ElSystStoyak.Count > 0)
                        {
                            foreach (var elsstt in st.SP_ElSystStoyak)
                            {
                                if (elsstt.Id.IntegerValue == el_syst.Id.IntegerValue)
                                {
                                    Sp_Stoyyak_asFam.Add(st.famins);
                                }                            
                            }                       
                        }                        
                    }                                    
                }

                if (SPprovod_Graffs_asFam.Count > 0 )
                {
                    SPprovod_Graffs = new List<Provod_Graff>();

                    foreach (var fam in SPprovod_Graffs_asFam)
                    {
                        Provod_Graff provod_Graff = new Provod_Graff(fam);

                        provod_Graff.typ_provod_ili_stoyak = 0;

                        SPprovod_Graffs.Add(provod_Graff);

                    }

                    if (Sp_Stoyyak_asFam.Count > 0)
                    {
                        foreach (var fam in Sp_Stoyyak_asFam)
                        {
                            Provod_Graff provod_Graff = new Provod_Graff(fam);                          

                            SPprovod_Graffs.Add(provod_Graff);

                        }
                    }
                }

                



                //try
                //{
                //    zagatovka_Numerac_Potr = new Zagatovka_Numerac_Potr();

                //    try
                //    {
                //        string zagatovka_Numerac_Potr_str = ReadDataFromElement_posp(el_syst as Element, MetaData_for_Cep_Markirovka_El);

                //        zagatovka_Numerac_Potr = new Zagatovka_Numerac_Potr(zagatovka_Numerac_Potr_str);
                //    }

                //    catch { }

                //}
                //catch { }
            }

            public void Zapis_Meta_Data_markerCep()
            {
                try
                {
                    markerCep.Zapakovka();

                    SchemaSetData_univers_posp(el_syst, MetaData_for_Elem_NomCEP, markerCep.Stroca_Itog);
                }

                catch { }
            }            

            //public void Podkl_Cepi_Graf()
            //{
            //    if (SPprovod_Graffs.Count > 0)
            //    {
            //        List<FamilyInstance> Sp = new List<FamilyInstance>();

            //        foreach (var pr in SPprovod_Graffs)
            //        {
            //            Sp.Add(pr.Pr_family_ins);
            //        }

            //        Otkluch_Ot_cepi(Sp, el_syst); // Отключаем все элементы графа от цепи
            //    }

            //    List<FamilyInstance> Sp1 = new List<FamilyInstance>();               

            //        Sp1.Add(BaseEq);

            //        foreach (var Fam in ListPotr)
            //        {
            //            Sp1.Add(Fam);
            //        }

            //        List<List<Provod_Graff>> nayti_Vse_Cepochku_Ravnoznacn = Nayti_Vse_Cepochku_Ravnoznacn(Sp1);               

            //        List<Tuple<Provod_Graff, int>> nayti_Unicaln_El_S_Colichestvom =  Nayti_Unicaln_El_S_Colichestvom(nayti_Vse_Cepochku_Ravnoznacn);

            //        List <FamilyInstance> fams = new List<FamilyInstance>();    

            //        foreach (var pr in D_vse_provoda)
            //        {
            //            string str = ReadDataFromElement_posp(pr.Pr_family_ins as Element, MetaData_for_ElGraf_Data);

            //            if (str.Contains(el_syst.Id.IntegerValue.ToString()))
            //            {
            //                fams.Add(pr.Pr_family_ins);
            //            }                 
            //        }

            //        if (fams.Count > 0)
            //        {
            //            using (Transaction t = new Transaction(doc, "Удаление данных по группе из элемента графа"))
            //            {
            //                t.Start();

            //                    foreach (var fam in fams)
            //                    {
            //                        Graf_Infa_Cep graf_Infa_Cep = new Graf_Infa_Cep(fam, el_syst);

            //                        graf_Infa_Cep.Udalenie(1);
            //                    }                            

            //                t.Commit();
            //            }
            //        }

            //        if (nayti_Unicaln_El_S_Colichestvom.Count > 0)
            //        {
            //            List <FamilyInstance> famspr = new List<FamilyInstance>();  

            //            foreach (var trup in nayti_Unicaln_El_S_Colichestvom)
            //            {
            //                famspr.Add(trup.Item1.Pr_family_ins);
            //            }

            //            if (famspr.Count > 0)
            //            {
            //            ConnectorSet connectorSet = new ConnectorSet();

            //            for (int i = 0; i < famspr.Count; i++)
            //            {
            //                try
            //                {
            //                    Connector con = naiti_con(famspr[i] , D_revitSystemsDict[D_Typ_System]); // D_SPelectricalSystemType["Пожарная сигнализация"] 

            //                    if (con != null)
            //                    {                              

            //                        connectorSet.Insert(con);
            //                    }
            //                }
            //                catch { }
            //            }

            //            using (Transaction t = new Transaction(doc, "Подключение элементов графа"))

            //            {
            //                t.Start();                           

            //                    el_syst.Add(connectorSet);

            //                t.Commit();
            //            }
            //        }

            //        using (Transaction t = new Transaction(doc, "Присвоение параметров элементам графа"))
            //        {
            //            t.Start();

            //            foreach (var trup in nayti_Unicaln_El_S_Colichestvom)
            //            {

            //                Graf_Infa_Cep raf_Infa_Cep = new Graf_Infa_Cep(trup.Item1.Pr_family_ins, el_syst);

            //                raf_Infa_Cep.Kol_Prov_Cepi = trup.Item2;

            //                raf_Infa_Cep.redactir_spisvoen(1);

            //            }

            //            t.Commit();
            //        }

            //    }
                
            //}

            public void Podkl_Cepi_Graf_S_Vidami( )
            {
                if (SPprovod_Graffs_asFam.Count > 0)
                {
                    Otkluch_Ot_cepi(SPprovod_Graffs_asFam, el_syst); // Отключаем все элементы графа от цепи
                }

                List<FamilyInstance> Sp1 = new List<FamilyInstance>();

                Sp1.Add(BaseEq);

                foreach (var Fam in ListPotr)
                {
                    Sp1.Add(Fam);
                }

                List<ViewPlan> SpPlan = NaytiVdi(Sp1, 0);

                if (SpPlan == null)
                {
                    return;
                }

                if (SpPlan.Count == 0)
                {
                    return;
                }

                List<List<Provod_Graff>> nayti_Vse_Cepochku_Ravnoznacn =

                Nayti_Vse_Cepochku_Ravnoznacn_na_vse_vidi(Sp1 , SpPlan);

                if (nayti_Vse_Cepochku_Ravnoznacn.Count == 0)
                {
                    SpPlan = NaytiVdi(Sp1, 1);

                    nayti_Vse_Cepochku_Ravnoznacn =

                    Nayti_Vse_Cepochku_Ravnoznacn_na_vse_vidi(Sp1, SpPlan);

                    if (nayti_Vse_Cepochku_Ravnoznacn.Count == 0)
                    {
                        return;
                    }
                }



                List<Tuple<Provod_Graff, int>> nayti_Unicaln_El_S_Colichestvom = Nayti_Unicaln_El_S_Colichestvom(nayti_Vse_Cepochku_Ravnoznacn);

                List<FamilyInstance> fams = new List<FamilyInstance>();

                foreach (var pr in D_vse_provoda_vse_vidi)
                {
                    string str = ReadDataFromElement_posp(pr.Pr_family_ins as Element, MetaData_for_ElGraff_Data);

                    if (str.Contains(el_syst.Id.IntegerValue.ToString()))
                    {
                        fams.Add(pr.Pr_family_ins);
                    }
                }

                foreach (var st in D_Vse_StoYak)
                {
                    string str = ReadDataFromElement_posp(st.famins as Element, MetaData_for_ElGraff_Data);

                    if (str.Contains(el_syst.Id.IntegerValue.ToString()))
                    {
                        fams.Add(st.famins);
                    }
                }


                if (fams.Count > 0)
                {
                    using (Transaction t = new Transaction(doc, "Удаление данных по группе из элемента графа"))
                    {
                        t.Start();

                        foreach (var fam in fams)
                        {
                            Graf_Infa_Cep graf_Infa_Cep = new Graf_Infa_Cep(fam, el_syst);

                            graf_Infa_Cep.Udalenie(1);

                        }

                        t.Commit();
                    }
                }

                if (nayti_Unicaln_El_S_Colichestvom.Count > 0)
                {
                    List<FamilyInstance> famspr = new List<FamilyInstance>();

                    foreach (var trup in nayti_Unicaln_El_S_Colichestvom)
                    {
                        famspr.Add(trup.Item1.Pr_family_ins);
                    }

                    if (famspr.Count > 0)
                    {
                        ConnectorSet connectorSet = new ConnectorSet();

                        for (int i = 0; i < famspr.Count; i++)
                        {
                            try
                            {
                                Connector con = naiti_con(famspr[i], D_revitSystemsDict[D_Typ_System]); // D_SPelectricalSystemType["Пожарная сигнализация"] 

                                if (con != null)
                                {
                                    connectorSet.Insert(con);
                                }
                            }
                            catch { }
                        }

                        using (Transaction t = new Transaction(doc, "Подключение элементов графа"))

                        {
                            t.Start();

                               el_syst.Add(connectorSet);

                            t.Commit();
                        }
                    }

                    using (Transaction t = new Transaction(doc, "Присвоение параметров элементам графа"))
                    {
                        t.Start();

                        foreach (var trup in nayti_Unicaln_El_S_Colichestvom)
                        {
                           
                                Graf_Infa_Cep raf_Infa_Cep = new Graf_Infa_Cep(trup.Item1.Pr_family_ins, el_syst);

                                raf_Infa_Cep.Kol_Prov_Cepi = trup.Item2;

                                raf_Infa_Cep.redactir_spisvoen(1);

                        }

                        t.Commit();
                    }

                }

                redakt_nazv_cep();

            }           
            

            public void redakt_nazv_cep()
            {
                foreach (var prov in SPprovod_Graffs)
                {
                    Graf_Infa_Cep raf_Infa_Cep = new Graf_Infa_Cep(prov.Pr_family_ins, el_syst);

                    raf_Infa_Cep.redactir_spisvoen(1);

                }
            }
        }

        public class MarkerCep
        {
            /// ////////////////////////////////////////////

            public string Razd_Glob = "_RGL_";

            public string Razd_Vnutr = "-RVN-";

            /// ////////////////////////////////////////////

            public string Nazv_Naim = "|НаименованиеЦепи|";

            public string Nazv_Sposob = "|Способ|";           

            public string Nazv_Nomer = "|Номер Число|";          

            public string Nazv_Nomer_v_Rod = "|Номер в Родительском|";

            public string Nazv_Rod_Panel = "|Id_Родительской Панели|";

            public string Nazv_Rod_Cep = "|Id_Родительской цепи|";

            /// //////////////////////////////////////////// 
           
            public string Naim = D_Pust_Znach;

            public string Sposob = D_Pust_Znach;

            public int Nomer = 0;

            public int Nomer_v_Rod = 0;

            public FamilyInstance Rod_Panel = null;

            public ElectricalSystem Rod_Cep = null;    


            /// ////////////////////////////////////////////

            public string Stroca_Itog = D_Pust_Znach;
            /// //////////////////////////////////////////// 

            public MarkerCep()
            {

            }

            public MarkerCep(string _Stroca_Itog)
            {
                Stroca_Itog = _Stroca_Itog;

                try
                {
                    Raspacovka();
                }
                catch { }
            }

            public MarkerCep(ElectricalSystem elsyst)
            {
                string _Stroca_Itog = ReadDataFromElement_posp(elsyst as Element, MetaData_for_Cep_Dan);

                if (string.IsNullOrEmpty(_Stroca_Itog))
                {
                    Stroca_Itog = D_Pust_Znach;
                }

                else 
                {
                    Stroca_Itog = _Stroca_Itog;
                    Raspacovka();
                }           

            }


            public void Raspacovka()
            {
                if ((string.IsNullOrEmpty(Stroca_Itog)) || (Stroca_Itog == D_Pust_Znach))

                { return; }

                Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik();

                try
                {
                    Naim = preobrazovatVSpravochnik[Nazv_Naim];

                    if (string.IsNullOrEmpty(Naim))
                    {
                        Naim = D_Pust_Znach;
                    }
                }
                catch  { }

                try
                {
                    Sposob = preobrazovatVSpravochnik[Nazv_Sposob];

                    if (string.IsNullOrEmpty(Sposob))
                    {
                        Sposob = D_Pust_Znach;
                    }
                }
                catch { }

                try
                {
                    if (
                        (preobrazovatVSpravochnik[Nazv_Nomer] == D_Pust_Znach) 
                        || 
                        ( string.IsNullOrEmpty(preobrazovatVSpravochnik[Nazv_Nomer])
                        )
                        )
                    {
                        Nomer = 0;
                    }

                    else
                    {

                        int.TryParse(preobrazovatVSpravochnik[Nazv_Nomer], out int numericId);

                        Nomer = numericId;
                    }

                }
                catch { }


                try
                {
                    if (
                        (preobrazovatVSpravochnik[Nazv_Nomer_v_Rod] == D_Pust_Znach)
                        ||
                        (string.IsNullOrEmpty(preobrazovatVSpravochnik[Nazv_Nomer_v_Rod])
                        )
                        )
                    {
                        Nomer_v_Rod = 0;
                    }

                    else
                    {

                        int.TryParse(preobrazovatVSpravochnik[Nazv_Nomer_v_Rod], out int numericId);

                        Nomer_v_Rod = numericId;
                    }

                }
                catch { }                 


                try
                {
                    if (
                        (preobrazovatVSpravochnik[Nazv_Rod_Panel] == D_Pust_Znach)
                        ||
                        (string.IsNullOrEmpty(preobrazovatVSpravochnik[Nazv_Rod_Panel])
                        )
                        )
                    {
                        Rod_Panel = null;
                    }

                    else
                    {

                        int.TryParse(preobrazovatVSpravochnik[Nazv_Rod_Panel], out int numericId);

                        Rod_Panel = doc.GetElement(new ElementId(numericId)) as FamilyInstance;
                    }

                }
                catch { }


                try
                {
                    if (
                        (preobrazovatVSpravochnik[Nazv_Rod_Cep] == D_Pust_Znach)
                        ||
                        (string.IsNullOrEmpty(preobrazovatVSpravochnik[Nazv_Rod_Cep])
                        )
                        )
                    {
                        Rod_Cep = null;
                    }

                    else
                    {

                        int.TryParse(preobrazovatVSpravochnik[Nazv_Rod_Cep], out int numericId);

                        Rod_Cep = doc.GetElement(new ElementId(numericId)) as ElectricalSystem;
                    }

                }
                catch { }

            }

            public void Zapakovka()
            {
                List<string> Dla_Zapakovki = new List<string>();

                /////////////////////////////////////////////////////////////////////

                if (string.IsNullOrEmpty(Naim))
                {
                    Naim = D_Pust_Znach;
                }

                string str1 = Nazv_Naim + Naim;      

                Dla_Zapakovki.Add(str1);

                /////////////////////////////////////////////////////////////////////

                if (string.IsNullOrEmpty(Sposob))
                {
                    Sposob = D_Pust_Znach;
                }

                string str2 = Nazv_Sposob + Sposob;

                Dla_Zapakovki.Add(str2);

                /////////////////////////////////////////////////////////////////////

                string str_nom = D_Pust_Znach;
                if (Nomer != 0)
                {
                    str_nom = Nomer.ToString();
                }

                string str3 = Nazv_Nomer + str_nom;

                Dla_Zapakovki.Add(str3);

                /////////////////////////////////////////////////////////////////////

                string str_nom_rod = D_Pust_Znach;
                if (Nomer_v_Rod != 0)
                {
                    str_nom_rod = Nomer_v_Rod.ToString();
                }

                string str4 = Nazv_Nomer_v_Rod + str_nom_rod;

                Dla_Zapakovki.Add(str4);

                /////////////////////////////////////////////////////////////////////

                string str_id_Rod_Pan = D_Pust_Znach;
                if (Rod_Panel != null)
                {
                    str_id_Rod_Pan = Rod_Panel.Id.IntegerValue.ToString();
                }

                string str5 = Nazv_Rod_Panel + str_id_Rod_Pan;

                Dla_Zapakovki.Add(str5);

                /////////////////////////////////////////////////////////////////////

                string str_id_Rod_Cep = D_Pust_Znach;
                if (Rod_Cep != null)
                {
                    str_id_Rod_Cep = Rod_Cep.Id.IntegerValue.ToString();
                }

                string str6 = Nazv_Rod_Cep + str_id_Rod_Cep;

                Dla_Zapakovki.Add(str6);


                /////////////////////////////////////////////////////////////////////


                Stroca_Itog = D_Pust_Znach;

                if (Dla_Zapakovki.Count > 0)
                {
                    Stroca_Itog = string.Join(Razd_Glob, Dla_Zapakovki);
                }

            }

            public Dictionary<string, string> PreobrazovatVSpravochnik()
            {
                // Список возможных ключей
                var keys = new List<string>
                {
                    Nazv_Naim,
                    Nazv_Sposob,
                    Nazv_Nomer,
                    Nazv_Nomer_v_Rod,
                    Nazv_Rod_Panel,
                    Nazv_Rod_Cep

                };       

                // Разбиваем строку на фрагменты по разделителю
                var segments = Stroca_Itog.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                var result = new Dictionary<string, string>();

                // Обрабатываем каждую часть строки
                foreach (string key in keys)
                {
                    int fl = 0;

                    // Проверяем, содержит ли сегмент один из ключей
                    foreach (string segment in segments)
                    {
                        if (segment.Contains(key))
                        {
                            // Извлекаем значение (удаляем ключ из сегмента)
                            string value = segment.Replace(key, "").Trim();
                            // Добавляем пару ключ-значение в словарь
                            result[key] = value;
                            fl = 1;
                            break; // Ключ найден, дальше проверять смысла нет
                        }
                    }

                    if (fl == 0)
                    {
                        result[key] = D_Pust_Znach;
                    }
                }

                return result;
            }



        }

        public class OpisanCon
        {
            public string Razd_Glob = "-RVN-";     

            /// ////////////////////////////////////////////

            public string Nazv_Id_Con = "|Id коннектора|";

            public string Nazv_NomV_Cepi = "|Номер в цепи|";            

            /// ////////////////////////////////////////////

            public int Id_Con    = 0;           

            public int NomV_Cepi = 0;

            /// ////////////////////////////////////////////

            public string Stroca_Itog_Con = D_Pust_Znach;

            /// //////////////////////////////////////////// <summary>

            public OpisanCon(string _Stroca_Itog_Con)
            {
                Stroca_Itog_Con = _Stroca_Itog_Con;

                try
                {
                    Raspacovka();
                }
                catch { }
            }

            public OpisanCon()
            { }

            public void Raspacovka()
            {
                if ((string.IsNullOrEmpty(Stroca_Itog_Con)) || (Stroca_Itog_Con == D_Pust_Znach))

                { return; }

                Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik();
                                

                try
                {
                    if (
                        (preobrazovatVSpravochnik[Nazv_Id_Con] == D_Pust_Znach)
                        ||
                        (string.IsNullOrEmpty(preobrazovatVSpravochnik[Nazv_Id_Con])
                        )
                        )
                    {
                        Id_Con = 0;
                    }

                    else
                    {

                        int.TryParse(preobrazovatVSpravochnik[Nazv_Id_Con], out int numericId);

                        Id_Con = numericId;
                    }

                }
                catch { }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                              


                try
                {
                    if (
                        (preobrazovatVSpravochnik[Nazv_NomV_Cepi] == D_Pust_Znach)
                        ||
                        (string.IsNullOrEmpty(preobrazovatVSpravochnik[Nazv_NomV_Cepi])
                        )
                        )
                    {
                        NomV_Cepi = 0;
                    }

                    else
                    {

                        int.TryParse(preobrazovatVSpravochnik[Nazv_NomV_Cepi], out int numericId);

                        NomV_Cepi = numericId;
                    }

                }
                catch { }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////

            }

            public void Zapakovka()
            {
                List<string> Dla_Zapakovki = new List<string>();

                /////////////////////////////////////////////////////////////////////              

                string str_nId = D_Pust_Znach;
                if (Id_Con != 0)
                {
                    str_nId = Id_Con.ToString();
                }

                string str1 = Nazv_Id_Con + str_nId;

                Dla_Zapakovki.Add(str1);

                ///////////////////////////////////////////////////////////////////// 

                string str_NomV_Cepi = D_Pust_Znach;

                if (NomV_Cepi != 0)
                {
                    str_NomV_Cepi = NomV_Cepi.ToString();
                }

                string str2 = Nazv_NomV_Cepi + str_NomV_Cepi;

                Dla_Zapakovki.Add(str2);

                ///////////////////////////////////////////////////////////////////// 


                Stroca_Itog_Con = D_Pust_Znach;

                if (Dla_Zapakovki.Count > 0)
                {
                    Stroca_Itog_Con = string.Join(Razd_Glob, Dla_Zapakovki);
                }

            }

            public Dictionary<string, string> PreobrazovatVSpravochnik()
            {
                // Список возможных ключей
                var keys = new List<string>
                {

                      Nazv_Id_Con ,
                      Nazv_NomV_Cepi           
                };

                // Разбиваем строку на фрагменты по разделителю
                var segments = Stroca_Itog_Con.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                var result = new Dictionary<string, string>();

                // Обрабатываем каждую часть строки
                foreach (string key in keys)
                {
                    int fl = 0;

                    // Проверяем, содержит ли сегмент один из ключей
                    foreach (string segment in segments)
                    {
                        if (segment.Contains(key))
                        {
                            // Извлекаем значение (удаляем ключ из сегмента)
                            string value = segment.Replace(key, "").Trim();
                            // Добавляем пару ключ-значение в словарь
                            result[key] = value;
                            fl = 1;
                            break; // Ключ найден, дальше проверять смысла нет
                        }
                    }

                    if (fl == 0)
                    {
                        result[key] = D_Pust_Znach;
                    }
                }

                return result;
            }

        }

        public class El_Panel
        { 
           public  List<ElSet> elSets = new List<ElSet>();   
        
        
        }        

        public class El_Priemn_Fam
        {
            public FamilyInstance famPriemn = null;

            public string imaPriemn = D_Pust_Znach;

            public string Oboznach = D_Pust_Znach;

            public string Poryadk_Nomer = D_Pust_Znach;   

            public string SecEtag = D_Pust_Znach;

            public string Ima_pom = D_Pust_Znach;

            public List<El_Con> Sp_Con_Vse = new List<El_Con>();

            public List<El_Con> Sp_Con_Podkl = new List<El_Con>();

            public List<El_Con> Sp_Con_NePodkl = new List<El_Con>();           

            public string STR_Opis_Con = D_Pust_Znach;

            public string STR_Marka_Priemn = D_Pust_Znach;

            public string STR_Marka_Cepei = D_Pust_Znach;

            public int Nom_v_Cepi = 0; // Это номер в цепи. Нужно уточнить цепь.

            public int Kol_adresov = 0;



            public El_Priemn_Fam(FamilyInstance _famPriemn)
            {
                famPriemn = _famPriemn;

                Shitat_Dan();

                SpisocConPodkl_i_Ne_Podkl();

                Schit_Par();

                Sobiranie_v_Stroku();

                Uzn_Zn_Marki_Cep();
            }            

            public void Shitat_Dan()
            {
                Sp_Con_Vse.Clear();

                var ElSystSet = famPriemn.MEPModel.ElectricalSystems;

                ConnectorSet Sp_Con_set = null;

                try
                {
                    Sp_Con_set =  famPriemn.MEPModel.ConnectorManager.Connectors;
                }
                catch { }

                if (Sp_Con_set != null)
                {

                    List<Connector> connectorsEl = new List<Connector>();

                    Connector con2 = null;

                    foreach (var cc in Sp_Con_set)
                    {
                        con2 = cc as Connector;

                        if (con2 != null)
                        {

                            string strsist = string.Empty;

                            try 
                            {
                                strsist = con2.ElectricalSystemType.ToString();
                            }
                            catch { }


                            if (! string.IsNullOrEmpty(strsist) )
                            {
                                Connector con3 = null;

                                ConnectorSet ccc = null;

                                try
                                {
                                    ccc = con2.AllRefs;
                                }
                                catch { }

                                if (ccc != null)
                                {
                                    

                                    foreach (var ccc3 in ccc)
                                    {
                                        con3 = ccc3 as Connector;

                                        if (con3 != null)
                                        {                                            
                                            break;
                                        }

                                        else 
                                        {
                                            Sp_Con_Vse.Add(new El_Con(con2));
                                            break;
                                        }
                                    }
                                }

                                else
                                {
                                    Sp_Con_Vse.Add(new El_Con(con2));
                                }


                                if (con3 != null)
                                {
                                    try
                                    {
                                        var IdInt = (con3.Owner as ElectricalSystem).BaseEquipment.Id.IntegerValue;

                                        if (IdInt != famPriemn.Id.IntegerValue)
                                        {
                                            Sp_Con_Vse.Add(new El_Con(con2));

                                        }
                                    }
                                    catch { }
                                }

                                else 
                                {
                                    Sp_Con_Vse.Add(new El_Con(con2));
                                }


                            }
                        }
                    }               
                }
               

                List<El_Con> SPcon_dop = chtenieDan_proCon();

                if ((Sp_Con_Vse.Count > 0) && (SPcon_dop.Count > 0))
                {

                    List<El_Con> SPcon_dop11 = new List<El_Con>() ;

                    foreach ( var  con in  Sp_Con_Vse )
                    {                        

                        foreach (var con1 in SPcon_dop)
                        {
                            try
                            {
                                if ((con.famPriemn_Host.Id.IntegerValue == con1.famPriemn_Host.Id.IntegerValue) && (con.con.Id.ToString() == con1.con.Id.ToString()))
                                {
                                    con.Nomer = con1.Nomer;

                                    break;

                                }
                            }
                            catch { }   

                        }

                        SPcon_dop11.Add(con);
                    }

                    Sp_Con_Vse = SPcon_dop11;
                }
            }

            public void  SpisocConPodkl_i_Ne_Podkl()
            {
                Sp_Con_Podkl.Clear();

                Sp_Con_NePodkl.Clear();

                if (Sp_Con_Vse.Count > 0 )
                {
                    foreach (var item in Sp_Con_Vse)
                    {
                        if (item.flag_Podkl == 0)
                        {
                            Sp_Con_NePodkl.Add(item);
                        }

                        else
                        {
                            Sp_Con_Podkl.Add(item);
                        }
                    }               
                }            
            }

            public List<El_Con> Sp_Con_Podkl_k_Systeme(string syst_str)
            {
                List<El_Con> sp_Con_Podkl_k_Systeme = new List<El_Con>();

                if (Sp_Con_Podkl.Count > 0)
                {
                    foreach (var item in Sp_Con_Vse)
                    {
                        if (item.syst_typ_con.ToString() == syst_str)
                        {
                            sp_Con_Podkl_k_Systeme.Add(item);
                        }
                    }
                }

                return sp_Con_Podkl_k_Systeme;
            }

            public List<El_Con> Sp_Con_Ne_Podkl_k_Systeme(string syst_str)
            {
                List<El_Con> sp_Con_NePodkl_k_Systeme = new List<El_Con>();

                if (Sp_Con_NePodkl.Count > 0)
                {
                    foreach (var item in Sp_Con_Vse)
                    {
                        if (item.syst_typ_con.ToString() == syst_str)
                        {
                            sp_Con_NePodkl_k_Systeme.Add(item);
                        }
                    }
                }

                return sp_Con_NePodkl_k_Systeme;
            }

            public void Schit_Par()

            {
                 Oboznach = D_Pust_Znach;

                try
                {
                    Oboznach = famPriemn.Symbol.LookupParameter(D_Ima_Oboznach).AsString();

                    if (string.IsNullOrEmpty(Oboznach))
                    {
                        Oboznach = D_Pust_Znach;

                    }
                }
                catch { }

                Poryadk_Nomer = D_Pust_Znach;

                try
                {
                    Poryadk_Nomer = famPriemn.LookupParameter(D_Ima_Poryadk_Nomer).AsString();

                    if (string.IsNullOrEmpty(Oboznach))
                    {
                        Poryadk_Nomer = D_Pust_Znach;

                    }
                }
                catch { }
                
                imaPriemn = D_Pust_Znach;

                try
                {
                    imaPriemn = Oboznach + "-" + Poryadk_Nomer;
                }
                catch { }

                SecEtag = D_Pust_Znach;

                try
                {
                    string et_str = famPriemn.LookupParameter(D_Ima_Par_Etag_str).AsString();

                    string et_sekc = famPriemn.LookupParameter(D_Ima_Par_Sekcia_str).AsString();                   

                    SecEtag = et_str + "/" + et_sekc;

                    if (string.IsNullOrEmpty(SecEtag))
                    {
                        SecEtag = D_Pust_Znach;
                    }
                }
                catch { }

                Ima_pom = D_Pust_Znach;

                try
                {
                    string imapomm = famPriemn.LookupParameter(D_Ima_Ima_Pomech).AsString();

                    Ima_pom = imapomm;

                    if (string.IsNullOrEmpty(Ima_pom))
                    {
                        Ima_pom = D_Pust_Znach;
                    }
                }
                catch { }

                STR_Opis_Con = D_Pust_Znach;

                try
                {
                    STR_Opis_Con = ReadDataFromElement_posp(famPriemn as Element, MetaData_for_Elem_Potr_Sp_Con);
                }
                catch { }

                Kol_adresov = 1;

                try
                {
                    Kol_adresov = famPriemn.Symbol.LookupParameter(D_Ima_Kol_Adresov).AsInteger() ;
                }
                catch { }

                if (Kol_adresov <= 0)
                {
                    Kol_adresov = 1;
                }

            }



            public List<El_Con> chtenieDan_proCon()
            {

                List<El_Con> Sp_Con_Vse11 = new List<El_Con>();

                STR_Opis_Con = D_Pust_Znach;              

                try
                {
                    STR_Opis_Con = ReadDataFromElement_posp(famPriemn as Element, MetaData_for_Elem_Potr_Sp_Con);  // 
                }
                catch { }

                if (string.IsNullOrEmpty(STR_Opis_Con))
                {
                    STR_Opis_Con = D_Pust_Znach;
                }

                if (STR_Opis_Con != D_Pust_Znach)
                {
                    var segments = STR_Opis_Con.Split(new[] { D_Razd_Glob }, StringSplitOptions.None);

                    if (segments.Length > 0)
                    {
                        foreach (string consss in segments)
                        {
                            try
                            {
                                Sp_Con_Vse11.Add(new El_Con(consss));
                            }
                            catch { }
                        }                    
                    }                   

                }

                return Sp_Con_Vse11;   

            }

            public void Smena_Dan_U_Con(El_Con el_Con_izm, int Flag_izm_dob_ubrat )
            {   

                if (el_Con_izm == null)
                { return; }                

                if (Sp_Con_Vse.Count > 0)
                {
                   List<El_Con> Sp_Con_Vse1 = new List<El_Con>();

                    int flll = 0;

                    foreach (var con in Sp_Con_Vse)
                    {
                        if ((con.famPriemn_Host.Id.IntegerValue == el_Con_izm.famPriemn_Host.Id.IntegerValue) 
                            && (con.con.Id.ToString() == el_Con_izm.con.Id.ToString())              
                            )
                        {
                            if (Flag_izm_dob_ubrat != 2)
                            {
                                Sp_Con_Vse1.Add(el_Con_izm);
                                flll++;
                            }
                        }

                        else 
                        {
                            Sp_Con_Vse1.Add(con);
                        }
                    }

                    if ((flll == 0) && (Flag_izm_dob_ubrat == 1))
                    {
                        Sp_Con_Vse1.Add(el_Con_izm);
                    }
                }
            }

            public void Sobiranie_v_Stroku()
            {
                STR_Opis_Con = D_Pust_Znach;

                if (Sp_Con_Vse.Count > 0)
                {
                    STR_Opis_Con = string.Join(D_Razd_Glob, Sp_Con_Vse);
                }
            }

            public void Uzn_Zn_Marki_Cep()
            {
                STR_Marka_Priemn = D_Pust_Znach;

                if (Sp_Con_Podkl.Count > 0)
                {

                    List<string> SP_Dla_Marki_Cep = new List<string>();

                    foreach (var item in Sp_Con_Podkl)
                    {
                        var els = item.el_syst;

                        string Nazv = D_Pust_Znach;

                        try
                        {
                            Nazv = els.LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).AsString();
                        }
                        catch { }

                        if (string.IsNullOrEmpty(Nazv))
                        {
                            Nazv = D_Pust_Znach;
                        }

                        SP_Dla_Marki_Cep.Add(Nazv);

                    }

                    if (SP_Dla_Marki_Cep.Count > 0)
                    {
                        List<string> uniqueSortedList = SP_Dla_Marki_Cep
                                .Distinct() // Удаляет дубли
                                .OrderBy(x => x) // Сортирует в алфавитном порядке
                                 .ToList(); // Возвращает в виде List<string>

                        STR_Marka_Priemn = string.Join(D_Razd_v_markach2_TiZap, uniqueSortedList);

                    }
                }
            }

            public int Proverka_na_imen_Marki_Cep()
            {
                // 1 нужно присвоить новую марку // 2 не нужно ничего делать // 3 нужно обнулить

                int flag_na_deystv = 1;

                string ot_marki = D_Pust_Znach;

                string marka_pofactu = D_Pust_Znach;

                string mark_kot_est = D_Pust_Znach;

                try
                {
                    mark_kot_est = famPriemn.LookupParameter(D_Ima_Par_Nomer_Linii).AsString();

                    if (string.IsNullOrEmpty(mark_kot_est))
                    {
                        mark_kot_est = D_Pust_Znach;
                    }
                }
                catch { }

                if (mark_kot_est != D_Pust_Znach)
                {                    

                    try
                    {

                        ot_marki = ProcessAndCombine(mark_kot_est, D_Razd_v_markach2_TiZap);
                    }
                    catch { }                    

                    try
                    {
                        marka_pofactu = ProcessAndCombine(STR_Marka_Cepei, D_Razd_v_markach2_TiZap);
                    }
                    catch { }
                }                

                if (ot_marki == marka_pofactu )
                {
                    return 2;
                }

                else
                {

                    if (marka_pofactu != D_Pust_Znach)
                    {
                        return 1;
                    }

                    else
                    {
                        return 3;
                    }                    
                }
            }


            public void Zapis_dan_v_Sem_Marki( )
            {
                // flag_tras= 1 с транзакцией 2 - без 

               int prov = Proverka_na_imen_Marki_Cep();

               if  (prov == 1 )
               {
                    try
                    {

                        using (Transaction t = new Transaction(doc, "Назначение марке группы"))
                        {
                            t.Start();

                            famPriemn.LookupParameter(D_Ima_Par_Nomer_Linii).Set(STR_Marka_Cepei);

                            t.Commit();
                        }
                    } 
                    catch 
                    {
                        try
                        {                          

                           famPriemn.LookupParameter(D_Ima_Par_Nomer_Linii).Set(STR_Marka_Cepei);                               
                        }
                        catch { }

                    }
               }

                if (prov == 3)
                {
                    try
                    {


                        using (Transaction t = new Transaction(doc, "Назначение марке группы"))
                        {
                            t.Start();

                            famPriemn.LookupParameter(D_Ima_Par_Nomer_Linii).Set(string.Empty);

                            t.Commit();
                        }
                    }
                    catch 
                    {
                        try 
                        {
                            famPriemn.LookupParameter(D_Ima_Par_Nomer_Linii).Set(string.Empty);

                        } 
                        catch { }                   
                    
                    }
                }              

            }

            public void Zapis_dan_v_Sem_Dan_Po_Con()
            {
                Sobiranie_v_Stroku();
                SchemaSetData_univers_posp(famPriemn as Element, MetaData_for_Elem_Potr_Sp_Con, STR_Opis_Con);
            }

            public Connector Naiti_Perv_Nepodl_Dla_Proizv_Cep()
            {
                Connector con_isk = null;

                try
                {
                    con_isk = NaitiCon1El(famPriemn).Item1;
                }

                catch { }

                return con_isk; 
            }

            public void Nayti_Nom_V_Cepi(ElectricalSystem els)
            {
                foreach (var con in Sp_Con_Podkl)
                {
                    if (con.el_syst.Id.IntegerValue == els.Id.IntegerValue)
                    {
                        Nom_v_Cepi = con.Nomer;
                    }
                }
            }
        }

        public class El_Con 
        {
            public Connector con = null;

            public FamilyInstance famPriemn_Host = null;

            public ElectricalSystem el_syst;

            public ElectricalSystemType syst_typ_con = ElectricalSystemType.FireAlarm;

            public int Nomer = 0;

            public string stroka_inf_con = D_Pust_Znach;

            public int flag_Podkl = 0;

            //public string ima_Nazv_Dla_Con_str = D_Pust_Znach;

            //public Ima_Nazv_Dla_Con ima_Nazv_Dla_Con = null;

            public El_Con(Connector _con)
            {
                con = _con;
                Dan_iz_con();
                Formir_Str();
            }

            public El_Con(string _stroka_inf_con)
            {
                stroka_inf_con = _stroka_inf_con;

                if ((stroka_inf_con != D_Pust_Znach) && ( string.IsNullOrEmpty(stroka_inf_con)))
                {
                    var segments = stroka_inf_con.Split(new[] { D_Razd_Vnutr }, StringSplitOptions.None);

                    try
                    {
                        famPriemn_Host = doc.GetElement( new ElementId (int.Parse(segments[0]))) as FamilyInstance;
                    }
                    catch { }

                    try
                    {
                        var cons = famPriemn_Host.MEPModel.ConnectorManager.Connectors;

                        foreach ( var cons2 in cons)                        
                        {
                            if ((cons2 as Connector).Id.ToString() == segments[1])
                            {
                                con = cons2 as Connector;

                                break;
                            }                        
                        }                        
                    }
                    catch { }

                    try
                    {
                        Nomer = int.Parse(segments[1]);
                    }
                    catch { }

                    //ima_Nazv_Dla_Con_str = segments[2];

                    //if (ima_Nazv_Dla_Con_str != D_Pust_Znach)
                    //{
                    //    ima_Nazv_Dla_Con = new Ima_Nazv_Dla_Con(ima_Nazv_Dla_Con_str);

                    //}

                    if (con != null)
                    {
                        Dan_iz_con();
                    }
                }               

            }

            public void Dan_iz_con ()
            { 

                if (con != null)
                {
                    try
                    {
                        famPriemn_Host = con.Owner as FamilyInstance;
                    }
                    catch { }

                    if (famPriemn_Host == null)
                    {
                        try
                        {
                            var con_set = con.AllRefs;

                            foreach (var cc in con_set)
                            {
                                Connector connector2 = cc as Connector;

                                if (connector2 != null)
                                {
                                    famPriemn_Host = connector2.Owner as FamilyInstance;
                                    break;

                                }
                            }

                        }
                        catch { }
                    }

                    try
                    {
                        el_syst = Naiti_Syst_El_u_Con(con);
                    }
                    catch { }

                    if (el_syst == null)
                    {
                        try 
                        {
                            el_syst = con.Owner as ElectricalSystem;    
                        } 
                        catch { }
                    
                    }

                    if (el_syst != null)
                    {
                        flag_Podkl = 1;
                    }

                    else
                    {
                        flag_Podkl = 0;
                    }

                    try
                    {
                        syst_typ_con = con.ElectricalSystemType;
                    }
                    catch { }

                    try
                    {
                        if ((famPriemn_Host is FamilyInstance) && (flag_Podkl == 1))
                        {                          

                            var fff =  ReadDataFromElement_posp(famPriemn_Host as Element, MetaData_for_Elem_NomCEP);

                            var segments = fff.Split(new[] { D_Razd_Glob }, StringSplitOptions.None);

                            foreach (var segment in segments)
                            {
                                OpisanCon op = new OpisanCon(segment);

                                if (con.Id == op.Id_Con)
                                {
                                    Nomer = op.NomV_Cepi;
                                }
                            
                            }

                        }
                    }
                    catch { }

                }
            }

            public void Formir_Str()
            {
                if ((famPriemn_Host != null) && (con != null))
                {
                    //if (ima_Nazv_Dla_Con != null)
                    //{
                    //    ima_Nazv_Dla_Con.Sborka();
                    //    ima_Nazv_Dla_Con_str = ima_Nazv_Dla_Con.stroka_itog_dla_chran;
                    //}

                    stroka_inf_con = famPriemn_Host.Id.IntegerValue.ToString() + D_Razd_Vnutr + con.Id.ToString() + D_Razd_Vnutr + Nomer.ToString();
                }            
            }

            public void Izm_Nomer_V_Cepi_S_Prisvoeniem(int Nom_nov)
            {
                if (Nom_nov > 0)
                {
                    if (Nom_nov != Nomer)
                    {
                        Nomer = Nom_nov;

                        Formir_Str();

                        El_Priemn_Fam pr_el = new El_Priemn_Fam(famPriemn_Host);

                        pr_el.Smena_Dan_U_Con(this, 0);

                        pr_el.Zapis_dan_v_Sem_Dan_Po_Con();

                    }
                }            
            }
        }       

        public class Ima_Nazv_Dla_Cep
        {
            public string Perfics = D_Pust_Znach;

            public int Znach_ch = 0;

            public string Sufics = D_Pust_Znach;

            public int rezhim = 1;
        }

        public class Ima_Nazv_Dla_Con
        {
            public string Perfics = D_Pust_Znach;

            public int Znach_ch = 0;

            public string Sufics = D_Pust_Znach;

            public int rezhim = 0;

            public string stroka_itog_dla_chran = D_Pust_Znach;

            public string stroka_na_marku = D_Pust_Znach;

            public Ima_Nazv_Dla_Con()
            {
                
            }

            public Ima_Nazv_Dla_Con(string _stroka_itog_dla_chran)
            {
                stroka_itog_dla_chran = _stroka_itog_dla_chran;

                if (stroka_itog_dla_chran != D_Pust_Znach)
                {
                    Razborka();
                }
            }

            public void Sborka()
            {

                if (string.IsNullOrEmpty(Perfics))
                {
                    Perfics = D_Pust_Znach;
                }

                if (string.IsNullOrEmpty(Sufics))
                {
                    Sufics = D_Pust_Znach;
                }

                stroka_itog_dla_chran = Perfics + D_Razd_Vnutr2 + Znach_ch.ToString() + D_Razd_Vnutr2 + Sufics + D_Razd_Vnutr2 + rezhim.ToString();
                Formir_na_marku();
            }

            public void Razborka()
            {
                var segments = stroka_itog_dla_chran.Split(new[] { D_Razd_Vnutr }, StringSplitOptions.None);

                if (segments.Length == 4)
                {
                    Perfics = segments[0];

                    Znach_ch = int.Parse(segments[1]);

                    Sufics = segments[2];

                    rezhim = int.Parse(segments[3]);
                }
            }

            public void Formir_na_marku()
            {

                string chislo_otfor = "0";

                try
                {

                  chislo_otfor = FormatNumber(Znach_ch, rezhim).ToString();
                }
                catch { }

                stroka_na_marku = Perfics + chislo_otfor + Sufics;

            }


        }

        public class Zagatovka_Numerac_Potr
        {
            public int forma_zapis = 0; // Если 0 то 1,2,3,4 Если 1 то 01 ,02 , 03 и т.д..

            public string perf = string.Empty;

            public string sufics = string.Empty;

            public int Nach_znach_chislo = 1;

            public string Nach_znach_Str = string.Empty;

            public string Razd_Vnutr = "-RVNUTR-";

            public string Nazv_forma_zapis = "|Форма записи|";

            public string Nazv_Nach_znach_chislo = "|Начальное значение число|";

            public string Nazv_Nach_znach_stroka = "|Начальное значение строка|";

            public string Nazv_perf = "|Perfics|";

            public string Nazv_sufics = "|Sufics|";

            public string Stroca_Itog = D_Pust_Znach;           

            public Zagatovka_Numerac_Potr()
            { }

            public Zagatovka_Numerac_Potr(string _Stroca_Itog)
            {
                Stroca_Itog = _Stroca_Itog;

                Raspacovka();
            }

            public Zagatovka_Numerac_Potr(string _Nach_znach_Str, string _perf , string _sufics)
            {
                Nach_znach_Str = _Nach_znach_Str;
                ParseNumber();
                perf = _perf;
                sufics = _sufics;
                Zapakovka();
            }


            public void Raspacovka()
            {
                if ((string.IsNullOrEmpty(Stroca_Itog)) || (Stroca_Itog == D_Pust_Znach))

                { return; }

                Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik();


                try
                {
                    Nach_znach_Str = preobrazovatVSpravochnik[Nazv_Nach_znach_stroka];

                    ParseNumber();
                }
                catch { }              

                /////////////////////////////////////////////////////////////////////////////////////////////////////////

                try
                {
                    perf = preobrazovatVSpravochnik[Nazv_perf];
                }
                catch { }


                try
                {
                    sufics = preobrazovatVSpravochnik[Nazv_sufics];
                }
                catch { }              


            }

            public void Zapakovka()
            {
                Stroca_Itog = Nazv_Nach_znach_stroka + Nach_znach_Str + Razd_Vnutr + Nazv_perf 
                    + perf + Razd_Vnutr + Nazv_sufics
                    + sufics;
            }

            public Dictionary<string, string> PreobrazovatVSpravochnik()
            {
                // Список возможных ключей
                var keys = new List<string>
                {
                    Nazv_Nach_znach_stroka,
                    Nazv_perf,                    
                    Nazv_sufics

                };

                // Разбиваем строку на фрагменты по разделителю
                var segments = Stroca_Itog.Split(new[] { Razd_Vnutr }, StringSplitOptions.None);

                var result = new Dictionary<string, string>();

                // Обрабатываем каждую часть строки
                foreach (string key in keys)
                {
                    int fl = 0;

                    // Проверяем, содержит ли сегмент один из ключей
                    foreach (string segment in segments)
                    {
                        if (segment.Contains(key))
                        {
                            // Извлекаем значение (удаляем ключ из сегмента)
                            string value = segment.Replace(key, "").Trim();
                            // Добавляем пару ключ-значение в словарь
                            result[key] = value;
                            fl = 1;
                            break; // Ключ найден, дальше проверять смысла нет
                        }
                    }

                    if (fl == 0)
                    {
                        result[key] = D_Pust_Znach;
                    }
                }

                return result;
            }

            public string MetodStrIz_Int(int a, int b)
            {
                b++;
                // Форматирование с помощью PadLeft
                return a.ToString().PadLeft(b, '0');
            }

            public List<string> uznat_znach_Stroca_Itog_V_Elem()
            {
                List<string> SP_rez = new List<string>();

                int ch_nach = 0;

                int ch_konechn = 0;

                int ch_skvozn = Nach_znach_chislo;

                for (int i = 0; i < D_SpFam_na_Numerac_Dla_Potr.Count; i++  )
                {

                    //////////////////////////////////////////////////////////

                    string pref1 = string.Empty;



                    if (!string.IsNullOrEmpty(perf) && perf.StartsWith("\"") && perf.EndsWith("\""))
                    {
                        try
                        {
                            pref1 = D_SpFam_na_Numerac_Dla_Potr[i].LookupParameter(perf.Replace("\"", "")).AsString();
                        }
                        catch { }
                    }

                    else
                    {
                        pref1 = perf;
                    }



                    ///////////////////////////////////////////////////////////

                    string Stroca_Itog_V_Elem = string.Empty;

                    ch_nach = ch_skvozn;

                    int Kol_adresov = 1;

                    try
                    {
                        Kol_adresov = D_SpFam_na_Numerac_Dla_Potr[i].Symbol.LookupParameter(D_Ima_Kol_Adresov).AsInteger();
                    }
                    catch { }

                    if (Kol_adresov <= 0)
                    {
                        Kol_adresov = 1;
                    }

                    ch_konechn = ch_skvozn + Kol_adresov - 1;

                    ch_skvozn = ch_skvozn + Kol_adresov;

                    if (ch_nach == ch_konechn)
                    {
                        string nomm = MetodStrIz_Int(ch_nach);

                        Stroca_Itog_V_Elem = pref1 + " " + nomm + sufics;

                    }

                    if (ch_nach != ch_konechn)
                    {
                        string nomm1 = MetodStrIz_Int(ch_nach);
                        string nomm2 = MetodStrIz_Int(ch_konechn);
                        Stroca_Itog_V_Elem = pref1 + " " + nomm1 + "-" + nomm2 + sufics;
                    }

                    SP_rez.Add(Stroca_Itog_V_Elem);
                }               

                return SP_rez;
            }


            public string MetodStrIz_Int(int a )
            {
                // Преобразуем число в строку и дополняем его до исходной длины, заданной forma_zapis
                return a.ToString().PadLeft(forma_zapis, '0');
            }


            public void ParseNumber() // Узнаем формат и первое значение
            {
                if (!string.IsNullOrEmpty(Nach_znach_Str))
                {
                    try
                    {
                        // Сохраняем общую длину исходной строки
                        forma_zapis = Nach_znach_Str.Length;

                        // Убираем ведущие нули для извлечения значения числа
                        string trimmed = Nach_znach_Str.TrimStart('0');

                        // Если строка состоит только из нулей, то приводим к значению 0
                        Nach_znach_chislo = trimmed.Length > 0 ? int.Parse(trimmed) : 0;
                    }
                    catch
                    {
                        // Обработка ошибок (оставляем forma_zapis и Nach_znach_chislo без изменений, если возникла ошибка)
                    }
                }
            }


            //public void ParseNumber() // Узнаем формат и первое значение
            //{

            //    if (Nach_znach_Str != string.Empty)
            //    {
            //        try
            //        {
            //            // Убираем все начальные нули
            //            string trimmed = Nach_znach_Str.TrimStart('0');

            //            // Считаем количество ведущих нулей
            //            int leadingZeros = Nach_znach_Str.Length - trimmed.Length;

            //            // Если все число состоит из нулей, то значение равно 0
            //            int parsedValue = trimmed.Length > 0 ? int.Parse(trimmed) : 0;

            //            // Возвращаем кортеж: (количество нулей, значение числа)

            //            forma_zapis = leadingZeros;

            //            Nach_znach_chislo = parsedValue;
            //        }
            //        catch { }
            //    }

            //}



        }

        public class Zagatovka_Numerac_Cepi
        {
            public int forma_zapis = 0; // Если 0 то 1,2,3,4 Если 1 то 01 ,02 , 03 и т.д..

            public string perf = string.Empty;

            public string sufics = string.Empty;

            public int Nach_znach_chislo = 1;

            public string Nach_znach_Str = string.Empty;

            public string Razd_Vnutr = "-RVNUTR-";

            public string Nazv_forma_zapis = "|Форма записи|";

            public string Nazv_Nach_znach_chislo = "|Начальное значение число|";

            public string Nazv_Nach_znach_stroka = "|Начальное значение строка|";

            public string Nazv_perf = "|Perfics|";

            public string Nazv_sufics = "|Sufics|";

            public string Stroca_Itog = D_Pust_Znach;

            public Zagatovka_Numerac_Cepi()
            { }

            public Zagatovka_Numerac_Cepi(string _Stroca_Itog)
            {
                Stroca_Itog = _Stroca_Itog;

                Raspacovka();
            }

            public Zagatovka_Numerac_Cepi(string _Nach_znach_Str, string _perf, string _sufics)
            {
                Nach_znach_Str = _Nach_znach_Str;
                ParseNumber();
                perf = _perf;
                sufics = _sufics;
                Zapakovka();
            }


            public void Raspacovka()
            {
                if ((string.IsNullOrEmpty(Stroca_Itog)) || (Stroca_Itog == D_Pust_Znach))

                { return; }

                Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik();


                try
                {
                    Nach_znach_Str = preobrazovatVSpravochnik[Nazv_Nach_znach_stroka];

                    ParseNumber();
                }
                catch { }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////

                try
                {
                    perf = preobrazovatVSpravochnik[Nazv_perf];
                }
                catch { }


                try
                {
                    sufics = preobrazovatVSpravochnik[Nazv_sufics];
                }
                catch { }

            }

            public void Zapakovka()
            {
                Stroca_Itog = Nazv_Nach_znach_stroka + Nach_znach_Str + Razd_Vnutr + Nazv_perf
                    + perf + Razd_Vnutr + Nazv_sufics
                    + sufics;
            }

            public Dictionary<string, string> PreobrazovatVSpravochnik()
            {
                // Список возможных ключей
                var keys = new List<string>
                {
                    Nazv_Nach_znach_stroka,
                    Nazv_perf,
                    Nazv_sufics

                };

                // Разбиваем строку на фрагменты по разделителю
                var segments = Stroca_Itog.Split(new[] { Razd_Vnutr }, StringSplitOptions.None);

                var result = new Dictionary<string, string>();

                // Обрабатываем каждую часть строки
                foreach (string key in keys)
                {
                    int fl = 0;

                    // Проверяем, содержит ли сегмент один из ключей
                    foreach (string segment in segments)
                    {
                        if (segment.Contains(key))
                        {
                            // Извлекаем значение (удаляем ключ из сегмента)
                            string value = segment.Replace(key, "").Trim();
                            // Добавляем пару ключ-значение в словарь
                            result[key] = value;
                            fl = 1;
                            break; // Ключ найден, дальше проверять смысла нет
                        }
                    }

                    if (fl == 0)
                    {
                        result[key] = D_Pust_Znach;
                    }
                }

                return result;
            }

            public string MetodStrIz_Int(int a, int b)
            {
                b++;
                // Форматирование с помощью PadLeft
                return a.ToString().PadLeft(b, '0');
            }

            public List<string> uznat_znach_Stroca_Itog_V_Cepach()
            {
                List<string> SP_rez = new List<string>();               

                int ch_skvozn = Nach_znach_chislo;

                for (int i = 0; i < D_SpEls_na_Numerac_Dla_Els.Count; i++)
                {

                    //////////////////////////////////////////////////////////

                    string pref1 = string.Empty;


                    if (!string.IsNullOrEmpty(perf) && perf.StartsWith("\"") && perf.EndsWith("\""))
                    {
                        try
                        {
                            pref1 = D_SpEls_na_Numerac_Dla_Els[i].LookupParameter(perf.Replace("\"", "")).AsString();
                        }
                        catch { }
                    }

                    else
                    {
                        pref1 = perf;
                    }

                    ///////////////////////////////////////////////////////////

                    string Stroca_Itog_V_Elem = string.Empty;                    

                    string nomm = MetodStrIz_Int(ch_skvozn);

                    Stroca_Itog_V_Elem = pref1 + " " + nomm + sufics;                    

                    SP_rez.Add(Stroca_Itog_V_Elem);

                    ch_skvozn++;
                }

                return SP_rez;
            }


            public string MetodStrIz_Int(int a)
            {
                // Преобразуем число в строку и дополняем его до исходной длины, заданной forma_zapis
                return a.ToString().PadLeft(forma_zapis, '0');
            }

            public void ParseNumber() // Узнаем формат и первое значение
            {
                if (!string.IsNullOrEmpty(Nach_znach_Str))
                {
                    try
                    {
                        // Сохраняем общую длину исходной строки
                        forma_zapis = Nach_znach_Str.Length;

                        // Убираем ведущие нули для извлечения значения числа
                        string trimmed = Nach_znach_Str.TrimStart('0');

                        // Если строка состоит только из нулей, то приводим к значению 0
                        Nach_znach_chislo = trimmed.Length > 0 ? int.Parse(trimmed) : 0;
                    }
                    catch
                    {
                        // Обработка ошибок (оставляем forma_zapis и Nach_znach_chislo без изменений, если возникла ошибка)
                    }
                }
            }
        }        

        public class Graf_Infa_Cep
        {
            public FamilyInstance famHost = null;

            public ElectricalSystem els = null;

            public int Id_Cpi = 0;

            public int Kol_Prov_Cepi = 1;

            public string Razd_Glob = "_RGL_";

            public string Razd_Vnutr = "-RVN-";

            public string Nazv_Id_Cep = "|Id Цепи|";

            public string Nazv_Kol_Prov_Cepi = "|Кол проводов|";          

            public string Stroca_Itog_Graf_Infa_Glob = string.Empty;

            public string Stroca_Itog_Graf_Infa_v_Param = string.Empty;

            public List<ElectricalSystem> Vse_systems = new List<ElectricalSystem>();

            public int Flag_na_redaktir = 0;

            public string Naim_Par = "ДСК1_Примечание";

            public string Znach_par = string.Empty; 

            public Graf_Infa_Cep(FamilyInstance _famHost , ElectricalSystem _els)
            {
                famHost = _famHost;
                els = _els;

                Id_Cpi = els.Id.IntegerValue;

                try
                {
                    Raspacovka();
                }
                catch { }

                NaytiVse_systems();
            }

            public Graf_Infa_Cep(FamilyInstance _famHost)
            {
                famHost = _famHost;

                NaytiVse_systems();
            }            


            public void NaytiVse_systems()
            {

               

                Vse_systems = new List<ElectricalSystem>();

                try
                {
                    Znach_par = famHost.LookupParameter(Naim_Par).AsString();

                    Stroca_Itog_Graf_Infa_Glob = ReadDataFromElement_posp(famHost as Element, MetaData_for_ElGraff_Data);

                    if (Stroca_Itog_Graf_Infa_Glob == null)
                    {
                        return;
                    }

                    if (Stroca_Itog_Graf_Infa_Glob == D_Pust_Znach)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(Stroca_Itog_Graf_Infa_Glob))
                    {
                        return;
                    }

                    var segments = Stroca_Itog_Graf_Infa_Glob.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                    foreach (var seg in segments)
                    {
                        try
                        {
                           
                                Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik(seg);

                                string id_gr = preobrazovatVSpravochnik[Nazv_Id_Cep];

                                int idel = int.Parse(id_gr);

                                ElectricalSystem elss = doc.GetElement(new ElementId(idel)) as ElectricalSystem;

                            if (elss != null)
                            {
                                Vse_systems.Add(elss);
                            }

                            else 
                            {
                                Flag_na_redaktir = 1;
                            }
                            
                        }
                        catch { }
                    }
                }

                catch { }

            }

            public void Raspacovka()
            {

                Stroca_Itog_Graf_Infa_Glob = ReadDataFromElement_posp( famHost as Element ,  MetaData_for_ElGraff_Data);               


                if (Stroca_Itog_Graf_Infa_Glob == null)
                {
                    return;                
                }

                if (Stroca_Itog_Graf_Infa_Glob == D_Pust_Znach)
                {
                    return;
                }

                if (string.IsNullOrEmpty(Stroca_Itog_Graf_Infa_Glob))
                {
                    return;
                }                

                var segments = Stroca_Itog_Graf_Infa_Glob.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                foreach (var seg in segments)
                {
                    if (seg.Contains(els.Id.IntegerValue.ToString()))
                    {
                        Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik(seg);

                        int.TryParse(preobrazovatVSpravochnik[Nazv_Kol_Prov_Cepi], out int numericId);

                        Kol_Prov_Cepi = numericId;                        
                    }
                }          

            }

            public void Zapakovka()
            {
                Stroca_Itog_Graf_Infa_Glob = ReadDataFromElement_posp(famHost as Element, MetaData_for_ElGraff_Data);

                string strcep = string.Empty;

                try
                {
                    strcep = Nazv_Id_Cep + els.Id.IntegerValue.ToString() + Razd_Vnutr + Nazv_Kol_Prov_Cepi + Kol_Prov_Cepi;
                }
                catch { }               

                if (Stroca_Itog_Graf_Infa_Glob != null)
                {
                    if ((Stroca_Itog_Graf_Infa_Glob != D_Pust_Znach) && (!string.IsNullOrEmpty(Stroca_Itog_Graf_Infa_Glob)))
                    {
                        List<string> SpStr1 = new List<string>();

                        var segments = Stroca_Itog_Graf_Infa_Glob.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                        foreach (var segment in segments)
                        {
                            if (!segment.Contains(els.Id.IntegerValue.ToString()))
                            {
                                Element elll = null;

                                try
                                {
                                    Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik(segment);

                                    int idseg = int.Parse(preobrazovatVSpravochnik[Nazv_Id_Cep]);

                                    elll = doc.GetElement(new ElementId(idseg));
                                }
                                catch { }

                                if (elll != null)
                                {
                                    SpStr1.Add(segment);
                                }
                            }                         
                                
                        }

                        if (!string.IsNullOrEmpty(strcep))
                        {
                            SpStr1.Add(strcep);
                        }

                        Stroca_Itog_Graf_Infa_Glob = string.Join(Razd_Glob, SpStr1);

                    }

                    else { Stroca_Itog_Graf_Infa_Glob = strcep; }
                }

                else { Stroca_Itog_Graf_Infa_Glob = strcep; }

            }

            public void Zapakovka1()
            {
                Stroca_Itog_Graf_Infa_Glob = ReadDataFromElement_posp(famHost as Element, MetaData_for_ElGraff_Data);               

                    
                        List<string> SpStr1 = new List<string>();

                        var segments = Stroca_Itog_Graf_Infa_Glob.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                        foreach (var segment in segments)
                        {                           
                                Element elll = null;                                

                                try
                                {
                                    Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik(segment);

                                    int idseg = int.Parse(preobrazovatVSpravochnik[Nazv_Id_Cep]);

                                    elll = doc.GetElement(new ElementId(idseg));
                                }
                                catch { }

                                if (elll != null)
                                {
                                    SpStr1.Add(segment);
                                }                            

                        }                        

                        Stroca_Itog_Graf_Infa_Glob = string.Join(Razd_Glob, SpStr1);               

            }


            public Dictionary<string, string> PreobrazovatVSpravochnik(string strr)
            {               

                // Список возможных ключей
                var keys = new List<string>
                {
                      Nazv_Id_Cep ,
                      Nazv_Kol_Prov_Cepi
                };

                // Разбиваем строку на фрагменты по разделителю
                var segments = strr.Split(new[] { Razd_Vnutr }, StringSplitOptions.None);

                var result = new Dictionary<string, string>();

                // Обрабатываем каждую часть строки
                foreach (string key in keys)
                {
                    int fl = 0;

                    // Проверяем, содержит ли сегмент один из ключей
                    foreach (string segment in segments)
                    {
                        if (segment.Contains(key))
                        {
                            // Извлекаем значение (удаляем ключ из сегмента)
                            string value = segment.Replace(key, "").Trim();
                            // Добавляем пару ключ-значение в словарь
                            result[key] = value;
                            fl = 1;
                            break; // Ключ найден, дальше проверять смысла нет
                        }
                    }

                    if (fl == 0)
                    {
                        result[key] = D_Pust_Znach;
                    }
                }

                return result;
            }

            public void Udalenie( int flag_trans) // Если 0 то не внутнри нтранзакции Если 1 то внутри Если 2 то без присвоению параметру.
            {
                Stroca_Itog_Graf_Infa_Glob = string.Empty;

                Stroca_Itog_Graf_Infa_Glob = ReadDataFromElement_posp(famHost as Element, MetaData_for_ElGraff_Data);              

                if (Stroca_Itog_Graf_Infa_Glob != null)
                {
                    if ((Stroca_Itog_Graf_Infa_Glob != D_Pust_Znach) || (string.IsNullOrEmpty(Stroca_Itog_Graf_Infa_Glob)))
                    {
                        List<string> SpStr1 = new List<string>();

                        var segments = Stroca_Itog_Graf_Infa_Glob.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                        foreach (var segment in segments)
                        {
                            if (!segment.Contains(els.Id.IntegerValue.ToString()))
                            {
                                SpStr1.Add(segment);
                            }                           

                            Stroca_Itog_Graf_Infa_Glob = string.Join(Razd_Glob, SpStr1);
                        }
                    }                    
                }

                nayty_Stroca_Itog_Graf_Infa_v_Param();

                SchemaSetData_univers_posp(famHost, MetaData_for_ElGraff_Data, Stroca_Itog_Graf_Infa_Glob);

                prisvit_par(flag_trans);
            }

            public void Udalenie(int flag_trans, string id_str)
            {
                Stroca_Itog_Graf_Infa_Glob = string.Empty;

                Stroca_Itog_Graf_Infa_Glob = ReadDataFromElement_posp(famHost as Element, MetaData_for_ElGraff_Data);

                if (Stroca_Itog_Graf_Infa_Glob != null)
                {
                    if ((Stroca_Itog_Graf_Infa_Glob != D_Pust_Znach) || (string.IsNullOrEmpty(Stroca_Itog_Graf_Infa_Glob)))
                    {
                        List<string> SpStr1 = new List<string>();

                        var segments = Stroca_Itog_Graf_Infa_Glob.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                        foreach (var segment in segments)
                        {
                            if (!segment.Contains (id_str))
                            {
                                SpStr1.Add(segment);
                            }

                            Stroca_Itog_Graf_Infa_Glob = string.Join(Razd_Glob, SpStr1);
                        }
                    }
                }

                nayty_Stroca_Itog_Graf_Infa_v_Param();

                SchemaSetData_univers_posp(famHost, MetaData_for_ElGraff_Data, Stroca_Itog_Graf_Infa_Glob);

                prisvit_par(flag_trans);


            }


            public void redactir_spisvoen(int flag_trans) // Если 0 то не внутнри нтранзакции Если 1 то внутри Если 2 то без присвоению параметру.
            {
                Zapakovka();

                SchemaSetData_univers_posp(famHost, MetaData_for_ElGraff_Data, Stroca_Itog_Graf_Infa_Glob);

                nayty_Stroca_Itog_Graf_Infa_v_Param();

                prisvit_par(flag_trans);
            }

            public void redactir_spisvoen1(int flag_trans) // Если 0 то не внутнри нтранзакции Если 1 то внутри Если 2 то без присвоению параметру.
            {
                Zapakovka1();

                SchemaSetData_univers_posp(famHost, MetaData_for_ElGraff_Data, Stroca_Itog_Graf_Infa_Glob);

                nayty_Stroca_Itog_Graf_Infa_v_Param();

                prisvit_par(flag_trans);
            }




            public void nayty_Stroca_Itog_Graf_Infa_v_Param()
            {
                Stroca_Itog_Graf_Infa_v_Param = string.Empty;

                List<string> strings = new List<string>();

                var segments = Stroca_Itog_Graf_Infa_Glob.Split(new[] { Razd_Glob }, StringSplitOptions.None);

                foreach (var segment in segments) 
                {
                    try
                    {
                        Dictionary<string, string> preobrazovatVSpravochnik = PreobrazovatVSpravochnik(segment);

                        int.TryParse(preobrazovatVSpravochnik[Nazv_Id_Cep], out int idcep);

                        Element el_cep = doc.GetElement(new ElementId(idcep));

                        string formir_nazv = el_cep.Id.IntegerValue.ToString();

                        try
                        {
                            string formir_nazv1 = (el_cep as ElectricalSystem).LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).AsString();

                            if (formir_nazv1 != null)
                            {
                                if (!string.IsNullOrEmpty(formir_nazv1))
                                {
                                    formir_nazv = formir_nazv1;
                                }
                            }
                        }
                        catch { }

                        int.TryParse(preobrazovatVSpravochnik[Nazv_Kol_Prov_Cepi], out int kol_prov);

                        if (kol_prov > 1)
                        {
                            formir_nazv = formir_nazv + " " + kol_prov.ToString() + "пр.";
                        }

                        if ((formir_nazv != "0") && (!string.IsNullOrEmpty(formir_nazv)))
                        {
                            strings.Add(formir_nazv);
                        }
                    }
                    catch { }
                }

                strings.Sort();

                Stroca_Itog_Graf_Infa_v_Param = string.Join("; ", strings);
            }

            public void prisvit_par(int flag_trans)

            {
                if (flag_trans == 0)
                {
                    using (Transaction t = new Transaction(doc, "Назначение параметру примечание марок кабелей"))
                    {
                        t.Start();

                           famHost.LookupParameter(Naim_Par).Set(Stroca_Itog_Graf_Infa_v_Param);

                        t.Commit();
                    }
                }

                if (flag_trans == 1)
                {
                    famHost.LookupParameter(Naim_Par).Set(Stroca_Itog_Graf_Infa_v_Param);
                }
            }
        }
    }
}
