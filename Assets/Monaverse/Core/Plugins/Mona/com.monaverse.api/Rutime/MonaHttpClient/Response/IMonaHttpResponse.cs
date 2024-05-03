using System.Collections.Generic;
using System.Text;
using Monaverse.Api.MonaHttpClient.Request;

namespace Monaverse.Api.MonaHttpClient.Response
{
    public interface IMonaHttpResponse
    {
        IMonaHttpRequest HttpRequest { get; }
        string Url { get ; }
        int ResponseCode { get ; }
        byte [] Response { get ;  }
        bool IsSuccess { get ; }
        string Error { get; }
        IEnumerable<KeyValuePair<string,string>> Headers { get ; }
        bool GetHeader(string header, out string value) ;
        string GetResponseString(Encoding encoding = null) ; 
    }

    public interface IMonaHttpResponse<out T> : IMonaHttpRequest
    {
        T Data { get; }
    }
}