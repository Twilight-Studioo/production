using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

namespace Core.Utilities
{
    public class CloudService
    {
        private static CloudService instance;
        public static CloudService Instance { get; } = instance ??= new CloudService();
        
        private bool isAuthenticating;
        public async Task Initialize()
        {
            try
            {
                // Unity Servicesの初期化
                await UnityServices.InitializeAsync();
                Debug.Log("Unity Services Initialized");

                // 匿名サインイン
                await SignInAnonymously();
                Debug.Log("Signed in anonymously");

                if (!isAuthenticating)
                {
                    return;
                }
                // データの保存を試行
               // await SaveData();
            }
            catch (System.Exception e)
            {
                isAuthenticating = false;
                Debug.LogError($"CloudService initialization failed: {e}");
            }
        }
        
        private async Task SignInAnonymously()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                isAuthenticating = true;
                Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
            };
            AuthenticationService.Instance.SignInFailed += s =>
            {
                isAuthenticating = false;
                // Take some action here...
                Debug.Log(s);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private async Task SaveData()
        {
            // 保存するデータを定義
            var playerData = new Dictionary<string, object>
            {
                {"firstKeyName", "a text value"},
                {"secondKeyName", 123},
                {"thirdKeyName", 0.456f},
                {"fourthKeyName", new Dictionary<string, object>
                {
                    {"nestedKeyName", "nested text value"},
                }},
            };

            try
            {
                // データをクラウドに保存
                await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
                await SubmitScore("twilight-studio-score", 100);
                await FetchVersionScores("twilight-studio-score", "1");
                Debug.Log($"Saved data: {string.Join(", ", playerData)}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save data: {e}");
            }
        }
        
        public async Task SubmitScore(string leaderboardId, int score)
        {
            try
            {
                // 指定したリーダーボードにスコアを送信
                await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
                Debug.Log($"Score {score} submitted to leaderboard: {leaderboardId}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to submit score: {e.Message}");
            }
        }
        
        public async Task FetchVersionScores(string leaderboardId, string versionNumber, int limit = 10)
        {
            try
            {
                var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);

                foreach (var entry in scores.Results)
                {
                    Debug.Log($"Player ID: {entry.PlayerId}, Score: {entry.Score}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to fetch version scores: {e.Message}");
            }
        }
    }
}