using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF
{
    public class ToastScreenService : IService
    {
        private ToastMessgeUI _toastMessgeUI;
        public void Initialize()
        {
            Console.Log(LogType.Log, "ToastScreenService created");
        }

        public void RegisterListener()
        {
            EventManager.Instance.AddListener<ToastScreenCreated>(InitializeToastMessage);
        }

        private void InitializeToastMessage(ToastScreenCreated e)
        {
            _toastMessgeUI = e.ToastMessgeUI;
            EventManager.Instance.AddListener<ToastEvent>(OnToastEvent);
        }

        private void OnToastEvent(ToastEvent e)
        {
            _toastMessgeUI.ShowToast(e.Message,e.Duration);
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<ToastScreenCreated>(InitializeToastMessage);
            EventManager.Instance.RemoveListener<ToastEvent>(OnToastEvent);
        }
    }
}
