using System;
using Monaverse.Api.Modules.Leaderboard.Enums;
using Monaverse.Core;
using UnityEngine;

namespace Monaverse.Examples
{
    public class MonaLeaderboardExample : MonoBehaviour
    {
        [Header("Get Top Scores")]
        [SerializeField] private int _limit = 20;

        [SerializeField] private int _offset = 0;
        [SerializeField] private bool _featured = false;
        [SerializeField] private string _topScoresTopic = null;
        [SerializeField] private LeaderboardSortingOrder _sortingOrder = LeaderboardSortingOrder.Highest;
        [SerializeField] private LeaderboardPeriod _period = LeaderboardPeriod.AllTime;
        [SerializeField] private bool _includeAllUserScores = false;

        [Header("Post Score")]
        [SerializeField] private float _score;

        [SerializeField] private string _postScoreTopic = null; //Leave null to post with no topic;

        /// For security reasons, do not use serialized fields to store secrets
        private string _sdkSecret = "YOUR_SDK_SECRET_HERE";

        private void Start()
        {
            //You can set the SDK secret once on startup
            MonaverseManager.Instance.SDK.SetSecret(_sdkSecret);
        }

        [ContextMenu("Get Top Scores")]
        public async void GetTopScores()
        {
            var result = await MonaverseManager.Instance.SDK
                .GetTopScores(
                    limit: _limit,
                    offset: _offset,
                    featured: _featured,
                    topic: _topScoresTopic,
                    sortingOrder: _sortingOrder,
                    period: _period,
                    includeAllUserScores: _includeAllUserScores);
            Debug.Log(result);
        }

        [ContextMenu("Post Score")]
        public async void PostScore()
        {
            var result = await MonaverseManager.Instance.SDK
                .PostScore(score: _score,
                    topic: _postScoreTopic,
                    sdkSecret: _sdkSecret); //Optionally, pass in the SDK secret if not set on startup
            Debug.Log(result);
        }
    }
}