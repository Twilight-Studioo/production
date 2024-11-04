using UnityEngine;

namespace Core.Navigation
{
    public abstract class AScreen: MonoBehaviour
    {
        /// <summary>
        /// 画面の初期化処理を実装します。
        /// このメソッドは画面が作成された時に一度だけ呼び出されます。
        /// </summary>
        public abstract void OnCreate();

        /// <summary>
        /// 画面が表示される際に呼び出されます。
        /// </summary>
        public virtual void OnShow()
        {
        }

        /// <summary>
        /// 画面が非表示になる際に呼び出されます。
        /// </summary>
        public virtual void OnHide()
        {
        }

        /// <summary>
        /// 画面が破棄される際に呼び出されます。
        /// </summary>
        public virtual void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}