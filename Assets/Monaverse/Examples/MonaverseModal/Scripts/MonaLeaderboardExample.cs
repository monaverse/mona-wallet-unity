using Monaverse.Core;
using UnityEngine;

namespace Monaverse.Examples
{
    public class MonaLeaderboardExample : MonoBehaviour
    {
        [SerializeField] private float _score;
        [SerializeField] private string _topic;
        
        /// For security reasons, do not use serialized fields to store secrets
        private string _sdkSecret = "YOUR_SDK_SECRET_HERE";

        private void Start()
        {
            //You can set the SDK secret once on startup
            MonaverseManager.Instance.SDK.SetSecret(_sdkSecret);
        }
        
        [ContextMenu("Post Score")]
        public async void PostScore()
        {
            var result = await MonaverseManager.Instance.SDK
                .PostScore(score: _score,
                    topic: _topic,
                    sdkSecret: _sdkSecret); //Optionally, pass in the SDK secret if not set on startup
            Debug.Log(result);
        }
    }
}