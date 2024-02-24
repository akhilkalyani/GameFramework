using System;

namespace GF
{
    public class MonoService : IService
    {
        public void Initialize()
        {
            Console.Log(LogType.Log, "MonoService created");
        }

        public void RegisterListener()
        {
            EventManager.Instance.AddListener<CoroutineEvent>(StartCoroutine);
        }

        private void StartCoroutine(CoroutineEvent e)
        {
            ApplicationManager.Instance.StartCoroutine(e.Enumerator);
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<CoroutineEvent>(StartCoroutine);
        }
    }
}
