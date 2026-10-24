using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Autodesk.Revit.Attributes;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Autodesk.Revit.DB.Electrical;
using static LKS_DEC.Vse_Classi;
using static LKS_DEC.MyEventHandlerLKC;
using static LKS_DEC.Sem_Postr_Graff;


//


namespace LKS_DEC
{
    public class Data
    {
        public static double D_koef_dl = (1 / 304.8);


        public static UIApplication uiapp;
        public static UIDocument uidoc;
        public static Autodesk.Revit.ApplicationServices.Application app;
        public static Document doc;
        public static Selection sel;


        /////////////////////////////////////////////////////////////       



        public static  ElectricalSystemType[] D_SPelectricalSystemType = new[]
        {
             ElectricalSystemType.FireAlarm,
             ElectricalSystemType.Data,
             ElectricalSystemType.Security,
             ElectricalSystemType.Communication,
             ElectricalSystemType.Controls,
             ElectricalSystemType.Telephone,
             ElectricalSystemType.NurseCall,
             ElectricalSystemType.PowerCircuit
       };

        public static Dictionary<string, ElectricalSystemType> D_revitSystemsDict = new Dictionary<string, ElectricalSystemType>
        {
                { "Пожарная сигнализация", ElectricalSystemType.FireAlarm },
                { "Передача данных", ElectricalSystemType.Data },
                { "Сигнализация", ElectricalSystemType.Security },
                { "Система связи", ElectricalSystemType.Communication },
                { "Система управления", ElectricalSystemType.Controls },
                { "Телефонная система", ElectricalSystemType.Telephone },
                { "Система вызова персонала", ElectricalSystemType.NurseCall },
                { "Силовая цепь", ElectricalSystemType.PowerCircuit }
        };

        /////////////////////////////////////////////////////////////

        public static string D_str_var_podkl_kaskad = "Каскадом";

        public static string D_str_var_podkl_posled = "Последовательно";

        public static string D_str_var_podkl_zvezda = "Звездой";        

        public static string[] D_str_var_SP = new[]
        {
            D_str_var_podkl_posled,

            D_str_var_podkl_zvezda,

            D_str_var_podkl_kaskad ,         
           
        };

        /////////////////////////////////////////////////////////////////////////////////////////

        public static string D_Razd_Glob = "_RGL_";      

        public static string D_Razd_Vnutr = "-RVN-";

        public static string D_Razd_Vnutr2 = "-RVNN-";

        public static string D_Pust_Znach = "Пустое значение";

        public static List<ElectricalSystem> D_VseElectricalSystem;

        public static List<ElSet> D_VseEls;

        public static Dictionary<string, List<ElSet>> Dict_D_VseEls;

        public static List<ElSet> D_Els_Pokomplektu;

        public static List<string> D_Vse_Komplects;

        public static string D_Glob_Marka_Complect ; 

        /////////////////////////////////////////////////////////////////////////////////////////

        public static string D_Razd_v_markach1_Prob = "  ";
        public static string D_Razd_v_markach2_TiZap = "; ";
        public static string D_Razd_v_markach3_Zap = ", ";
        /////////////////////////////////////////////////////////////////////////////////////////

        public static string D_Ima_Par_Kompl = "ДСК1_Комплект Марка";

        public static string D_Ima_Par_Nomer_Linii_U_CEP = "Маркировка кабеля";

        public static string D_Ima_Par_Nomer_Linii = "ДСК1_Номер линии";

        public static string D_Ima_Par_Pozicia = "ДСК1_Позиция";

        public static string D_Ima_Par_Etag_str = "ДСК1_Этаж";

        public static string D_Ima_Par_Sekcia_str = "ДСК1_Секция";

        public static string D_Ima_Ima_Pomech = "ДСК1_Имя помещения";

        public static string D_Ima_Nom_Pomech_str = "ДСК1_Номер помещения";

        public static string D_Ima_Oboznach = "ДСК1_Обозначение";

        public static string D_Ima_Poryadk_Nomer = "ДСК1_Порядковый номер";

        public static string D_Ima_Kol_Adresov = "ФСК_Количество адресов";

        //////////////////////////////////////////////////////////////////////////////////////////

        public static string D_NazvVseMark_comboBox_Complect_Marka = "Все разделы";

        /// /////////////////////////////////////////////////////////////////////////////////////        

        public static List<El_Priemn_Fam> D_SpPriemn_Raboch;

        /// //////////////////////////////////////////////////////////////////////////////////////

        public static List<FamilyInstance> D_Sp_Potr;
        public static FamilyInstance D_Pan;

        public static string D_Typ_podkl;
        public static string D_Typ_System;

        public static string D_Ima_Cepi;

        public static MyEventHandler1 D_handler1;
        public static ExternalEvent D_exEvent1;

        public static MyEventHandler2 D_handler2;
        public static ExternalEvent D_exEvent2;

        public static MyEventHandler3 D_handler3;
        public static ExternalEvent D_exEvent3;

        public static MyEventHandler4 D_handler4;
        public static ExternalEvent D_exEvent4;

        public static MyEventHandler5 D_handler5;
        public static ExternalEvent D_exEvent5;

        public static MyEventHandler6 D_handler6;
        public static ExternalEvent D_exEvent6;

        public static MyEventHandler7 D_handler7;
        public static ExternalEvent D_exEvent7;

        /// ////////////////////////////////////////////////////////////////////////////////////// 

        public static string D_marker_kab_trass = "абельные трассы";

        /////////////////////////////////////////////////////////////////////////////////////////

        public static ElSet D_Rabocia_Gruppa_El;

        public static Zagatovka_Numerac_Potr D_zagatovka_Numerac_Potr_dla_prisv;

        public static Zagatovka_Numerac_Cepi D_zagatovka_Numerac_Cepey_dla_prisv;

        public static string D_naim_new_cep = "Новая цепь";

        public static int D_flag_chablon_numerac_potr = 0;

        public static int D_flag_chablon_numerac_cepey = 0;

        public static List<FamilyInstance> D_SpFam_na_Numerac_Dla_Potr;

        public static List<ElectricalSystem> D_SpEls_na_Numerac_Dla_Els;

        ///////////////////////////////////////////////////////////////////////////////////////// 
        // Здесь для опыта

        public static FamilyInstance D_Fam_Opit;

        public static string D_nazv_par_opit = D_Ima_Nom_Pomech_str;

        public static string D_znach_par_opit = string.Empty;


        ///////////////////////////////////////////////////////////////////////////////////////// 

        public static string D_nazv_sem_Provoda = "ДСК1_СВУ_Кабельные трассы_Горизонтальная_Эл.цепи";

       // public static List<Provod_Graff> D_vse_provoda  = new List<Provod_Graff>();

       // public static List<Potrebit_Graff> D_vse_fam_Potrebit_na_Vide = new List<Potrebit_Graff>();

        public static List<Potrebit_Graff> D_vse_fam_Potrebit_na_vsech_Vidach = new List<Potrebit_Graff>();

        public static double  D_toler_rav_point = 0.001;

        public static double D_toler_tocka_kon = 5000 * D_koef_dl;

        public static double D_toler_tocka_stoyak = 100 * D_koef_dl;
        ///////////////////////////////////////////////////////////////////////////////////////// 

        public static List<Stoyak> D_Vse_StoYak = new List<Stoyak>();

        public static string D_NazvSem_Sroyak = "ДСК1_ЭЛУ_СС_Направление кабеля";

        public static string D_NazvTyp_Sroyak_Uchodit = "На этаж";

        public static string D_NazvTyp_Sroyak_Prichodit = "С этажа";

        public static string D_NazvTyp_Sroyak_Prichodit_Uchodit = "Часть";

        public static string D_NazvPar_Ident = "ДСК1_Обозначение";

        /////////////////////////////////////////////////////////////////////////////////////////

        public static List<FamilyInstance> D_All_Provod_and_Stoyak = new List<FamilyInstance>(); // все провода и стояки в модели


        /////////////////////////////////////////////////////////////////////////////////////////

        public static List<Graff_Plan_View> D_Vse_Obrabat_Vidi = new List<Graff_Plan_View>();

        public static List<Provod_Graff>  D_vse_provoda_vse_vidi = new List<Provod_Graff>();

        /////////////////////////////////////////////////////////////////////////////////////////

        public static void load(ExternalCommandData commandData)

        {
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
        }

    }
}
