using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DefaultLoadingUI : MonoBehaviour
{
    [SerializeField] protected GameObject loadingParent;
    [SerializeField] protected Animator _loadState;
    [SerializeField] protected GameObject _idleState;
    [SerializeField] protected Animator _unloadState;
    [SerializeField] protected Image _progressbar;
    protected Coroutine loadRoutine = null;
    protected Coroutine unloadRoutine = null;
    protected float _minimumWaitDuration = 1f;
    public void UpdateProgress(float progress)
    {
        if (_progressbar == null) return;
        _progressbar.fillAmount = progress;
    }
    public abstract void Load(Action onComplete);
    public abstract void Unload(Action onComplete);
}
