using Cronos;
using System.Linq;

namespace WorkerService.Module.Cronos
{
    public static class CronParseHelper
    {
        public static CronExpression GetCronExpression(string cronExpression)
        {
            if (string.IsNullOrEmpty(cronExpression))
            {
                return default;
            }

            const int WithSeconds = 5;
            const int WithoutSeconds = 4;

            cronExpression = cronExpression.Trim();
            int spacesCount = cronExpression.Count(x => x == ' ');

            switch (spacesCount)
            {
                case WithSeconds:
                    return CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
                case WithoutSeconds:
                    return CronExpression.Parse(cronExpression);
                default:
                    return default;
            }
        }
    }
}
