using System.Linq;
using UniRx;
using UnityEngine;
using VContainer;
using Script.Feature.Model;
using Script.Feature.View;

namespace Script.Feature.Presenter
{
    public class SwapPresenter
    {
        private SwapModel model;
        private SwapView view;
        private GameObject player;
        private GameObject selectedObject;

        private float swapModeTimeRemaining;
        private bool isInSwapMode;
        private float originalTimeScale;

        public SwapPresenter(SwapModel model, SwapView view, GameObject player)
        {
            this.model = model;
            this.view = view;
            this.player = player;
            this.isInSwapMode = false;
            this.swapModeTimeRemaining = 0f;
        }

        public void EnterSwapMode()
        {
            if (model.CurrentResource < model.ResourceConsumptionOnEnter)
            {
                return;
            }

            model.CurrentResource -= model.ResourceConsumptionOnEnter;
            isInSwapMode = true;
            swapModeTimeRemaining = model.SwapModeDuration;
            originalTimeScale = Time.timeScale;
            Time.timeScale = model.SwapTimeScale;
            view.HighlightSelectableObjects(FindSelectableObjects());
        }

        public void Update()
        {
            if (isInSwapMode)
            {
                model.CurrentResource -= model.ResourceConsumptionPerSecond * Time.deltaTime;
                swapModeTimeRemaining -= Time.deltaTime;

                if (model.CurrentResource <= 0 || swapModeTimeRemaining <= 0)
                {
                    ExitSwapMode();
                }

                HandleSelection();
            }
        }

        public void ExitSwapMode()
        {
            isInSwapMode = false;
            Time.timeScale = originalTimeScale;
            view.UpdateUI();
        }

        public void ExecuteSwap()
        {
            if (selectedObject != null)
            {
                Vector3 playerPosition = player.transform.position;
                player.transform.position = selectedObject.transform.position;
                selectedObject.transform.position = playerPosition;

                model.CurrentResource -= model.ResourceConsumptionOnSwap;
                view.ApplySwapEffects(selectedObject);
                ExitSwapMode();
            }
        }

        private GameObject[] FindSelectableObjects()
        {
            // スワップ可能なオブジェクトの取得
            Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, model.SwapDistance);
            return hitColliders
                .Select(c => c.gameObject)
                .Where(obj => obj.CompareTag("SwappableObjects"))
                .ToArray();
        }

        private void HandleSelection()
        {
            // 選択の処理
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                GameObject closestObject = null;
                float closestDistance = float.MaxValue;

                foreach (GameObject obj in FindSelectableObjects())
                {
                    float distance = Vector3.Distance(player.transform.position, obj.transform.position);
                    if (distance < closestDistance)
                    {
                        closestObject = obj;
                        closestDistance = distance;
                    }
                }

                selectedObject = closestObject;
                view.HighlightSelectableObjects(new GameObject[] { selectedObject });
            }
        }
    }
}