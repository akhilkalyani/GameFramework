using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class LobbyMenuController : MonoBehaviour
    {
        [Space(8, order = 0)]
        [Header("Header menu objects: ", order = 1)]
        [SerializeField]
        private Text dealTimeText;
        [SerializeField]
        private Button dealTimeButton;
        [SerializeField]
        private DealSaleController dealSaleController;

        #region temp vars
        private Button[] buttons;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion temp vars

        #region properties
        #endregion properties

        #region regular
        void Start()
        {
            buttons = GetComponentsInChildren<Button>();
            dealSaleController.WorkingDealTickRestDaysHourMinSecEvent += WorkingDealTickRestDaysHourMinSecHandler;
            dealSaleController.WorkingDealTimePassedEvent += WorkingDealTimePassedHandler;
            dealSaleController.WorkingDealStartEvent += WorkingDealStartHandler;
            dealSaleController.PausedDealStartEvent += PausedDealStartHandler;
        }

        void OnDestroy()
        {
            if (dealSaleController)
            {
                dealSaleController.WorkingDealTickRestDaysHourMinSecEvent -= WorkingDealTickRestDaysHourMinSecHandler;
                dealSaleController.WorkingDealTimePassedEvent -= WorkingDealTimePassedHandler;
                dealSaleController.WorkingDealStartEvent -= WorkingDealStartHandler;
                dealSaleController.PausedDealStartEvent -= PausedDealStartHandler;
            }
        }
        #endregion regular

        /// <summary>
        /// Set all buttons interactble = activity
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            if (buttons == null) return;
            foreach (Button b in buttons)
            {
                if (b) b.interactable = activity;
            }
        }

        public void ResetPlayer()
        {
            SlotPlayer.Instance.SetDefaultData();
        }

        #region event handlers
        private void DealSaleStartHandler()
        {
            if (dealTimeButton) dealTimeButton.enabled = true;
        }

        private void WorkingDealTickRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            if (dealTimeText) dealTimeText.text = String.Format("{0:00}:{1:00}:{2:00}", h, m, s);
        }

        private void WorkingDealTimePassedHandler(double initTime, double realyTime)
        {
            if (dealTimeButton) dealTimeButton.gameObject.SetActive(false);
            if (dealTimeText) dealTimeText.text = String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);
        }

        private void WorkingDealStartHandler()
        {
            if (dealTimeButton) dealTimeButton.gameObject.SetActive(true);
        }

        private void PausedDealStartHandler()
        {
            if (dealTimeButton) dealTimeButton.gameObject.SetActive(false);
            if (dealTimeText) dealTimeText.text = String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);
        }
        #endregion event handlers
    }

    public class TweenIntValue
    {
        private int value;
        private int tweenId;
        private float maxTweenTime;
        private float minTweenTime;
        private Action<int> onUpdate;
        private GameObject g;
        private bool onlyPositive;

        public TweenIntValue(GameObject g, int initValue, float minTweenTime, float maxTweenTime, bool onlyPositive, Action<int> onUpdate)
        {
            value = initValue;
            this.maxTweenTime = Mathf.Max(0,  maxTweenTime);
            this.minTweenTime = Mathf.Clamp(minTweenTime, 0, maxTweenTime);
            this.g = g;
            this.onlyPositive = onlyPositive;
            this.onUpdate = onUpdate;
        }

        public void Tween(int newValue, int valuePerSecond)
        {
            SimpleTween.Cancel(tweenId, false);
            float add = newValue - value;

            if ((add > 0 && onlyPositive) || !onlyPositive)
            {
                valuePerSecond = Mathf.Max(1, valuePerSecond);
                float tT = Mathf.Abs((float)add / (float)valuePerSecond);
                tT = Mathf.Clamp(tT, minTweenTime, maxTweenTime);
                tweenId = SimpleTween.Value(g, value, newValue, tT).SetOnUpdate((float val) =>
                {
                    if (this != null)
                    {
                        value = (int)val;
                        onUpdate?.Invoke(value);
                    }
                }).ID;
            }
            else if (onlyPositive)
            {
                if (this != null)
                {
                    value = newValue;
                    onUpdate?.Invoke(value);
                }
            }
        }
    }
}