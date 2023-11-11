
using System;

namespace GF
{
    public static class UnitySceneManager
    {
        public static void ChangeScene(int scene)
        {
            Utils.CallEventAsync(new LoadingEvent(()=>Utils.CallEventAsync(new SceneLoadingEvent(scene))));
        }
        public static void ShowLoadingScreen(Action complete)
        {
            Utils.CallEventAsync(new LoadingEvent(complete));
        }
        public static void HideLoadingScreen(Action complete)
        {
            Utils.CallEventAsync(new UnloadingEvent(complete));
        }
    }
}
