using GF;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDialog<T> : MonoBehaviour
{
    protected T data;
    public abstract void ShowDialog(T data,Action<bool> status);
    public abstract void CloseDialog();
}