namespace LKS_DEC
{
    partial class Form_Glavnoe_Okno
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.iGridtabl_Po_Panely = new TenTec.Windows.iGridLib.iGrid();
            this.panel_dla_Igrid_s_panel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_razbit_provod = new System.Windows.Forms.Button();
            this.button_Creat_cep = new System.Windows.Forms.Button();
            this.button_udal_el_iz_Nabora = new System.Windows.Forms.Button();
            this.textBox_ima_Pan = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_Typ_ElSyst = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_Typ_Podkl = new System.Windows.Forms.ComboBox();
            this.Vibor_elem_Cepi = new System.Windows.Forms.Button();
            this.Vibor_Panel = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button_cep_vniz = new System.Windows.Forms.Button();
            this.button_cep_vverch = new System.Windows.Forms.Button();
            this.button_nomerac_cepey_chablon = new System.Windows.Forms.Button();
            this.comboBox_Complect_Marka = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button_nomerac_priemn_chablon = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button_el_vverch = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel_dla_Igrid_s_cep = new System.Windows.Forms.Panel();
            this.iGrid_Po_Cepi = new TenTec.Windows.iGridLib.iGrid();
            this.iGAutoFilterManager1 = new TenTec.Windows.iGridLib.Filtering.iGAutoFilterManager(this.components);
            this.iGrid1DefaultCellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGrid1DefaultColHdrStyle = new TenTec.Windows.iGridLib.iGColHdrStyle(true);
            this.iGrid1RowTextColCellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGAutoFilterManager2 = new TenTec.Windows.iGridLib.Filtering.iGAutoFilterManager(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.button_dellet_gupp = new System.Windows.Forms.Button();
            this.button_obnovit_dan = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.iGridtabl_Po_Panely)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iGrid_Po_Cepi)).BeginInit();
            this.SuspendLayout();
            // 
            // iGridtabl_Po_Panely
            // 
            this.iGridtabl_Po_Panely.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iGridtabl_Po_Panely.Header.Height = 16;
            this.iGridtabl_Po_Panely.Location = new System.Drawing.Point(0, 0);
            this.iGridtabl_Po_Panely.Name = "iGridtabl_Po_Panely";
            this.iGridtabl_Po_Panely.Size = new System.Drawing.Size(200, 200);
            this.iGridtabl_Po_Panely.TabIndex = 0;
            // 
            // panel_dla_Igrid_s_panel
            // 
            this.panel_dla_Igrid_s_panel.Location = new System.Drawing.Point(3, 114);
            this.panel_dla_Igrid_s_panel.Name = "panel_dla_Igrid_s_panel";
            this.panel_dla_Igrid_s_panel.Size = new System.Drawing.Size(413, 295);
            this.panel_dla_Igrid_s_panel.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_razbit_provod);
            this.panel2.Controls.Add(this.button_Creat_cep);
            this.panel2.Controls.Add(this.button_udal_el_iz_Nabora);
            this.panel2.Controls.Add(this.textBox_ima_Pan);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.comboBox_Typ_ElSyst);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.comboBox_Typ_Podkl);
            this.panel2.Controls.Add(this.Vibor_elem_Cepi);
            this.panel2.Controls.Add(this.Vibor_Panel);
            this.panel2.Location = new System.Drawing.Point(881, 22);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(379, 473);
            this.panel2.TabIndex = 1;
            // 
            // button_razbit_provod
            // 
            this.button_razbit_provod.Location = new System.Drawing.Point(3, 325);
            this.button_razbit_provod.Name = "button_razbit_provod";
            this.button_razbit_provod.Size = new System.Drawing.Size(170, 23);
            this.button_razbit_provod.TabIndex = 9;
            this.button_razbit_provod.Text = "Разбить провод";
            this.button_razbit_provod.UseVisualStyleBackColor = true;
            this.button_razbit_provod.Click += new System.EventHandler(this.button_razbit_provod_Click);
            // 
            // button_Creat_cep
            // 
            this.button_Creat_cep.Location = new System.Drawing.Point(3, 273);
            this.button_Creat_cep.Name = "button_Creat_cep";
            this.button_Creat_cep.Size = new System.Drawing.Size(170, 23);
            this.button_Creat_cep.TabIndex = 8;
            this.button_Creat_cep.Text = "Построить цепь";
            this.button_Creat_cep.UseVisualStyleBackColor = true;
            this.button_Creat_cep.Click += new System.EventHandler(this.button_Creat_cep_Click);
            // 
            // button_udal_el_iz_Nabora
            // 
            this.button_udal_el_iz_Nabora.Location = new System.Drawing.Point(3, 210);
            this.button_udal_el_iz_Nabora.Name = "button_udal_el_iz_Nabora";
            this.button_udal_el_iz_Nabora.Size = new System.Drawing.Size(170, 23);
            this.button_udal_el_iz_Nabora.TabIndex = 7;
            this.button_udal_el_iz_Nabora.Text = "Удалить из выбора";
            this.button_udal_el_iz_Nabora.UseVisualStyleBackColor = true;
            this.button_udal_el_iz_Nabora.Click += new System.EventHandler(this.button_udal_el_iz_Nabora_Click);
            // 
            // textBox_ima_Pan
            // 
            this.textBox_ima_Pan.Location = new System.Drawing.Point(199, 123);
            this.textBox_ima_Pan.Name = "textBox_ima_Pan";
            this.textBox_ima_Pan.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBox_ima_Pan.Size = new System.Drawing.Size(141, 20);
            this.textBox_ima_Pan.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Тип электросистемы";
            // 
            // comboBox_Typ_ElSyst
            // 
            this.comboBox_Typ_ElSyst.FormattingEnabled = true;
            this.comboBox_Typ_ElSyst.Location = new System.Drawing.Point(3, 77);
            this.comboBox_Typ_ElSyst.Name = "comboBox_Typ_ElSyst";
            this.comboBox_Typ_ElSyst.Size = new System.Drawing.Size(165, 21);
            this.comboBox_Typ_ElSyst.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Тип подключения";
            // 
            // comboBox_Typ_Podkl
            // 
            this.comboBox_Typ_Podkl.FormattingEnabled = true;
            this.comboBox_Typ_Podkl.Location = new System.Drawing.Point(3, 27);
            this.comboBox_Typ_Podkl.Name = "comboBox_Typ_Podkl";
            this.comboBox_Typ_Podkl.Size = new System.Drawing.Size(165, 21);
            this.comboBox_Typ_Podkl.TabIndex = 2;
            // 
            // Vibor_elem_Cepi
            // 
            this.Vibor_elem_Cepi.Location = new System.Drawing.Point(3, 165);
            this.Vibor_elem_Cepi.Name = "Vibor_elem_Cepi";
            this.Vibor_elem_Cepi.Size = new System.Drawing.Size(170, 23);
            this.Vibor_elem_Cepi.TabIndex = 1;
            this.Vibor_elem_Cepi.Text = "Выбрать элементы цепи";
            this.Vibor_elem_Cepi.UseVisualStyleBackColor = true;
            this.Vibor_elem_Cepi.Click += new System.EventHandler(this.Vibor_elem_Cepi_Click);
            // 
            // Vibor_Panel
            // 
            this.Vibor_Panel.Location = new System.Drawing.Point(3, 120);
            this.Vibor_Panel.Name = "Vibor_Panel";
            this.Vibor_Panel.Size = new System.Drawing.Size(170, 23);
            this.Vibor_Panel.TabIndex = 0;
            this.Vibor_Panel.Text = "Выбрать панель";
            this.Vibor_Panel.UseVisualStyleBackColor = true;
            this.Vibor_Panel.Click += new System.EventHandler(this.Vibor_Panel_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button_obnovit_dan);
            this.panel3.Controls.Add(this.button_dellet_gupp);
            this.panel3.Controls.Add(this.button_cep_vniz);
            this.panel3.Controls.Add(this.button_cep_vverch);
            this.panel3.Controls.Add(this.button_nomerac_cepey_chablon);
            this.panel3.Controls.Add(this.comboBox_Complect_Marka);
            this.panel3.Controls.Add(this.panel_dla_Igrid_s_panel);
            this.panel3.Location = new System.Drawing.Point(12, 22);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(419, 473);
            this.panel3.TabIndex = 2;
            // 
            // button_cep_vniz
            // 
            this.button_cep_vniz.Location = new System.Drawing.Point(339, 14);
            this.button_cep_vniz.Name = "button_cep_vniz";
            this.button_cep_vniz.Size = new System.Drawing.Size(46, 34);
            this.button_cep_vniz.TabIndex = 9;
            this.button_cep_vniz.Text = "Вниз";
            this.button_cep_vniz.UseVisualStyleBackColor = true;
            this.button_cep_vniz.Click += new System.EventHandler(this.button_cep_vniz_Click);
            // 
            // button_cep_vverch
            // 
            this.button_cep_vverch.Location = new System.Drawing.Point(264, 14);
            this.button_cep_vverch.Name = "button_cep_vverch";
            this.button_cep_vverch.Size = new System.Drawing.Size(46, 34);
            this.button_cep_vverch.TabIndex = 8;
            this.button_cep_vverch.Text = "Вверх";
            this.button_cep_vverch.UseVisualStyleBackColor = true;
            this.button_cep_vverch.Click += new System.EventHandler(this.button_cep_vverch_Click);
            // 
            // button_nomerac_cepey_chablon
            // 
            this.button_nomerac_cepey_chablon.Location = new System.Drawing.Point(17, 61);
            this.button_nomerac_cepey_chablon.Name = "button_nomerac_cepey_chablon";
            this.button_nomerac_cepey_chablon.Size = new System.Drawing.Size(107, 37);
            this.button_nomerac_cepey_chablon.TabIndex = 7;
            this.button_nomerac_cepey_chablon.Text = "Шаблон номерац цепей";
            this.button_nomerac_cepey_chablon.UseVisualStyleBackColor = true;
            this.button_nomerac_cepey_chablon.Click += new System.EventHandler(this.button_nomerac_cepey_chablon_Click);
            // 
            // comboBox_Complect_Marka
            // 
            this.comboBox_Complect_Marka.FormattingEnabled = true;
            this.comboBox_Complect_Marka.Location = new System.Drawing.Point(17, 20);
            this.comboBox_Complect_Marka.Name = "comboBox_Complect_Marka";
            this.comboBox_Complect_Marka.Size = new System.Drawing.Size(115, 21);
            this.comboBox_Complect_Marka.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button_nomerac_priemn_chablon);
            this.panel4.Controls.Add(this.button1);
            this.panel4.Controls.Add(this.button_el_vverch);
            this.panel4.Controls.Add(this.comboBox1);
            this.panel4.Controls.Add(this.panel_dla_Igrid_s_cep);
            this.panel4.Location = new System.Drawing.Point(434, 22);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(441, 473);
            this.panel4.TabIndex = 3;
            // 
            // button_nomerac_priemn_chablon
            // 
            this.button_nomerac_priemn_chablon.Location = new System.Drawing.Point(20, 61);
            this.button_nomerac_priemn_chablon.Name = "button_nomerac_priemn_chablon";
            this.button_nomerac_priemn_chablon.Size = new System.Drawing.Size(107, 37);
            this.button_nomerac_priemn_chablon.TabIndex = 6;
            this.button_nomerac_priemn_chablon.Text = "Шаблон номерац потребителей";
            this.button_nomerac_priemn_chablon.UseVisualStyleBackColor = true;
            this.button_nomerac_priemn_chablon.Click += new System.EventHandler(this.button_nomerac_priemn_chablon_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(311, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(46, 34);
            this.button1.TabIndex = 5;
            this.button1.Text = "Вниз";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_el_vverch
            // 
            this.button_el_vverch.Location = new System.Drawing.Point(239, 12);
            this.button_el_vverch.Name = "button_el_vverch";
            this.button_el_vverch.Size = new System.Drawing.Size(46, 34);
            this.button_el_vverch.TabIndex = 4;
            this.button_el_vverch.Text = "Вверх";
            this.button_el_vverch.UseVisualStyleBackColor = true;
            this.button_el_vverch.Click += new System.EventHandler(this.button_el_vverch_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(20, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(165, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // panel_dla_Igrid_s_cep
            // 
            this.panel_dla_Igrid_s_cep.Location = new System.Drawing.Point(0, 114);
            this.panel_dla_Igrid_s_cep.Name = "panel_dla_Igrid_s_cep";
            this.panel_dla_Igrid_s_cep.Size = new System.Drawing.Size(441, 295);
            this.panel_dla_Igrid_s_cep.TabIndex = 0;
            // 
            // iGrid_Po_Cepi
            // 
            this.iGrid_Po_Cepi.Header.Height = 16;
            this.iGrid_Po_Cepi.Location = new System.Drawing.Point(0, 0);
            this.iGrid_Po_Cepi.Name = "iGrid_Po_Cepi";
            this.iGrid_Po_Cepi.Size = new System.Drawing.Size(200, 200);
            this.iGrid_Po_Cepi.TabIndex = 0;
            // 
            // iGAutoFilterManager1
            // 
            this.iGAutoFilterManager1.Grid = this.iGrid_Po_Cepi;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(329, 523);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "label3";
            // 
            // button_dellet_gupp
            // 
            this.button_dellet_gupp.Location = new System.Drawing.Point(17, 431);
            this.button_dellet_gupp.Name = "button_dellet_gupp";
            this.button_dellet_gupp.Size = new System.Drawing.Size(115, 23);
            this.button_dellet_gupp.TabIndex = 10;
            this.button_dellet_gupp.Text = "Удалить группы";
            this.button_dellet_gupp.UseVisualStyleBackColor = true;
            this.button_dellet_gupp.Click += new System.EventHandler(this.button_dellet_gupp_Click);
            // 
            // button_obnovit_dan
            // 
            this.button_obnovit_dan.Location = new System.Drawing.Point(158, 431);
            this.button_obnovit_dan.Name = "button_obnovit_dan";
            this.button_obnovit_dan.Size = new System.Drawing.Size(115, 23);
            this.button_obnovit_dan.TabIndex = 11;
            this.button_obnovit_dan.Text = "Обновить данные";
            this.button_obnovit_dan.UseVisualStyleBackColor = true;
            this.button_obnovit_dan.Click += new System.EventHandler(this.button_obnovit_dan_Click);
            // 
            // Form_Glavnoe_Okno
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1286, 548);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "Form_Glavnoe_Okno";
            this.Text = "Form_Glavnoe_Okno";
            this.Load += new System.EventHandler(this.Form_Glavnoe_Okno_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iGridtabl_Po_Panely)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iGrid_Po_Cepi)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TenTec.Windows.iGridLib.iGrid iGridtabl_Po_Panely;
        private System.Windows.Forms.Panel panel_dla_Igrid_s_panel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button Vibor_elem_Cepi;
        private System.Windows.Forms.Button Vibor_Panel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_Typ_Podkl;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comboBox_Complect_Marka;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Panel panel_dla_Igrid_s_cep;
        private TenTec.Windows.iGridLib.iGrid iGrid_Po_Cepi;
        private System.Windows.Forms.Button button_el_vverch;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_Typ_ElSyst;
        private System.Windows.Forms.TextBox textBox_ima_Pan;
        private System.Windows.Forms.Button button_udal_el_iz_Nabora;
        private TenTec.Windows.iGridLib.Filtering.iGAutoFilterManager iGAutoFilterManager1;
        private System.Windows.Forms.Button button_Creat_cep;
        private TenTec.Windows.iGridLib.iGCellStyle iGrid1DefaultCellStyle;
        private TenTec.Windows.iGridLib.iGColHdrStyle iGrid1DefaultColHdrStyle;
        private TenTec.Windows.iGridLib.iGCellStyle iGrid1RowTextColCellStyle;
        private TenTec.Windows.iGridLib.Filtering.iGAutoFilterManager iGAutoFilterManager2;
        private System.Windows.Forms.Button button_nomerac_priemn_chablon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_nomerac_cepey_chablon;
        private System.Windows.Forms.Button button_cep_vniz;
        private System.Windows.Forms.Button button_cep_vverch;
        private System.Windows.Forms.Button button_razbit_provod;
        private System.Windows.Forms.Button button_dellet_gupp;
        private System.Windows.Forms.Button button_obnovit_dan;
    }
}