using System.Collections;
using System.Threading.Tasks;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;
using UnityEngine.Networking;

namespace Monaverse.Api.MonaHttpClient.Extensions
{
    internal static class UnityWebRequestExtensions
    {
        public static async Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest unityWebRequest)
        {
            var isComplete = false;

            unityWebRequest
                .SendWebRequestCoroutine()
                .RunCoroutine(() => isComplete = true);

            while (!isComplete && !unityWebRequest.isDone)
                await Task.Yield();

            return unityWebRequest;
        }

        public static IEnumerator SendWebRequestCoroutine(this UnityWebRequest unityWebRequest)
        {
            yield return unityWebRequest.SendWebRequest();
        }

        public static IMonaHttpResponse ToMonaHttpResponse(this UnityWebRequest unityWebRequest, IMonaHttpRequest request = null)
            => new MonaHttpResponse(
                url: UnityWebRequest.UnEscapeURL(unityWebRequest.url),
                responseCode: (int)unityWebRequest.responseCode,
                response: unityWebRequest.downloadHandler.data,
                headers: unityWebRequest.GetResponseHeaders(),
                httpRequest: request,
                error: unityWebRequest.error);
    }
}