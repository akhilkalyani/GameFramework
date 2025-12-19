using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    [ExecuteInEditMode]
    public class ProgressSlider : MonoBehaviour
    {
        [SerializeField]
        private float posX;
        [SerializeField]
        private float offset = 5;
        [SerializeField]
        private RectTransform pointer;

        #region temp vars
        private Image progressImage;
        private RectTransform rt;
        #endregion temp vars

        #region regular
        private void Start()
        {
            progressImage = GetComponent<Image>();
            rt = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (progressImage && rt)
            {
                float dx = progressImage.fillAmount;
                float p = (dx < 0.981) ? posX + offset : posX;

                rt.anchoredPosition = new Vector2(p + (dx - 1) * rt.rect.width, rt.anchoredPosition.y);
                if (pointer) pointer.gameObject.SetActive(dx > 0.05f);
            }
        }
        #endregion regular

        public void SetFillAmount(float fillAmount)
        {
            if (progressImage && rt)
            {
                progressImage.fillAmount = fillAmount;
            }
        }
    }
}