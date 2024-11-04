#region

#endregion

namespace Core.Navigation
{
    public interface IScreen
    {
        /// <summary>
        ///     画面が破棄される際に呼び出されます。
        /// </summary>
        public void OnDestroy();

        /// <summary>
        ///     画面の初期化処理を実装します。
        ///     このメソッドは画面が作成された時に一度だけ呼び出されます。
        /// </summary>
        public void OnCreate();

        /// <summary>
        ///     画面が表示される際に呼び出されます。
        /// </summary>
        public void OnShow();

        /// <summary>
        ///     画面が非表示になる際に呼び出されます。
        /// </summary>
        public void OnHide();
    }
}