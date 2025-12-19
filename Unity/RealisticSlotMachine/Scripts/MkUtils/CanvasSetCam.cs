using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
23.10.2019 - first
*/
namespace Mkey
{
	public class CanvasSetCam : MonoBehaviour
	{
        [SerializeField]
        private Camera cam;
		
        #region temp vars
        private SlotGuiController SGui { get { return SlotGuiController.Instance; } }
        private bool result = false;
        #endregion temp vars
		
		#region regular
		private void Start()
		{
            SetCam();
		}
		#endregion regular

        private void SetCam()
        {
            if (SGui && cam)
            {
                Canvas c =  SGui.GetComponent<Canvas>();
                if (c) c.worldCamera = cam;
                result = true;
                Debug.Log("Camera set complete");
            }
        }

        private IEnumerator SetCamC()
        {
            while (!result)
            {
                SetCam();
                yield return new WaitForEndOfFrame();
            }
        }
	}
}
