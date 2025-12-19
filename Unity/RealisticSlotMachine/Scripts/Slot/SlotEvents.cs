using UnityEngine;
using MkeyFW;

namespace Mkey
{
    public class SlotEvents : MonoBehaviour
    {
        [SerializeField]
        private PopUpsController chestsPrefab;
        public FortuneWheelInstantiator Instantiator;
        public bool autoStartMiniGame = true;

        public static SlotEvents Instance;

        public bool MiniGameStarted { get { return (Instantiator && Instantiator.MiniGame); } }

        #region temp vars
        private Mkey.SlotPlayer MPlayer { get { return Mkey.SlotPlayer.Instance; } }
        // private SlotSoundController MSound { get { return SlotSoundController.Instance; } }
        private SlotGuiController MGUI { get { return SlotGuiController.Instance; } }
        #endregion temp vars

        private void Awake()
        {
            Instance = this; 
        }

        public void Bonus_5()
        {
            Debug.Log("-------------- Bonus 5 win --------------------");
            // MSound.SoundPlayBonusGame(0, null);
            Instantiator.Create(autoStartMiniGame);
            Instantiator.MiniGame.SetBlocked(autoStartMiniGame, autoStartMiniGame);
            Instantiator.SpinResultEvent += (coins, isBigWin) => { MPlayer.AddCoins(coins); };
        }

        public void Scatter_5()
        {
            Debug.Log("-------------- Scatter 5 win --------------------");
            MPlayer.AddLevelProgress(100f);
            // MSound.SoundPlayWinCoins(0, null);
        }

        public void Scatter_4()
        {
            Debug.Log("-------------- Scatter 4 win --------------------");
            MGUI.ShowPopUp(chestsPrefab);
        }

        public void LineEvent_AAA()
        {
            Debug.Log("-------------- AAA win --------------------");
        }
    }
}