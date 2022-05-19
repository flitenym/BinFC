using ExcelDataReader;
using Storage.Module.Import.Enums;
using Storage.Module.Import.Services.Interfaces;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace Storage.Module.Import.Services
{
    public class ImportService : IImportService
    {
        /// <summary>
        /// Получение датасета из excel контент файла.
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataSet GetDataSet(byte[] fileContent, string fileName)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = new MemoryStream(fileContent))
            {
                if (Path.GetExtension(fileName) == ".csv")
                {
                    using (IExcelDataReader reader = ExcelReaderFactory.CreateCsvReader(stream))
                    {
                        return reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                        });
                    }
                }
                else
                {
                    using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        return reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                        });
                    }
                }
            }
        }

        public async Task<(bool IsSuccess, string Error)> ImportAsync(byte[] fileContent, string fileName, ImportType importType)
        {
            DataSet dataSet = GetDataSet(fileContent, fileName);

            return default;
        }
    }
}
