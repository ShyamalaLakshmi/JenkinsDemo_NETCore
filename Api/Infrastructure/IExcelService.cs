using System.Threading.Tasks;

namespace Api.Infrastructure
{
    public interface IExcelService
    {
        Task GetEventSummaryInformation();
    }
}
