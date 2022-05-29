using ExcelDataReader;
using Storage.Module.Entities.Base;
using Storage.Module.Import.Enums;
using Storage.Module.Import.Services.Interfaces;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace Storage.Module.Import.Services
{
    public class ImportService : IImportService
    {
        private readonly ISpotDataRepository _spotDataRepository;
        private readonly IFuturesDataRepository _futuresDataRepository;
        private readonly IBaseRepository _baseRepository;
        public ImportService(
            ISpotDataRepository spotDataRepository, 
            IFuturesDataRepository futuresDataRepository, 
            IBaseRepository baseRepository)
        {
            _spotDataRepository = spotDataRepository;
            _futuresDataRepository = futuresDataRepository;
            _baseRepository = baseRepository;
        }

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

        public IEnumerable<Data> GetImportData(DataSet dataSet)
        {
            if (dataSet.Tables.Count == 0)
            {
                return null;
            }

            List<Data> importData = new();

            var table = dataSet.Tables[0];

            DateTime loadingDate = DateTime.UtcNow;

            foreach (DataRow row in table.Rows)
            {
                Data data = new Data();

                data.UserId = long.Parse(row[row.Table.Columns[1].ColumnName].ToString());
                data.AgentEarnUsdt = decimal.Parse(row[row.Table.Columns[3].ColumnName].ToString().Replace(",", "").Replace('.', ','));
                data.LoadingDate = loadingDate;

                importData.Add(data);
            }

            return importData;
        }

        public async Task<(bool IsSuccess, string Error)> SaveImportDataAsync(IEnumerable<Data> importData, ImportType importType)
        {
            foreach(var data in importData)
            {
                switch (importType)
                {
                    case ImportType.Spot:
                        {
                            await _spotDataRepository.CreateAsync(data);
                            break;
                        }
                    case ImportType.Futures:
                        {
                            await _futuresDataRepository.CreateAsync(data);
                            break;
                        }
                }
            }

            string saveError = await _baseRepository.SaveChangesAsync();

            if (string.IsNullOrEmpty(saveError))
            {
                return (true, null);
            }

            return (false, saveError);
        }

        public async Task<(bool IsSuccess, string Error)> ImportAsync(byte[] fileContent, string fileName, ImportType importType)
        {
            DataSet dataSet = GetDataSet(fileContent, fileName);

            var importData = GetImportData(dataSet);

            if (importData == null)
            {
                return (false, "Нет данных в excel файле.");
            }

            return await SaveImportDataAsync(importData, importType);
        }
    }
}
