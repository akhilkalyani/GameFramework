﻿//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using UnityEngine;


/// <summary>
/// Interface for creating a Cell.
/// Prototype Cell must have a monobeviour inheriting from ICell
/// </summary>
namespace PolyAndCode.UI
{
    public interface ICell
    {
        GameObject GetGameObject();
    }
}
