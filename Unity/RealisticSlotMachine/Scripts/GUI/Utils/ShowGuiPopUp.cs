using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    24.10.2019 - first
 */
namespace Mkey
{
	public class ShowGuiPopUp : MonoBehaviour
	{
        #region temp vars
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion temp vars


        public void ShowPopUp(PopUpsController popUpsController)
        {
            MGui.ShowPopUp(popUpsController);
        }
    }
}
