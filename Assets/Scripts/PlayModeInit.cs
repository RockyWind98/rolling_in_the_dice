using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneMonitor
{
    // 静态构造函数，在类首次使用时自动调用
    static SceneMonitor()
    {
        // 订阅场景事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        Debug.Log("场景监视器已初始化");
    }

    // 场景加载完成时触发
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景加载完成: {scene.name}, 模式: {mode}");
        Scene currentScene = SceneManager.GetActiveScene();

        switch (scene.name)
        {
            case "MainMenu":
                if(GlobalManager.Instance != null)
                {
                    GlobalManager.Instance.currentSessionData = null;
                }
                break;
            case "DiceDesk":
                Debug.Log("====================加载======================");
                if(GlobalManager.Instance != null)
                {
                    GlobalManager.Instance.InitialGameSession();
                }
                break;
            default:
                break;
        }
        // 在这里添加你的自定义逻辑
        HandleSceneChange(scene.name, "loaded", mode);
    }

    // 场景卸载完成时触发
    private static void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"场景卸载完成: {scene.name}");
        HandleSceneChange(scene.name, "unloaded", LoadSceneMode.Single);
    }

    // 活动场景改变时触发
    private static void OnActiveSceneChanged(Scene previousScene, Scene newScene)
    {
        Debug.Log($"活动场景改变: {previousScene.name} -> {newScene.name}");
        // 可以记录当前活动场景信息
    }

    // 统一的场景变化处理逻辑
    private static void HandleSceneChange(string sceneName, string changeType, LoadSceneMode mode)
    {
        switch (changeType)
        {
            case "loaded":
                break;
            case "unloaded":
                // 处理场景卸载逻辑
                break;
        }

        // 可以记录场景切换历史
        RecordSceneHistory(sceneName, changeType);
    }

    private static void RecordSceneHistory(string sceneName, string changeType)
    {
        // 记录场景切换历史
    }

    public static void Initialize()
    {
        // 方法体可以为空，目的只是触发静态构造函数
        Debug.Log("场景监视器已激活");
    }
}



public class PlayModeInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnGameStart()
    {
        // 你希望默认启动的场景名称
        string startSceneName = "MainMenu";
        SceneMonitor.Initialize();

        // 获取当前活动场景
        Scene currentScene = SceneManager.GetActiveScene();

        // 如果当前场景不是目标场景，则加载目标场景
        if (!currentScene.name.Equals(startSceneName))
        {
            SceneManager.LoadScene(startSceneName);
        }
        // 如果当前已经是目标场景，则什么都不做，正常启动
    }
}