using System;
using System.Collections.Generic;
using UnityEngine;

namespace GF
{
    public enum Audio_type { Background=0,ButtonClick=1}
    public class AudioManager : Singleton<AudioManager>
    {
        private Dictionary<Audio_type, AudioSource> audioSourceDictionary;
        protected override void Awake()
        {   
            DontDestroyWhenLoad = true;
            ApplyHighlighter(Utils.GetColorByHashString("#00960C"), Color.white);
            audioSourceDictionary = new Dictionary<Audio_type, AudioSource>()
            {
                {Audio_type.Background, Utils.GetOrAddComponent<AudioSource>(gameObject,true) },
                {Audio_type.ButtonClick, Utils.GetOrAddComponent<AudioSource>(gameObject,true) }
            };
            base.Awake();
        }
        private void OnEnable()
        {
            EventManager.Instance.AddListener<PlayAudioEvent>(PlayAudio);
        }

        private void PlayAudio(PlayAudioEvent e)
        {
            audioSourceDictionary[e.Audio_Type].Play();
        }
        protected override void OnApplicationQuit()
        {
            EventManager.Instance.RemoveListener<PlayAudioEvent>(PlayAudio);
            base.OnApplicationQuit();
        }
    }
}
