using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    enum SoundMusic {Sound, Music }
	public class GUIMusicSoundButtonBehavior : MonoBehaviour
	{
        [SerializeField]
        private Image iconOnOff;
        [SerializeField]
        private Text textOnOff;
        [SerializeField]
        private Sprite buttonSpriteOn;
        [SerializeField]
        private Sprite buttonSpriteOff;
        [SerializeField]
        private Sprite iconSpriteOn;
        [SerializeField]
        private Sprite iconSpriteOff;
        [SerializeField]
        private string textOn;
        [SerializeField]
        private string textOff;

        [SerializeField]
        private SoundMusic soundOrMusic;

        #region temp vars
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        // private SlotSoundController MSound { get { return SlotSoundController.Instance; } }
        #endregion temp vars

        #region regular
        private void Start()
		{
            Button b = GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(Button_Click);
            Refresh();
		}
        #endregion regular

        public void Button_Click()
        {
            // if(soundOrMusic == SoundMusic.Music) MSound.SetMusic(!MSound.MusicOn);
            // else if(soundOrMusic == SoundMusic.Sound) MSound.SetSound(!MSound.SoundOn);
            Refresh();
        }

        private void Refresh()
        {
            Image image = GetComponent<Image>();
            bool on = false;
            // if (soundOrMusic == SoundMusic.Music)
            //     on = MSound.MusicOn;
            // else if (soundOrMusic == SoundMusic.Sound)
            //     on = MSound.SoundOn;
            
            if (image) image.sprite = (on) ? buttonSpriteOn : buttonSpriteOff;
            if (iconOnOff) iconOnOff.sprite = (on) ? iconSpriteOn : iconSpriteOff;
            if (textOnOff) textOnOff.text = (on) ? textOn : textOff;
        }
    }
}
