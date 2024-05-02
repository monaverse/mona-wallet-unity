using System.Threading.Tasks;

namespace Monaverse.MonaHttpClient.Endpoint
{
    public interface IEndpointHandler<in TRequest, TResponse>
        where TRequest : IEndpointRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }
}