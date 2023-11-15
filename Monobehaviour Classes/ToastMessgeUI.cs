using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF
{
    public abstract class ToastMessgeUI : MonoBehaviour
    {
        [SerializeField] protected GameObject _toastContentParent;
        protected virtual void OnEnable()
        {
            RegisterListener();
        }
        protected abstract void RegisterListener();
        protected abstract void RemoveListener();
        protected virtual void OnDisable()
        {
            RemoveListener();
        }
    }
}