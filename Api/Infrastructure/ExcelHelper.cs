using ClosedXML.Excel;
using FastMember;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Api.Infrastructure
{
    public static class ExcelHelper
    {
        public static DataTable ToDataTable(string excelPath)
        {
            var table = new DataTable();

            using (var workBook = new XLWorkbook(excelPath))
            {
                //Read the first Sheet from Excel file.
                var workSheet = workBook.Worksheet(1);

                //Loop through the Worksheet rows.
                var firstRow = true;
                foreach (var row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (firstRow)
                    {
                        foreach (var cell in row.Cells())
                        {
                            table.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        table.Rows.Add();
                        var i = 0;
                        foreach (var cell in row.Cells())
                        {
                            table.Rows[table.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }
                    }
                }
            }

            return table;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> data, string name)
        {
            var table = new DataTable(name ?? typeof(T).Name);
            using (var reader = ObjectReader.Create(data))
            {
                table.Load(reader);
            }

            return table;
        }

        //public static DataTable ToDataTable<T>(IEnumerable<T> data, string name = "")
        //{
        //    PropertyDescriptorCollection properties =
        //        TypeDescriptor.GetProperties(typeof(T));
        //    DataTable table = new DataTable(name ?? typeof(T).Name);
        //    foreach (PropertyDescriptor prop in properties)
        //        table.Columns.Add(prop.DisplayName ?? prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //    foreach (T item in data)
        //    {
        //        DataRow row = table.NewRow();
        //        foreach (PropertyDescriptor prop in properties)
        //            row[prop.DisplayName ?? prop.Name] = prop.GetValue(item) ?? DBNull.Value;
        //        table.Rows.Add(row);
        //    }
        //    return table;
        //}

        public static byte[] GenerateExcel<T>(this IEnumerable<T> data, string name)
        {
            var table = ToDataTable<T>(data, name);

            using (var wb = new XLWorkbook(XLEventTracking.Disabled))
            {
                wb.Worksheets
                    .Add(table)
                    .ColumnsUsed()
                    .AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static void SaveExcel<T>(this IEnumerable<T> data, string name, string path)
        {
            var table = ToDataTable<T>(data, name);

            using (var wb = new XLWorkbook(XLEventTracking.Disabled))
            {
                wb.Worksheets
                    .Add(table)
                    .ColumnsUsed()
                    .AdjustToContents();

                wb.SaveAs($@"{path}\{name}.xlsx");
            }
        }
    }
}
