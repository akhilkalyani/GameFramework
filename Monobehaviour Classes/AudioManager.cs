using System;
using System.Collections.Generic;
using UnityEngine;

namespace GF
{
    public enum AudioType { Background=0,ButtonClick=1}
    public class AudioManager : Singleton<AudioManager>
    {
        private Dictionary<AudioType, AudioSource> audioSourceDictionary;
        protected override void Awake()
        {   
            DontDestroyWhenLoad = true;
            ApplyHighlighter(Utils.GetColorByHashString("#00960C"), Color.white);
            audioSourceDictionary = new Dictionary<AudioType, AudioSource>()
            {
                {AudioType.Background, Utils.GetOrAddComponent<AudioSource>(gameObject,true) },
                {AudioType.ButtonClick, Utils.GetOrAddComponent<AudioSource>(gameObject,true) }
            };
            base.Awake();
        }
        private void OnEnable()
        {
            EventManager.Instance.AddListener<PlayAudioEvent>(PlayAudio);
        }

        private void PlayAudio(PlayAudioEvent e)
        {
            audioSourceDictionary[e.AudioType].Play();
        }
        protected override void OnApplicationQuit()
        {
            EventManager.Instance.RemoveListener<PlayAudioEvent>(PlayAudio);
            base.OnApplicationQuit();
        }
    }
}
