#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace Core.Navigation
{
    public partial class ScreenController<T> where T : Enum
    {
        private readonly Dictionary<T, Destination<T>> screenCache = new();

        /// <summary>
        ///     指定されたルートに対応する画面を検索します。
        /// </summary>
        /// <param name="route">検索対象の画面ルート</param>
        /// <returns>見つかった画面のDestinationオブジェクト</returns>
        /// <exception cref="ScreenNotFoundException">指定されたルートの画面が見つからない場合にスローされます</exception>
        private partial Destination<T> FindScreen(T route)
        {
            if (screens == null)
            {
                throw new InvalidOperationException("画面コレクションが初期化されていません。");
            }

            if (screenCache.TryGetValue(route, out var cachedScreen))
            {
                return cachedScreen;
            }

            foreach (var screen in screens)
            {
                if (!screen.Route.Equals(route))
                {
                    continue;
                }

                screenCache[route] = screen;
                return screen;
            }

            throw new ScreenNotFoundException($"ルート {route} に対応する画面が見つかりません。");
        }


        [Serializable]
        public class ScreenNotFoundException : Exception
        {
            public ScreenNotFoundException(string message) : base(message)
            {
            }

            protected ScreenNotFoundException(
                SerializationInfo info,
                StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}