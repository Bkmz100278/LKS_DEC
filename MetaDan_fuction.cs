////using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections;
using static LKS_DEC.Data;
using static LKS_DEC.Vse_Classi;
using static LKS_DEC.DopFunc;
using System.Reflection;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;

namespace LKS_DEC
{ 
    public class MetaDan_fuction
    {
        public static void SchemaSetData_s_trans(Element element, string schemaGuid_string, string MetaPar, string MetaData) // Запись доп данных к элементу
        {

            Guid schemaGuid = new Guid(schemaGuid_string);

            // Проверка на наличие существующей схемы
            Schema schema = Schema.Lookup(schemaGuid);

            if (schema != null)
            {
                // Присвоение данных элементу
                using (Transaction t = new Transaction(doc, "Добавить данные Extensible Storage"))
                {
                    t.Start();

                    // Создание экземпляра Entity для работы с данными элемента
                    Entity entity = new Entity(schema);
                    // Задание значения поля

                    entity.Set(MetaPar, MetaData);

                    // Присваивание экземпляра Entity элементу
                    element.SetEntity(entity);

                    t.Commit();
                }
            }
        }

        public static void SchemaSetData_no_transcsion(Element element, string schemaGuid_string, string MetaPar, string MetaData) // Запись доп данных к элементу без транзакции
        {

            Guid schemaGuid = new Guid(schemaGuid_string);

            // Проверка на наличие существующей схемы
            Schema schema = Schema.Lookup(schemaGuid);

            if (schema != null)
            {
                // Присвоение данных элементу
                //using (Transaction t = new Transaction(doc, "Добавить данные Extensible Storage"))
                //{
                //    t.Start();

                // Создание экземпляра Entity для работы с данными элемента
                Entity entity = new Entity(schema);
                // Задание значения поля


                entity.Set(MetaPar, MetaData);

                try
                {
                    element.SetEntity(entity);
                }
                catch { }

            }
        }


        public static void SchemaSetData_univers(Element element, string schemaGuid_string, string MetaPar, string MetaData)
        {
            try
            {
                SchemaSetData_s_trans(element, schemaGuid_string, MetaPar, MetaData);
            }
            catch
            {
                try
                {
                    SchemaSetData_no_transcsion(element, schemaGuid_string, MetaPar, MetaData);
                }
                catch { }
            }
        }


        public static string ReadDataFromElement(Element element, string schemaGuid_string, string MetaPar)  // Чтение доп данных у элемента
        {

            string res = string.Empty;

            try
            {

                Guid schemaGuid = new Guid(schemaGuid_string);

                // Поиск схемы по GUID
                Schema schema = Schema.Lookup(schemaGuid);
                if (schema != null && element.GetEntity(schema).IsValid())
                {
                    // Чтение данных из экземпляра Entity элемента
                    Entity entity = element.GetEntity(schema);

                    string ggg = MetaPar;

                    if (entity.Schema.GetField(MetaPar) != null)
                    {
                        string storedData = entity.Get<string>(MetaPar);
                        return storedData;
                    }

                }
            }

            catch { }

            return res;
        }

        public static void DeleteAllDataElement(Element element) // Удаление доп данных у элемента
        {
            // Проверяем наличие Extensible Storage в элементе
            IList<Guid> schemaGuids = element.GetEntitySchemaGuids();

            // Начинаем транзакцию для удаления данных
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Удаление метаданных из элемента");

                foreach (var schemaGuid in schemaGuids)
                {
                    Schema schema = Schema.Lookup(schemaGuid);
                    // Удаляем данные по схеме из элемента
                    if (schema != null)
                    {
                        element.DeleteEntity(schema);
                    }
                }

                trans.Commit();
            }
        }

        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void CreateSchema(string schemaGuid_str, string schemaName_str, string schemaName_Par_Dan_str)  //  Универсальное создание схемы с одним параметром 
        {
            Guid schemaGuid = new Guid(schemaGuid_str);

            Schema schema = Schema.Lookup(schemaGuid);

            if (schema == null)
            {

                SchemaBuilder sb = new SchemaBuilder(schemaGuid);

                sb.SetSchemaName(schemaName_str);

                sb.AddSimpleField(schemaName_Par_Dan_str, typeof(string));

                _ = sb.Finish();
            }
        }

        public static void CreateSchema_posp(List<string> Meta_Sp)
        {
            string schemaGuid_str = Meta_Sp[0];
            string schemaName_str = Meta_Sp[1];
            string schemaName_Par_Dan_str = Meta_Sp[2];

            CreateSchema(schemaGuid_str, schemaName_str, schemaName_Par_Dan_str);
        }

        public static void SchemaSetData_univers_posp(Element element, List<string> Meta_Sp, string MetaData)
        {
            string schemaGuid_str = Meta_Sp[0];
            string schemaName_Par_Dan_str = Meta_Sp[2];

            SchemaSetData_univers(element, schemaGuid_str, schemaName_Par_Dan_str, MetaData);
        }

        public static string ReadDataFromElement_posp(Element element, List<string> Meta_Sp)
        {
            string res = null;

            try
            {
                string schemaGuid_str = Meta_Sp[0];
                string schemaName_Par_Dan_str = Meta_Sp[2];

                res = ReadDataFromElement(element, schemaGuid_str, schemaName_Par_Dan_str);
            }
            catch { }

            return res;
        }

        public static void Create_schema_vsech()  // Создание схем 
        {
            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Создание схем Meta данных");

                foreach (var schem in All_Metadan_in_model)
                {
                    try
                    {
                        CreateSchema_posp(schem);
                    }

                    catch { }       
                
                }                    

                trans.Commit();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////       

        public static string schemaGuid_for_Elem_NomCEP = "12f22e0d-afb2-82e3-bc47-a2b11f3b95e8"; // 

        public static string schemaName_for_Elem_NomCEP = "Data_for_Elem_NomCEP"; //

        public static string schemaPar_for_Elem_NomCEP = "Par_Data_for_Elem_NomCEP"; //

        public static List<string> MetaData_for_Elem_NomCEP = new List<string> { schemaGuid_for_Elem_NomCEP, 
            schemaName_for_Elem_NomCEP, schemaPar_for_Elem_NomCEP };

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string schemaGuid_for_Elem_Potr_Sp_Con = "12f22e0d-afb2-80e3-bc47-a3b11f3b44e1"; // 

        public static string schemaName_for_Elem_Potr_Sp_Con = "Data_for_Elem_Potr_Sp_Con"; //

        public static string schemaPar_for_Elem_Potr_Sp_Con = "Par_Data_for_Elem_Potr_Sp_Con"; //

        public static List<string> MetaData_for_Elem_Potr_Sp_Con = new List<string> { schemaGuid_for_Elem_Potr_Sp_Con, 
            schemaName_for_Elem_Potr_Sp_Con, schemaPar_for_Elem_Potr_Sp_Con };


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string schemaGuid_for_Cep_Dan = "12f22e0d-afb2-82e6-bc47-a2b11f3b11e8"; // 

        public static string schemaName_for_Cep_Dan = "Data_for_Elem_Cep_Dan"; //

        public static string schemaPar_for_Cep_Dan = "Par_Data_for_Cep_Dan"; //

        public static List<string> MetaData_for_Cep_Dan = new List<string> { schemaGuid_for_Cep_Dan, schemaName_for_Cep_Dan, schemaPar_for_Cep_Dan };

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////        

        public static string schemaGuid_for_ElGraff_Data = "12f22e1d-afb0-42e6-bc43-a2b54f3b33e1"; //  Это данные которые будут записываться в элемент графа - палку

        public static string schemaName_for_ElGraff_Data = "Data_for_ElGraf_Markirovka_El"; // здесь ошибка Нужно

        public static string schemaPar_for_ElGraff_Data = "Par_for_ElGraf_Data"; //  здесь ошибка Нужно Par_for_ElGraf_Data

        public static List<string> MetaData_for_ElGraff_Data = new List<string> { schemaGuid_for_ElGraff_Data, schemaName_for_ElGraff_Data, schemaPar_for_ElGraff_Data };

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<List<string>> All_Metadan_in_model = new List<List<string>>
        {
            MetaData_for_Elem_NomCEP,
            MetaData_for_Elem_Potr_Sp_Con,
            MetaData_for_Cep_Dan,
            MetaData_for_ElGraff_Data
        };



    }
}
