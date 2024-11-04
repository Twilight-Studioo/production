#region

using System;
using System.Collections.Generic;

#endregion

namespace Core.Navigation
{
    public partial class ScreenController<T> where T : Enum
    {
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

            foreach (var screen in screens)
            {
                if (screen.Route.Equals(route))
                {
                    return screen;
                }
            }

            throw new ScreenNotFoundException($"ルート {route} に対応する画面が見つかりません。");
        }

        public class ScreenNotFoundException : Exception
        {
            public ScreenNotFoundException(string message) : base(message)
            {
            }
        }
    }
}