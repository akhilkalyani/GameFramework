namespace GF
{
    public class ToastService : IService
    {
        public bool IsUpdateRequired => false;
        private ITaost taostInstance;
        public void Initialize()
        {

        }

        public void RegisterListener()
        {
            EventManager.Instance.AddListener<CreateToastEvent>(InitializeToast);
            EventManager.Instance.AddListener<ToastShowEvent>(ShowToast);
        }

        private void ShowToast(ToastShowEvent e)
        {
            taostInstance.ShowToast(e.Message, e.Duration);
        }

        private void InitializeToast(CreateToastEvent e)
        {
            this.taostInstance = e.TaostInstance;
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<CreateToastEvent>(InitializeToast);
            EventManager.Instance.RemoveListener<ToastShowEvent>(ShowToast);
        }

        public void Update()
        {

        }
    }
}