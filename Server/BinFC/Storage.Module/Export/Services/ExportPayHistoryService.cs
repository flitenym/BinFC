using Storage.Module.Entities;
using Storage.Module.Export.Models;
using Storage.Module.Export.Services.Interfaces;
using Storage.Module.Localization;
using Storage.Module.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Module.Export.Services
{
    public class ExportPayHistoryService : IExportPayHistoryService
    {
        private readonly IPayHistoryRepository _payHistoryRepository;
        public ExportPayHistoryService(
            IPayHistoryRepository payHistoryRepository)
        {
            _payHistoryRepository = payHistoryRepository;
        }

        public IEnumerable<ExportPayHistoryModel> GetPayHistories(long[] ids)
        {
            IEnumerable<PayHistory> payHistories = _payHistoryRepository.Get(ids);

            List<ExportPayHistoryModel> exportData = new();

            foreach (var payHistory in payHistories)
            {
                exportData.Add(new ExportPayHistoryModel(payHistory));
            }

            return exportData;
        }

        public byte[] GetFileContent(IEnumerable<ExportPayHistoryModel> exportData)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(StorageLoc.ExportColumns);

            foreach (ExportPayHistoryModel item in exportData)
            {
                builder.AppendLine($"{item.UserId},{item.UserName},{item.SendedSum},{item.SendedTime.ToString("dd.MM.yyyy")},{item.NumberPay}");
            }

            var data = Encoding.UTF8.GetBytes(builder.ToString());

            return Encoding.UTF8.GetPreamble().Concat(data).ToArray();
        }

        public Task<(bool IsSuccess, string Error, byte[] FileContent)> ExportAsync(long[] ids)
        {
            var exportData = GetPayHistories(ids);

            if (!exportData.Any())
            {
                return Task.FromResult<(bool, string, byte[])>((false, StorageLoc.ExportNoData, null));
            }

            byte[] fileContent = GetFileContent(exportData);

            if (fileContent == null)
            {
                return Task.FromResult<(bool, string, byte[])>((false, StorageLoc.ExportCanNotCreateFile, null));
            }

            return Task.FromResult<(bool, string, byte[])>((true, null, fileContent));
        }
    }
}