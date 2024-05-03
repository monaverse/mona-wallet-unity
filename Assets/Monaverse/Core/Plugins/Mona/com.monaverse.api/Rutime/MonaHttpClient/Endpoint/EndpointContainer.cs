using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monaverse.Api.MonaHttpClient.Endpoint
{
    public sealed class EndpointContainer
    {
        private readonly Dictionary<Type, object> _handlers = new();

        public void AddHandler<TRequest, TResponse>(IEndpointHandler<TRequest, TResponse> handler) 
            where TRequest : IEndpointRequest<TResponse>
        {
            _handlers[typeof(TRequest)] = handler;
        }

        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request) where TRequest : IEndpointRequest<TResponse>
        {
            if (_handlers.TryGetValue(typeof(TRequest), out var handler))
            {
                return await ((IEndpointHandler<TRequest, TResponse>)handler).Handle(request);
            }
            else
            {
                throw new InvalidOperationException($"No handler registered for {typeof(TRequest).Name}");
            }
        }
    }
}