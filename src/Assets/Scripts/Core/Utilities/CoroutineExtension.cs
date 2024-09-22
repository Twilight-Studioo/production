#region

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Core.Utilities
{
    public static class CoroutineExtension
    {
        private const int MaxCacheSize = 20;

        // コルーチンキャッシュを保持する静的辞書
        private static readonly Dictionary<string, Coroutine> CoroutineCache = new();

        /// <summary>
        ///     ユニークなキーでコルーチンを開始します。同じキーのコルーチンが既に実行中の場合は停止してから新しく開始します。
        ///     キーが指定されない場合、自動的にキーを生成します。
        /// </summary>
        /// <param name="monoBehaviour">拡張メソッドを呼び出すMonoBehaviourのインスタンス</param>
        /// <param name="routine">実行するコルーチン</param>
        /// <param name="key">コルーチンを識別するためのキー（オプション）</param>
        public static void UniqueStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator routine,
            string key = null)
        {
            if (routine == null)
            {
                Debug.LogError("Attempted to start a null coroutine.");
                return;
            }

            if (CoroutineCache.Count >= MaxCacheSize && key != null && !CoroutineCache.ContainsKey(key))
            {
                Debug.LogError(
                    $"Coroutine cache has reached its maximum size of {MaxCacheSize}. Cannot start coroutine with key: {key}");
                return;
            }

            // キーが指定されていない場合、自動生成
            if (string.IsNullOrEmpty(key))
            {
                key = GenerateKeyFromRoutine(routine);
            }

            // 既に同じキーのコルーチンが存在する場合は停止
            monoBehaviour.DeleteCoroutine(key);

            // コルーチンをラップしてキャッシュに追加
            var newCoroutine = monoBehaviour.StartCoroutine(RunAndCacheCoroutine(monoBehaviour, routine, key));
            CoroutineCache[key] = newCoroutine;
        }

        private static IEnumerator RunAndCacheCoroutine(MonoBehaviour monoBehaviour, IEnumerator routine, string key)
        {
            yield return monoBehaviour.StartCoroutine(routine);
            // コルーチン終了後にキャッシュから削除
            CoroutineCache.Remove(key);
        }

        /// <summary>
        ///     コルーチンから一意のキーを生成します。
        /// </summary>
        /// <param name="routine">コルーチンのインスタンス</param>
        /// <returns>生成されたキー</returns>
        private static string GenerateKeyFromRoutine(IEnumerator routine) =>
            // ルーチンのハッシュコードをキーとして使用
            routine.GetHashCode().ToString();

        /// <summary>
        ///     指定されたキーのコルーチンを停止します。
        /// </summary>
        /// <param name="monoBehaviour">拡張メソッドを呼び出すMonoBehaviourのインスタンス</param>
        /// <param name="key">停止するコルーチンのキー</param>
        public static void UniqueStopCoroutine(this MonoBehaviour monoBehaviour, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Cannot stop a coroutine with a null or empty key.");
                return;
            }

            if (!DeleteCoroutine(monoBehaviour, key))
            {
                Debug.LogWarning($"No coroutine found with key: {key}");
            }
        }

        /// <summary>
        ///     すべてのユニークコルーチンを停止します。
        /// </summary>
        /// <param name="monoBehaviour">拡張メソッドを呼び出すMonoBehaviourのインスタンス</param>
        public static void UniqueStopAllCoroutines(this MonoBehaviour monoBehaviour)
        {
            foreach (var entry in new Dictionary<string, Coroutine>(CoroutineCache))
            {
                DeleteCoroutine(monoBehaviour, entry.Key);
            }
        }

        private static bool DeleteCoroutine(this MonoBehaviour monoBehaviour, string key)
        {
            if (!CoroutineCache.TryGetValue(key, out var existingCoroutine))
            {
                return false;
            }

            if (existingCoroutine == null)
            {
                return false;
            }

            monoBehaviour.StopCoroutine(existingCoroutine);
            CoroutineCache.Remove(key);
            return true;
        }
    }
}