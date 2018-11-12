
using CFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

    private bool isLoad = false;
    

    void Start () {

        Debug.Log("-------------App Start-------------");

        Screen.SetResolution(AppConst.DesignWidth, AppConst.DesignHeight,false);

        StartUpManager.Instance.Init();
        StartUpManager.Instance.StartUp();
        DontDestroyOnLoad(this);
        
    }
    
    void Update()
    {
    }
}
