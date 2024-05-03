using System;
using UnityEngine;
using System.Collections;

namespace Monaverse.Api.MonaHttpClient.Extensions
{
    internal static class CoroutineExtensions
    {
        private static CoroutineRunnerBehaviour _coroutineRunnerBehaviour;

        public static Coroutine RunCoroutine(this IEnumerator coroutine, Action onComplete)
        {
            if (_coroutineRunnerBehaviour != null)
                return _coroutineRunnerBehaviour.Run(coroutine, onComplete);

            var coroutineRunnerGameObject = new GameObject("~CoroutineRunner+" + Guid.NewGuid());
            var coroutineRunnerBehaviour = coroutineRunnerGameObject.GetComponent<CoroutineRunnerBehaviour>();

            if (coroutineRunnerBehaviour == null)
                coroutineRunnerBehaviour = coroutineRunnerGameObject.AddComponent<CoroutineRunnerBehaviour>();

            _coroutineRunnerBehaviour = coroutineRunnerBehaviour;

            GameObject.DontDestroyOnLoad(_coroutineRunnerBehaviour);

            _coroutineRunnerBehaviour.gameObject.hideFlags = HideFlags.HideInHierarchy;

            return _coroutineRunnerBehaviour.Run(coroutine, onComplete);
        }

        private sealed class CoroutineRunnerBehaviour : MonoBehaviour
        {
            private IEnumerator Coroutine(IEnumerator coroutine, Action onComplete)
            {
                yield return StartCoroutine(coroutine);
                onComplete?.Invoke();
            }

            public Coroutine Run(IEnumerator coroutine, Action onComplete)
            {
                return StartCoroutine(Coroutine(coroutine, onComplete));
            }
        }
    }
}