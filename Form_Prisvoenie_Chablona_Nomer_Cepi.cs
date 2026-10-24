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
using Autodesk.Revit.UI;
using static LKS_DEC.MyEventHandlerLKC;

namespace LKS_DEC.UI
{
    public partial class Form_Prisvoenie_Chablona_Nomer_Cepi : System.Windows.Forms.Form
    {
        public List<string> Spcombox_param = new List<string> { "Панель"};


        public Form_Prisvoenie_Chablona_Nomer_Cepi()
        {
            InitializeComponent();
            nach_dan();
        }


        private void Form_Prisvoenie_Chablona_Nomer_Cepi_Load(object sender, EventArgs e)
        {
            
        }

        public void nach_dan()
        {
            try
            {

                if (D_zagatovka_Numerac_Cepey_dla_prisv == null)
                {
                    D_zagatovka_Numerac_Cepey_dla_prisv = new Zagatovka_Numerac_Cepi();
                }

                comboBox1.Text = D_zagatovka_Numerac_Cepey_dla_prisv.perf;
                textBox_Nach_znach.Text = D_zagatovka_Numerac_Cepey_dla_prisv.Nach_znach_Str;
                textBox_Sufics.Text = D_zagatovka_Numerac_Cepey_dla_prisv.sufics;

            }
            catch { }

            comboBox1.Items.Clear();

            foreach (var item in Spcombox_param)
            {
                comboBox1.Items.Add($"\"{item}\""); // Добавляем строку в кавычках
            }
        }

        private void button_otmena_Click(object sender, EventArgs e)
        {
            D_flag_chablon_numerac_potr = 0;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            D_flag_chablon_numerac_cepey = 1;

          //  D_zagatovka_Numerac_Cepey_dla_prisv = new Zagatovka_Numerac_Cepi();

            D_zagatovka_Numerac_Cepey_dla_prisv =
                   new Zagatovka_Numerac_Cepi(textBox_Nach_znach.Text, comboBox1.Text, textBox_Sufics.Text);


            this.Close();
        }
    }
}
