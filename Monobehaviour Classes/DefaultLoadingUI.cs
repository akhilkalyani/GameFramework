using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DefaultLoadingUI : MonoBehaviour
{
    [SerializeField] protected Animator _loadState;
    [SerializeField] protected Animator _idleState;
    [SerializeField] protected Animator _unloadState;
    [SerializeField] private Image _progressbar; 
    public void UpdateProgress(float progress)
    {
        _progressbar.fillAmount = progress;
    }

    public abstract IEnumerator ShowLoadingScreenCoroutine();
    //{
    //    _loadState.gameObject.SetActive(true);
    //    yield return new WaitForSeconds(_loadState.GetCurrentAnimatorStateInfo(0).length);
    //    _idleState.gameObject.SetActive(true);
    //    _loadState.gameObject.SetActive(false);
    //    yield break;
    //}
    public abstract IEnumerator CloseLoadingScreenCoroutine();
    //{
    //    _idleState.gameObject.SetActive(false);
    //    _unloadState.gameObject.SetActive(true);
    //    yield return new WaitForSeconds(_unloadState.GetCurrentAnimatorStateInfo(0).length);
    //    _unloadState.gameObject.SetActive(false);
    //    yield break;
    //}
}
