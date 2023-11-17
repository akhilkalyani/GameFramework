using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace GF
{
    public abstract class ToastMessgeUI : MonoBehaviour
    {
        [SerializeField] protected GameObject _toastContentParent;
        [SerializeField] protected TMP_Text messageTxt;
        public abstract void ShowToast(string message,float duration);
    }
}