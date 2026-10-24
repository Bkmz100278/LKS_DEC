using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TenTec.Windows.iGridLib; // Обязательно подключаем библиотеку iGrid.NET
using static LKS_DEC.Data;
using static LKS_DEC.DopFunc;
using static LKS_DEC.DopFunc2;
using static LKS_DEC.Vse_Classi;
using static LKS_DEC.MetaDan_fuction;
using System.Xml.Linq;
using TenTec.Windows.iGridLib.Filtering;
using System.Reflection.Emit;
using System.Reflection;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB.Electrical;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using LKS_DEC.UI;
using static LKS_DEC.MyEventHandlerLKC;


namespace LKS_DEC
{

    public partial class Form_Glavnoe_Okno : System.Windows.Forms.Form
    {

        public int hhh = 0;

        public List<FamilyInstance> SelFams_From = new List<FamilyInstance>(); // Это вот для текущего выбора нескольких элементов

        public FamilyInstance Sel1_Fam_From = null; // Это вот для текущего выбора 1го элемента

        public List<El_Priemn_Fam> SpPriemn = new List<El_Priemn_Fam>();  //  Это рабочий набор для работы с таблицей на вход

        public List<string> Sp_El_Syst = D_revitSystemsDict.Keys.ToList();

        public List<string> Sp_Ima_Panels = new List<string>(); 
        
        public Form_Glavnoe_Okno()
        {
            InitializeComponent();

            AddItemsToComboBoxElSyst();

            InitializeGrid_Po_1Cepi();

            InitializeGrid_Po_Vsem_Cep();

            LoadComboBox();

            D_zagatovka_Numerac_Potr_dla_prisv = new Zagatovka_Numerac_Potr();

            D_zagatovka_Numerac_Cepey_dla_prisv = new Zagatovka_Numerac_Cepi();

            D_flag_chablon_numerac_potr = 0;

            D_flag_chablon_numerac_cepey = 0;

            D_SpFam_na_Numerac_Dla_Potr = new List<FamilyInstance>();

            D_SpEls_na_Numerac_Dla_Els = new List<ElectricalSystem>();
        }        

        private void LoadComboBox()
        {
            // Очистка ComboBox на случай, если данные уже были загружены
            comboBox_Typ_Podkl.Items.Clear();

            // Загрузка значений из массива
            comboBox_Typ_Podkl.Items.AddRange(D_str_var_SP);

            // Установка первого значения в качестве выбранного
            if (comboBox_Typ_Podkl.Items.Count > 0)
            {
                comboBox_Typ_Podkl.SelectedIndex = 0; // Выбирает первый элемент списка
            }
            // Запрет ввода пользователем значений, не входящих в список
            comboBox_Typ_Podkl.DropDownStyle = ComboBoxStyle.DropDownList;
        }        

        public void crenie_izapis_potrebitForm()
        {
            SelFams_From = crenie_izapis_potrebit(comboBox_Typ_ElSyst.Text);

            if (SelFams_From.Count == 0)
            { return; }

            int flag_na_Zapoln = 0;


            if (SpPriemn.Count > 0)
            {
                chtenie_aktualn_SP_priemn();

                flag_na_Zapoln = 1;

                List<FamilyInstance> SelFams_From11 = SpPriemn.Select(x => x.famPriemn).ToList();

                var result = SelFams_From11.Concat(SelFams_From).ToList();

                SelFams_From = result;
            }

            SelFams_From = SelFams_From
           .GroupBy(x => x.Id.IntegerValue)
           .Select(g => g.First())
           .ToList();

            if ((Sel1_Fam_From != null) && (flag_na_Zapoln == 0))
            {
                SelFams_From = GetSortedSpPotr(SelFams_From, Sel1_Fam_From);
            }

            SpPriemn.Clear();

            SpPriemn = SelFams_From.Select(x => new El_Priemn_Fam(x)).ToList();            

            LoadDataToGrid();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form_Glavnoe_Okno_Load(object sender, EventArgs e)
        {

        }       

        private void Vibor_Panel_Click(object sender, EventArgs e)  
        {
            Sel1_Fam_From = Vibor_pan();

            string str1 = string.Empty;

            string str2 = string.Empty;

            try
            {
                str1 = Sel1_Fam_From.Symbol.LookupParameter(D_Ima_Oboznach).AsString();
            }
            catch { }

            try
            {
                str2 = Sel1_Fam_From.LookupParameter(D_Ima_Poryadk_Nomer).AsString(); 
            }
            catch { }

            textBox_ima_Pan.Text = str1+ "-" + str2;

            if (Sel1_Fam_From != null)
            {                
                    comboBox_Typ_Podkl.Enabled = true;

                    comboBox_Typ_ElSyst.Enabled = true;

                    comboBox1.Text = D_naim_new_cep; 
            }

            if (Sel1_Fam_From != null)
            {
                iGrid_Po_Cepi.Rows.Clear();

                D_Rabocia_Gruppa_El = null;

                D_zagatovka_Numerac_Potr_dla_prisv = null;
            }
        }

        private void AddItemsToComboBoxElSyst()
        {
            // Очищаем ComboBox, чтобы избежать дублирования
            comboBox_Typ_ElSyst.Items.Clear();

            // Добавляем элементы из списка в ComboBox
            foreach (string item in Sp_El_Syst)
            {
                comboBox_Typ_ElSyst.Items.Add(item);
            }

            // Устанавливаем первый элемент в качестве выбранного (по желанию)
            if (comboBox_Typ_ElSyst.Items.Count > 0)
            {
                comboBox_Typ_ElSyst.SelectedIndex = 0; // Индекс 0 — первый элемент
            }
        }

        private void InitializeGrid_Po_1Cepi()
        {
            panel_dla_Igrid_s_cep.Controls.Add(iGrid_Po_Cepi);
            iGrid_Po_Cepi.Dock = DockStyle.Fill; //

            // Удалим все колонки перед добавлением, если они есть

            iGrid_Po_Cepi.Cols.Clear();

            // Добавляем три столбца в iGrid
            iGrid_Po_Cepi.Cols.Add("Номер потребит.");  // imaPriemn
            iGrid_Po_Cepi.Cols.Add("Имя элемента");  // imaPriemn
            iGrid_Po_Cepi.Cols.Add("Секция/Этаж");  // SecEtag
            iGrid_Po_Cepi.Cols.Add("Имя помещения"); // Ima_pom
            iGrid_Po_Cepi.Cols.Add("HiddenIndex", "").Visible = false;
            iGrid_Po_Cepi.Cols.Add("Id элемента");
            iGrid_Po_Cepi.Cols.Add("+");
            iGrid_Po_Cepi.Cols.Add("Схема/вид");

            // Дополнительно можно задать ширину столбцов или другие параметры
            iGrid_Po_Cepi.Cols[0].Width = 50; // Ширина первой колонки
            iGrid_Po_Cepi.Cols[1].Width = 150; // Ширина первой колонки
            iGrid_Po_Cepi.Cols[2].Width = 100; // Ширина второй колонки
            iGrid_Po_Cepi.Cols[3].Width = 150; // Ширина третьей колонки
            iGrid_Po_Cepi.Cols[5].Width = 200; // Ширина третьей колонки
            iGrid_Po_Cepi.Cols[6].Width = 200;
            iGrid_Po_Cepi.Cols[7].Width = 200; 

            iGAutoFilterManager1.FilterBoxClosed += iGAutoFilterManager1_FilterApplied2;

            iGrid_Po_Cepi.SelectionEndChange += IGrid_Po_Cepi_SelectionEndChange;

            iGrid_Po_Cepi.SelectionMode = iGSelectionMode.MultiExtended;

            iGrid_Po_Cepi.RowMode = true;
            iGrid_Po_Cepi.RowSelectionInCellMode = TenTec.Windows.iGridLib.iGRowSelectionInCellModeTypes.MultipleRows;

            iGrid_Po_Cepi.SelectionChanged += SelectionChangedVsem_ElemPotr;

        }

        private void LoadDataToGrid()
        {
            // Очищаем данные iGrid перед заполнением
            iGrid_Po_Cepi.Rows.Clear();

            // Перебираем каждый объект из списка elPriemnFamList
            //foreach (var el in SpPriemn)

            for (int i = 0; i < SpPriemn.Count; i++)

            {
                var el = SpPriemn[i];

                // Проверяем значение свойства: если равно D_Pust_Znach, подставляем пустую строку
                string imaPriemn = el.imaPriemn == D_Pust_Znach ? "" : el.imaPriemn;
                string secEtag = el.SecEtag == D_Pust_Znach ? "" : el.SecEtag;
                string imaPom = el.Ima_pom == D_Pust_Znach ? "" : el.Ima_pom;

                // Добавляем данные в строку таблицы iGrid
                var newRow = iGrid_Po_Cepi.Rows.Add(); // Добавляем строку

                // Заполняем ячейки строки значениями
                newRow.Cells[0].Value = i + 1;
                newRow.Cells[1].Value = imaPriemn; // Первая ячейка (имеет индекс [0])
                newRow.Cells[2].Value = secEtag;   // Вторая ячейка (имеет индекс [1])
                newRow.Cells[3].Value = imaPom;    // Третья ячейка (имеет индекс [2])
                newRow.Cells[4].Value = i;    //
                newRow.Cells[5].Value = el.famPriemn.Id.IntegerValue.ToString();    //
                newRow.Cells[6].Value = el.Kol_adresov;               

            }
        }

        private void SelectionChangedVsem_ElemPotr(object sender, EventArgs e)
        {
            if (iGrid_Po_Cepi.SelectedRows.Count > 0)
            {
                try
                {
                    List<ElementId> newSelection = new List<ElementId>();
                    // Добавляем элементы в список

                    foreach (iGRow rou in iGrid_Po_Cepi.SelectedRows)
                    {
                        iGCell igcell = iGrid_Po_Cepi.Cells[rou.Index, 5];

                        var idstr = igcell.Value.ToString();

                        int idint = int.Parse(idstr);

                        Element els = doc.GetElement(new ElementId(idint));

                        newSelection.Add(els.Id);
                    }

                    // Устанавливаем новый выбор
                    uidoc.Selection.SetElementIds(newSelection);
                }
                catch { }
            }
        }

        /// /////////////////////////////////////////////////////////////////////////////////////////

        private void InitializeGrid_Po_Vsem_Cep()  // Это создание по всем цепям
        {

            panel_dla_Igrid_s_panel.Controls.Add(iGridtabl_Po_Panely);

            iGridtabl_Po_Panely.Dock = DockStyle.Fill; //

            // Удалим все колонки перед добавлением, если они есть
            iGridtabl_Po_Panely.Cols.Clear();

            // Добавляем три столбца в iGrid
            iGridtabl_Po_Panely.Cols.Add("Пока_Номер");  // imaPriemn
            iGridtabl_Po_Panely.Cols.Add("Марка кабеля");  // imaPriemn
            iGridtabl_Po_Panely.Cols.Add("Начало");  // SecEtag
            iGridtabl_Po_Panely.Cols.Add("Конец"); // Ima_pom
           //  iGridtabl_Po_Panely.Cols.Add("HiddenIndex", "").Visible = false;
            iGridtabl_Po_Panely.Cols.Add("HiddenIndex", "");

            // Дополнительно можно задать ширину столбцов или другие параметры
            iGridtabl_Po_Panely.Cols[0].Width = 50; // Ширина первой колонки
            iGridtabl_Po_Panely.Cols[1].Width = 150; // Ширина первой колонки
            iGridtabl_Po_Panely.Cols[2].Width = 150; // Ширина второй колонки
            iGridtabl_Po_Panely.Cols[3].Width = 150; // Ширина третьей колонки            
            iGridtabl_Po_Panely.Cols[4].Width = 200; // Ширина третьей колонки 

            //iGAutoFilterManager1.FilterBoxClosed += iGAutoFilterManager1_FilterApplied2;

            //iGrid_Po_Cepi.SelectionEndChange += IGrid_Po_Cepi_SelectionEndChange;

            LoadDataToGrid2_samoe_nachalo();

            D_Glob_Marka_Complect = D_NazvVseMark_comboBox_Complect_Marka;

            D_VseEls = D_VseElectricalSystem.Select(x => new ElSet(x)).ToList();  // Все цепи в проекте
            ConvertListToDictionary();

            D_Vse_Komplects = new List<string>();

            D_Vse_Komplects.Add(D_NazvVseMark_comboBox_Complect_Marka);

            foreach (var el in Dict_D_VseEls)
            {
                D_Vse_Komplects.Add(el.Key);
            }

            D_Vse_Komplects.Sort();           



            comboBox_Complect_Marka.Items.AddRange(D_Vse_Komplects.ToArray());
            comboBox_Complect_Marka.SelectedIndexChanged += SetComboBoxValueWithAddition;

            comboBox_Complect_Marka.SelectedIndex = 0;

            iGAutoFilterManager2.Grid = iGridtabl_Po_Panely;


            iGridtabl_Po_Panely.SelectionMode = iGSelectionMode.MultiExtended;

            iGridtabl_Po_Panely.RowMode = true;
            iGridtabl_Po_Panely.RowSelectionInCellMode = TenTec.Windows.iGridLib.iGRowSelectionInCellModeTypes.MultipleRows;

            iGridtabl_Po_Panely.SelectionChanged += SelectionChangedVsem_Cep;     


        }

        private void SelectionChangedVsem_Cep(object sender, EventArgs e)
        {
            int topmostIndex = int.MaxValue;

            // Проверяем, есть ли выбранные строки
            if (iGridtabl_Po_Panely.SelectedRows.Count >0)                 
                {
                    // Находим минимальный индекс выбранной строки
                    
                    foreach (iGRow rou in iGridtabl_Po_Panely.SelectedRows)
                    {
                        if (rou.Index < topmostIndex)                      
                        
                        topmostIndex = rou.Index;
                    }

                try 
                {
                    List<ElementId> newSelection = new List<ElementId>();
                    // Добавляем элементы в список

                    foreach (iGRow rou in iGridtabl_Po_Panely.SelectedRows)
                    {
                        iGCell igcell = iGridtabl_Po_Panely.Cells[rou.Index, 4];

                        var idstr = igcell.Value.ToString();

                        int idint = int.Parse(idstr);

                        ElectricalSystem els = doc.GetElement(new ElementId(idint)) as ElectricalSystem;

                        newSelection.Add(els.Id);
                    }                   
                  
                    // Устанавливаем новый выбор
                    uidoc.Selection.SetElementIds(newSelection);
                }
                catch { }

            }

            if (topmostIndex < int.MaxValue)
            {
                try
                {
                    iGrid_Po_Cepi.Rows.Clear();

                    iGCell igcell = iGridtabl_Po_Panely.Cells[topmostIndex, 4];

                    var idstr = igcell.Value.ToString();

                    int idint = int.Parse(idstr);

                    ElectricalSystem els = doc.GetElement(new ElementId(idint)) as ElectricalSystem ;

                    ElSet elset = new ElSet(els);

                    comboBox1.Text = elset.Nomer_Linii;

                    var elSp = elset.ListPotr;                    

                    if (elSp.Count > 0)
                    {
                        SpPriemn.Clear();

                        foreach (var el1 in elSp)
                        {
                            string ima = el1.Symbol.FamilyName;

                            if (!ima.Contains(D_marker_kab_trass))
                            {
                                SpPriemn.Add(new El_Priemn_Fam(el1));                                

                            }                            
                        }
                    }

                    LoadDataToGrid();

                    var chit = elset.BaseEq;

                    string str1 =string.Empty;
                    string str2 = string.Empty;

                    try
                    {
                        str1 = chit.Symbol.LookupParameter(D_Ima_Oboznach).AsString();
                    }
                    catch { }

                    try
                    {
                        str2 = chit.LookupParameter(D_Ima_Poryadk_Nomer).AsString();
                    }
                    catch { }

                    /////////////////////////////////////////////////////////////////////////////

                    textBox_ima_Pan.Text = str1 + "-" + str2;                   

                   /////////////////////////////////////////////////////////////////////////////

                    int index = comboBox_Typ_Podkl.FindStringExact(D_str_var_podkl_posled);
                    if (index != -1)
                    {
                        comboBox_Typ_Podkl.SelectedIndex = index;
                        comboBox_Typ_Podkl.Enabled = false;
                    }
                    

                    /////////////////////////////////////////////////////////////////////////////

                    string str3 = elset.TypSystemi; 

                    int index1 = comboBox_Typ_ElSyst.FindStringExact(elset.TypSystemi);
                    if (index1 != -1)
                    {
                        comboBox_Typ_ElSyst.SelectedIndex = index1;
                        comboBox_Typ_ElSyst.Enabled = false;
                    }

                    Sel1_Fam_From = null;

                    D_Rabocia_Gruppa_El = elset;

                    D_zagatovka_Numerac_Potr_dla_prisv = null;

                    /////////////////////////////////////////////////////////////////////////////
                }

                catch { }               
            }
        }

        private void Zagotovka_k_SelectionChangedVsem_Cep( int topmostIndex  )
        {
             try 
             {
                    List<ElementId> newSelection = new List<ElementId>();
                    // Добавляем элементы в список

                    foreach (iGRow rou in iGridtabl_Po_Panely.SelectedRows)
                    {
                        iGCell igcell = iGridtabl_Po_Panely.Cells[rou.Index, 4];

                        var idstr = igcell.Value.ToString();

                        int idint = int.Parse(idstr);

                        ElectricalSystem els = doc.GetElement(new ElementId(idint)) as ElectricalSystem;

                        newSelection.Add(els.Id);
                    }                   
                  
                    // Устанавливаем новый выбор
                    uidoc.Selection.SetElementIds(newSelection);
             }
             catch { }

            if (topmostIndex < int.MaxValue)
            {
                try
                {
                    iGrid_Po_Cepi.Rows.Clear();

                    iGCell igcell = iGridtabl_Po_Panely.Cells[topmostIndex, 4];

                    var idstr = igcell.Value.ToString();

                    int idint = int.Parse(idstr);

                    ElectricalSystem els = doc.GetElement(new ElementId(idint)) as ElectricalSystem;

                    ElSet elset = new ElSet(els);

                    comboBox1.Text = elset.Nomer_Linii;

                    var elSp = elset.ListPotr;

                    if (elSp.Count > 0)
                    {
                        SpPriemn.Clear();

                        foreach (var el1 in elSp)
                        {
                            string ima = el1.Symbol.FamilyName;

                            if (!ima.Contains(D_marker_kab_trass))
                            {
                                SpPriemn.Add(new El_Priemn_Fam(el1));

                            }
                        }
                    }

                    LoadDataToGrid();

                    var chit = elset.BaseEq;

                    string str1 = string.Empty;
                    string str2 = string.Empty;

                    try
                    {
                        str1 = chit.Symbol.LookupParameter(D_Ima_Oboznach).AsString();
                    }
                    catch { }

                    try
                    {
                        str2 = chit.LookupParameter(D_Ima_Poryadk_Nomer).AsString();
                    }
                    catch { }

                    /////////////////////////////////////////////////////////////////////////////

                    textBox_ima_Pan.Text = str1 + "-" + str2;

                    /////////////////////////////////////////////////////////////////////////////

                    int index = comboBox_Typ_Podkl.FindStringExact(D_str_var_podkl_posled);
                    if (index != -1)
                    {
                        comboBox_Typ_Podkl.SelectedIndex = index;
                        comboBox_Typ_Podkl.Enabled = false;
                    }


                    /////////////////////////////////////////////////////////////////////////////

                    string str3 = elset.TypSystemi;

                    int index1 = comboBox_Typ_ElSyst.FindStringExact(elset.TypSystemi);
                    if (index1 != -1)
                    {
                        comboBox_Typ_ElSyst.SelectedIndex = index1;
                        comboBox_Typ_ElSyst.Enabled = false;
                    }

                    Sel1_Fam_From = null;

                    D_Rabocia_Gruppa_El = elset;

                    D_zagatovka_Numerac_Potr_dla_prisv = null;

                    /////////////////////////////////////////////////////////////////////////////
                }

                catch { }
            }       
        
        }

        private void SetComboBoxValueWithAddition(object sender, EventArgs e)
        {
            //obnovit_dan();
            LoadDataToGrid_perefomatirovanie_po_komplektu();
        }
       
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////       

        private void LoadDataToGrid_perefomatirovanie_po_komplektu() // Это загрузка данных по всем цепям
        {
            string zach_kompl_marka = comboBox_Complect_Marka.Text;

            if (zach_kompl_marka != D_NazvVseMark_comboBox_Complect_Marka)
            {

                D_VseElectricalSystem = VseCepi();
                D_VseEls = D_VseElectricalSystem.Select(x => new ElSet(x)).ToList();  // Все цепи в проекте
                ConvertListToDictionary();

                D_Els_Pokomplektu = Dict_D_VseEls[zach_kompl_marka];

                D_Els_Pokomplektu.Sort(CompareElSets);

                // Очищаем данные iGrid перед заполнением
                iGridtabl_Po_Panely.Rows.Clear();


                if (D_VseEls.Count > 0)
                {
                    for (int i = 0; i < D_Els_Pokomplektu.Count; i++)

                    {
                        try
                        {
                            var el = D_Els_Pokomplektu[i];

                            string imaKab = D_Pust_Znach;

                            try
                            {
                                imaKab = el.Nomer_Linii;
                            }
                            catch { }

                            string nach = D_Pust_Znach;

                            try
                            {
                                nach = el.BaseEq.Symbol.LookupParameter(D_Ima_Oboznach).AsString() + "_" + el.BaseEq.LookupParameter(D_Ima_Poryadk_Nomer).AsString();
                            }

                            catch { }

                            string konec = D_Pust_Znach;

                            try

                            {
                                konec = el.ListPotr.Last().Symbol.LookupParameter(D_Ima_Oboznach).AsString() + "_" + el.ListPotr.Last().LookupParameter(D_Ima_Poryadk_Nomer).AsString();
                            }

                            catch { }

                            // Добавляем данные в строку таблицы iGrid

                            var newRow = iGridtabl_Po_Panely.Rows.Add(); // Добавляем строку


                            // Заполняем ячейки строки значениями
                            newRow.Cells[0].Value = (i + 1).ToString();
                            newRow.Cells[1].Value = imaKab; // Первая ячейка (имеет индекс [0])
                            newRow.Cells[2].Value = nach;   // Вторая ячейка (имеет индекс [1])
                            newRow.Cells[3].Value = konec;    // Третья ячейка (имеет индекс [2])
                            newRow.Cells[4].Value = el.el_syst.Id.IntegerValue.ToString();      // Четвертая ячейка (имеет индекс [2])
                        }
                        catch { }
                    }
                }
            }

            else 
            {
                LoadDataToGrid2_samoe_nachalo();
            }
        }

        private void obnovit_dan()
        {
            LoadDataToGrid_perefomatirovanie_po_komplektu();           

            D_zagatovka_Numerac_Potr_dla_prisv = new Zagatovka_Numerac_Potr();

            D_zagatovka_Numerac_Cepey_dla_prisv = new Zagatovka_Numerac_Cepi();

            D_flag_chablon_numerac_potr = 0;

            D_flag_chablon_numerac_cepey = 0;

            D_SpFam_na_Numerac_Dla_Potr = new List<FamilyInstance>();

            D_SpEls_na_Numerac_Dla_Els = new List<ElectricalSystem>();

            iGrid_Po_Cepi.Rows.Clear();            

            comboBox1.Text = string.Empty;            

            /////////////////////////////////////////////////////////////////////////////

            textBox_ima_Pan.Text = string.Empty;

            /////////////////////////////////////////////////////////////////////////////           

            D_Rabocia_Gruppa_El = null;            

            comboBox_Typ_Podkl.SelectedIndex = 0;
            comboBox_Typ_ElSyst.SelectedIndex = 0;

            D_Typ_podkl = comboBox_Typ_Podkl.Text;
            D_Typ_System = comboBox_Typ_ElSyst.Text;

            comboBox_Typ_Podkl.Enabled = true;
            comboBox_Typ_ElSyst.Enabled = true;
        }


        public void ConvertListToDictionary()
        {
            Dict_D_VseEls = new Dictionary<string, List<ElSet>>();

            // Проверяем введенный список
            if (D_VseEls == null || D_VseEls.Count == 0)
            {
                // Возврат пустого словаря, если список пустой
                return;
            }

            try
            {

                // Перед преобразованием проверяем каждую `Kompl_marka` и заменяем пустое значение
                foreach (var item in D_VseEls)
                {
                    if (string.IsNullOrEmpty(item.Kompl_marka))
                    {
                        item.Kompl_marka = D_Pust_Znach; // Присваиваем значение глобальной переменной
                    }
                }

                // Группируем элементы списка по Kompl_marka и создаем словарь
                var resultDictionary = D_VseEls
                    .GroupBy(item => item.Kompl_marka)
                    .ToDictionary(group => group.Key, group => group.ToList());

                Dict_D_VseEls = resultDictionary;
            }
            catch (Exception ex) { }
        }

        private void LoadDataToGrid2_samoe_nachalo() // Это загрузка данных по всем цепям
        {
            D_VseElectricalSystem = VseCepi();
            D_VseEls = D_VseElectricalSystem.Select(x => new ElSet(x)).ToList();  // Все цепи в проекте
            D_VseEls.Sort(CompareElSets);

            // Очищаем данные iGrid перед заполнением
            iGridtabl_Po_Panely.Rows.Clear();

            if (D_VseEls.Count > 0)
            {
                
                    for (int i = 0; i < D_VseEls.Count; i++)

                    {
                    try
                    {
                        var el = D_VseEls[i];

                        string imaKab = D_Pust_Znach;

                        try
                        {

                            imaKab = el.Nomer_Linii;
                        }
                        catch { }

                        string nach = D_Pust_Znach;

                        try

                        {
                            nach = el.BaseEq.Symbol.LookupParameter(D_Ima_Oboznach).AsString() + "_" + el.BaseEq.LookupParameter(D_Ima_Poryadk_Nomer).AsString();
                        }

                        catch { }

                        string konec = D_Pust_Znach;

                        try

                        {
                            konec = el.ListPotr.Last().Symbol.LookupParameter(D_Ima_Oboznach).AsString() + "_" + el.ListPotr.Last().LookupParameter(D_Ima_Poryadk_Nomer).AsString();
                        }

                        catch { }                       

                        // Добавляем данные в строку таблицы iGrid

                        var newRow = iGridtabl_Po_Panely.Rows.Add(); // Добавляем строку


                        // Заполняем ячейки строки значениями
                        newRow.Cells[0].Value = i + 1;
                        newRow.Cells[1].Value = imaKab; // Первая ячейка (имеет индекс [0])
                        newRow.Cells[2].Value = nach;   // Вторая ячейка (имеет индекс [1])
                        newRow.Cells[3].Value = konec;    // Третья ячейка (имеет индекс [2])
                        newRow.Cells[4].Value = el.el_syst.Id.IntegerValue.ToString();     // 
                    }
                    catch { }    

                 }
                
            }
        }


        /// //////////////////////////////////////////////////////////////////////////////////
       
        public static int CompareElSets(ElSet x, ElSet y) // Это про сортировку  
        {
            // Сначала сравниваем по imaBaseEq
            int result = CompareStringsByCustomLogic(x.imaBaseEq, y.imaBaseEq);
            if (result != 0)
            {
                return result;
            }

            // Если они равны, сравниваем по Nomer_Linii
            return CompareStringsByCustomLogic(x.Nomer_Linii, y.Nomer_Linii);
        }   

        public static int CompareStringsByCustomLogic(string str1, string str2)
        {
            // Извлекаем текстовую часть и числовую часть из обеих строк
            var part1 = ExtractTextAndNumberParts(str1);
            var part2 = ExtractTextAndNumberParts(str2);

            // Сравниваем текстовые части (по алфавиту)
            int textComparison = string.Compare(part1.Text, part2.Text, StringComparison.Ordinal);
            if (textComparison != 0)
            {
                return textComparison;
            }

            // Сравниваем числовые части (как числа)
            return part1.Number.CompareTo(part2.Number);
        }

        public static (string Text, int Number) ExtractTextAndNumberParts(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return ("", 0);
            }

            // Регулярное выражение для разделения текста и чисел
            var match = Regex.Match(input, @"^(.*?)(\d+)?$");
            if (match.Success)
            {
                string textPart = match.Groups[1].Value; // Текстовая часть
                string numberPart = match.Groups[2].Value; // Числовая часть

                // Если числовая часть найдена, преобразуем её в число, убирая ведущие нули
                int number = 0;
                if (!string.IsNullOrEmpty(numberPart))
                {
                    number = int.Parse(numberPart.TrimStart('0'));
                }

                return (textPart, number);
            }

            // Если не удалось ничего найти — возвращаем исходную строку как текст и 0 как число
            return (input, 0);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////

        private void IGrid_Po_Cepi_SelectionEndChange(object sender, iGSelectionStartEndChangeEventArgs e)
        {
            Univers_sbros_schetchika();
        }
               
        private void iGAutoFilterManager1_FilterApplied2(object sender, EventArgs e)
        {
            Univers_sbros_schetchika();
        }

        private void Univers_sbros_schetchika()
        {
            hhh = 1; // Сбрасываем счётчик

            foreach (iGRow row in iGrid_Po_Cepi.Rows)
            {
                if (row.Visible)
                {
                    iGrid_Po_Cepi.Cells[row.Index, 0].Value = hhh;
                    hhh++;
                }
            }
        }
       

        private void Vibor_elem_Cepi_Click(object sender, EventArgs e)
        {
            crenie_izapis_potrebitForm();
        }      

        

        private void MoveRowUp(iGrid grid)
        {
            // int rowIndex = grid.SelectedRows[0].Index;

            // int rowIndex = grid.SelectedCells[0].RowIndex;   //CurrentCell.RowIndex;

            int rowIndex = grid.SelectedRows[0].Index;           


            int rowIndex_nach = grid.SelectedRows[0].Index;


            // Проверяем, что строка существует и её можно переместить выше
            if (rowIndex <= 0 || rowIndex >= grid.Rows.Count)
                return;

            //// Меняем элементы в списке местами
            //var item = SpPriemn[rowIndex];
            //SpPriemn.RemoveAt(rowIndex);
            //SpPriemn.Insert(rowIndex - 1, item);

            // Меняем строки местами
            for (int colIndex = 0; colIndex < grid.Cols.Count; colIndex++)
            {
                var temp = grid.Rows[rowIndex].Cells[colIndex].Value;
                grid.Rows[rowIndex].Cells[colIndex].Value = grid.Rows[rowIndex - 1].Cells[colIndex].Value;
                grid.Rows[rowIndex - 1].Cells[colIndex].Value = temp;
            }

            SetRowIndexes(grid);

            grid.Rows[rowIndex_nach ].Selected = false;

            grid.Rows[rowIndex_nach-1].Selected = true;
        }           

        // Метод для перемещения строки вниз
        private void MoveRowDown(iGrid grid)
        {

            int rowIndex = grid.SelectedRows[0].Index;    

            int rowIndex_nach = grid.SelectedRows[0].Index;


            // Проверяем, что строку можно переместить вниз
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count - 1)
                return;         


            // Меняем строки местами
            for (int colIndex = 0; colIndex < grid.Cols.Count; colIndex++)     //ColumnCount; colIndex++)
            {
                var temp = grid.Rows[rowIndex].Cells[colIndex].Value;
                grid.Rows[rowIndex].Cells[colIndex].Value = grid.Rows[rowIndex + 1].Cells[colIndex].Value;
                grid.Rows[rowIndex + 1].Cells[colIndex].Value = temp;
            }

            SetRowIndexes(grid);

            grid.Rows[rowIndex_nach].Selected =false;

            grid.Rows[rowIndex_nach + 1].Selected = true;
        }

        public void SetRowIndexes(iGrid grid)
        {
            // Проверяем, что таблица не пустая
            if (grid == null || grid.Rows.Count == 0)
                return;

            // Проходим по всем строкам таблицы
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                // Присваиваем значение индекса + 1 (чтобы начать с 1)
                  grid.Cells[i, 0].Value = (i + 1).ToString();
            }
        }

        private void button_el_vverch_Click(object sender, EventArgs e)
        {
            MoveRowUp(iGrid_Po_Cepi);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MoveRowDown(iGrid_Po_Cepi);
        }

        private void button_udal_el_iz_Nabora_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (iGRow igrrow in iGrid_Po_Cepi.SelectedRows)
                {
                    iGrid_Po_Cepi.Rows.RemoveAt(igrrow.Index);

                }
            }
            catch { }       
        }

        

        private void chtenie_aktualn_SP_priemn()
        {
            SpPriemn.Clear();

            List<El_Priemn_Fam> SpPriemn1 = new List<El_Priemn_Fam>();

            if (iGrid_Po_Cepi.Rows.Count != 0)
            {
                List<iGRow> sortedRows = iGrid_Po_Cepi.Rows.Cast<iGRow>() // Преобразуем коллекцию строк в Enumerable для удобной работы
            .Where(row => row.Visible) // Фильтруем только видимые строки
            .Select(row => new
            {
                Row = row, // Сохраняем строку
                Index = int.TryParse(row.Cells[0].Value?.ToString(), out var index) ? index : int.MaxValue
                // Пробуем преобразовать значение в числовой индекс. Если не вышло, ставим максимальное значение, чтобы такие строки находились в конце.
            })
                    .OrderBy(item => item.Index) // Сортируем по числовому значению индекса
                    .Select(item => item.Row) // Извлекаем обратно строки iGrid
                    .ToList(); // Преобразуем в список


                foreach ( iGRow igrow in sortedRows)

                try 
                {
                    string str = iGrid_Po_Cepi.Cells[igrow.Index, 5].Text ;
                    FamilyInstance el = doc.GetElement( new ElementId (int.Parse(str))) as FamilyInstance;
                    SpPriemn.Add(new El_Priemn_Fam(el));
                }
                catch { }           
            
            }     
           
        }

        private void button_Creat_cep_Click(object sender, EventArgs e)
        {
            chtenie_aktualn_SP_priemn();

            D_Sp_Potr = new List<FamilyInstance>();            

            foreach (var el in SpPriemn)
            {
                D_Sp_Potr.Add(el.famPriemn);
            }

            D_Pan = Sel1_Fam_From;
            D_Typ_podkl = comboBox_Typ_Podkl.Text;
            D_Typ_System = comboBox_Typ_ElSyst.Text;

            D_Ima_Cepi = "Временно";

            if (D_zagatovka_Numerac_Potr_dla_prisv == null)
            {
                D_zagatovka_Numerac_Potr_dla_prisv = new Zagatovka_Numerac_Potr();
            }

            if ((D_flag_chablon_numerac_potr == 0) && (D_Rabocia_Gruppa_El != null))
            {
                D_zagatovka_Numerac_Potr_dla_prisv = D_Rabocia_Gruppa_El.zagatovka_Numerac_Potr;
            }

           // Numerac_Vnutri_Tabl();

            if (D_Pan != null)
            {
                try
                {
                    D_handler1.WorkContextRevitFinish += FinishWork;
                    D_exEvent1.Raise();
                }
                catch { }   
            }

            else
            {

                D_handler2.WorkContextRevitFinish += FinishWork;
                D_exEvent2.Raise();
            }           
        }

        public void FinishWork(object ob, TestEventArgs args)
        {
            try
            {
                object param = args.Parameter;
                if (param != null)
                {
                    try
                    {                       
                        LoadDataToGrid_perefomatirovanie_po_komplektu();

                        comboBox1.Text = D_Rabocia_Gruppa_El.Nomer_Linii;

                        uidoc.Selection.SetElementIds( new List<ElementId> { D_Rabocia_Gruppa_El.el_syst.Id });

                    }
                    catch { }


                    if (D_Typ_podkl == D_str_var_podkl_posled)
                    {
                        if (D_Rabocia_Gruppa_El == null)
                        {
                            label3.Text = "Цепь не создана";
                        }

                        else
                        {
                            label3.Text = D_Rabocia_Gruppa_El.el_syst.Id.IntegerValue.ToString();

                        }
                    }

                    else 
                    {
                        D_Rabocia_Gruppa_El = null;

                        D_Pan = null;

                        iGrid_Po_Cepi.Rows.Clear();

                        textBox_ima_Pan.Text = string.Empty;

                        comboBox1.Text = string.Empty;  

                    }


                    Sel1_Fam_From = null;

                    textBox_ima_Pan.Text = string.Empty;

                }
            }
            finally
            {
                //отписываемся от события
                D_handler1.WorkContextRevitFinish -= FinishWork;
                D_handler2.WorkContextRevitFinish -= FinishWork;
            }
        }

        private void button_nomerac_priemn_chablon_Click(object sender, EventArgs e)
        {
            D_flag_chablon_numerac_potr = 0;

            D_SpFam_na_Numerac_Dla_Potr = new List<FamilyInstance>();            

            if (iGrid_Po_Cepi.SelectedRows.Count > 0)
            {
                foreach (iGRow r in iGrid_Po_Cepi.SelectedRows)
                {
                    if (r.Visible == true)
                    { 
                        var strfam = iGrid_Po_Cepi.Cells[r.Index, 5].Value as string;
                        FamilyInstance el = doc.GetElement(new ElementId(int.Parse(strfam))) as FamilyInstance;

                        D_SpFam_na_Numerac_Dla_Potr.Add(el);
                    }
                }                

                Form_Prisvoenie_Chablona_Nomer_Potr form_Prisvoenie_Chablona_Nomer_Potr = new Form_Prisvoenie_Chablona_Nomer_Potr();

                form_Prisvoenie_Chablona_Nomer_Potr.ShowDialog();
            }

            if (D_flag_chablon_numerac_potr == 1)
            {
                try
                {
                    D_handler3.WorkContextRevitFinish += FinishWork3;
                    D_exEvent3.Raise();
                }
                catch { }
            }           

        }

        public void FinishWork3(object ob, TestEventArgs args)
        {
            try
            {
                object param = args.Parameter;
                if (param != null)
                {
                    Numerac_Vnutri_Tabl_Po_Potrebit();
                    LoadDataToGrid_perefomatirovanie_po_komplektu();
                }
            }
            finally
            {
                //отписываемся от события
                D_handler3.WorkContextRevitFinish -= FinishWork3;
            }
        }

        // D_SpFam_na_Numerac

        private void Numerac_Vnutri_Tabl_Po_Potrebit()
        {
            try
            {
                foreach (iGRow r in iGrid_Po_Cepi.SelectedRows)
                {
                    if (r.Visible == true)
                    {
                        var strfam = iGrid_Po_Cepi.Cells[r.Index, 5].Value as string;
                        FamilyInstance el = doc.GetElement(new ElementId(int.Parse(strfam))) as FamilyInstance;

                        foreach (FamilyInstance fam in D_SpFam_na_Numerac_Dla_Potr)
                        {
                            if (fam.Id.IntegerValue == el.Id.IntegerValue)
                            {
                                string str_oboznach = el.Symbol.LookupParameter(D_Ima_Oboznach).AsString() + "-" + el.LookupParameter(D_Ima_Poryadk_Nomer).AsString();

                                iGrid_Po_Cepi.Cells[r.Index, 1].Value = str_oboznach;
                            }                        
                        }
                    }
                }

                D_SpFam_na_Numerac_Dla_Potr.Clear();
            }

            catch { }
        }

        private void button_nomerac_cepey_chablon_Click(object sender, EventArgs e)
        {
            D_flag_chablon_numerac_cepey = 0;

            D_SpEls_na_Numerac_Dla_Els = new List<ElectricalSystem>();

            if ( iGridtabl_Po_Panely.SelectedRows.Count > 0)
            {
                foreach (iGRow r in iGridtabl_Po_Panely.SelectedRows)
                {
                    if (r.Visible == true)
                    {
                        var strfam = iGridtabl_Po_Panely.Cells[r.Index, 4].Value as string;
                        ElectricalSystem el = doc.GetElement(new ElementId(int.Parse(strfam))) as ElectricalSystem;

                        D_SpEls_na_Numerac_Dla_Els.Add(el);
                    }
                }

                Form_Prisvoenie_Chablona_Nomer_Cepi form_Prisvoenie_Chablona_Nomer_Cepi = new Form_Prisvoenie_Chablona_Nomer_Cepi();

                form_Prisvoenie_Chablona_Nomer_Cepi.ShowDialog();
            }

            if (D_flag_chablon_numerac_cepey == 1)
            {
                try
                {
                    D_handler4.WorkContextRevitFinish += FinishWork4;
                    D_exEvent4.Raise();
                }
                catch { }
            }
        }

        public void FinishWork4(object ob, TestEventArgs args)
        {
            try
            {
                object param = args.Parameter;
                if (param != null)
                {
                    Numerac_Vnutri_Tabl_Po_Cepyam();
                }
            }
            finally
            {
                //отписываемся от события
                D_handler4.WorkContextRevitFinish -= FinishWork4;
            }
        }

        public void FinishWork6(object ob, TestEventArgs args)
        {
            try
            {
                object param = args.Parameter;
                if (param != null)
                {
                    obnovit_dan();

                    // LoadDataToGrid_perefomatirovanie_po_komplektu(); 
                }
            }
            finally
            {
                //отписываемся от события
                D_handler6.WorkContextRevitFinish -= FinishWork6;
            }
        }



        private void Numerac_Vnutri_Tabl_Po_Cepyam()
        {
            try
            {
                foreach (iGRow r in iGridtabl_Po_Panely.SelectedRows)  

                    if (r.Visible == true)
                    {
                        var strfam = iGridtabl_Po_Panely.Cells[r.Index, 4].Value as string;

                        ElectricalSystem el = doc.GetElement(new ElementId(int.Parse(strfam))) as ElectricalSystem;

                        foreach (ElectricalSystem els in D_SpEls_na_Numerac_Dla_Els)
                        {
                            if (els.Id.IntegerValue == el.Id.IntegerValue)
                            {
                                string str_oboznach = els.LookupParameter(D_Ima_Par_Nomer_Linii_U_CEP).AsString();

                                iGridtabl_Po_Panely.Cells[r.Index, 1].Value = str_oboznach;

                            }
                        }
                    }                

                D_SpEls_na_Numerac_Dla_Els.Clear();
            }

            catch { }
        }

        private void button_cep_vverch_Click(object sender, EventArgs e)
        {
            MoveRowUp(iGridtabl_Po_Panely);
        }

        private void button_cep_vniz_Click(object sender, EventArgs e)
        {
            MoveRowDown(iGridtabl_Po_Panely);
        }

        private void button_razbit_provod_Click(object sender, EventArgs e)
        {

            try
            {
                //D_handler4.WorkContextRevitFinish += FinishWork4;
                D_exEvent5.Raise();
            }
            catch { }

        }

        private void button_dellet_gupp_Click(object sender, EventArgs e)
        {
            D_SpEls_na_Numerac_Dla_Els = new List<ElectricalSystem>();

            if (iGridtabl_Po_Panely.SelectedRows.Count > 0)
            {
                foreach (iGRow r in iGridtabl_Po_Panely.SelectedRows)
                {
                    if (r.Visible == true)
                    {
                        var strfam = iGridtabl_Po_Panely.Cells[r.Index, 4].Value as string;
                        ElectricalSystem el = doc.GetElement(new ElementId(int.Parse(strfam))) as ElectricalSystem;

                        D_SpEls_na_Numerac_Dla_Els.Add(el);
                    }
                }
            }

            if (D_SpEls_na_Numerac_Dla_Els.Count > 0)
            {
                try
                {
                    D_handler6.WorkContextRevitFinish += FinishWork6;
                    D_exEvent6.Raise();
                }
                catch { }
            }
        }

        private void button_obnovit_dan_Click(object sender, EventArgs e)
        {
            try
            {               
                D_exEvent7.Raise();
            }
            catch { }

            obnovit_dan();
        }
    }   
}
