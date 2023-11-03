using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DefaultLoadingUI : MonoBehaviour
{
    [SerializeField] protected GameObject safeArea;
    [SerializeField] protected Animator _loadState;
    [SerializeField] protected Animator _idleState;
    [SerializeField] protected Animator _unloadState;
    [SerializeField] private Image _progressbar;


    public void UpdateProgress(float progress)
    {
        _progressbar.fillAmount = progress;
    }

    public abstract IEnumerator ShowLoadingScreenCoroutine();

    public abstract IEnumerator CloseLoadingScreenCoroutine();
}
