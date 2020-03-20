using System.ServiceModel;
using System.Threading.Tasks;
using TrafficWaveService.Factory.SearchFactory;
using TrafficWaveService.Client;

namespace TrafficWaveService
{
    [ServiceContract]
    public interface ITrafficWave
    {
        [OperationContract]
        Task<SearchQuery> Search(SearchQuery pSearchQuery, SearchType pSearchType);

        [OperationContract]
        Task<Result> AddClient(ClientQuery pClientQuery);


        [OperationContract]
        Task<Result> CheckCriminalStatus(long pQueryId);
    }
}