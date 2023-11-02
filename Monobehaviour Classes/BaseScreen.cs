using System;
using UnityEngine;
namespace GF
{
    public class BaseScreen<E> : MonoBehaviour where E: Enum
    {
        public E Screen;
        private CanvasGroup _canvasGroup;
        protected virtual void OnEnable()
        {
            //EventManager.Instance.RemoveListener<UnLoadingCompletedEvent>(DoInitialLoading);
        }
        public void OpenScreen()
        {
            gameObject.SetActive(true);
        }
        public void CloseScreen()
        {
            gameObject.SetActive(false);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackKeyPressed();
            }
        }
        protected virtual void OnBackKeyPressed()
        {

        }
        protected virtual void OnDisable()
        {
            //EventManager.Instance.RemoveListener<UnLoadingCompletedEvent>(DoInitialLoading);
        }
    }
}