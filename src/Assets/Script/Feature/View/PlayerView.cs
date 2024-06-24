using UniRx;
using UnityEngine;

namespace Script.Feature.View
{
    public class PlayerView : MonoBehaviour
    {
        public void UpDatePosition(Vector2 moveDirection)
        {
            transform.Translate(moveDirection*Time.deltaTime);
        }
    }
}