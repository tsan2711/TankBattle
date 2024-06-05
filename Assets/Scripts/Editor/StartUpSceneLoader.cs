using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class StartUpSceneLoader
{
    static StartUpSceneLoader(){
        EditorApplication.playModeStateChanged += LoadStartUpScene;
    }

    private static void LoadStartUpScene(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingEditMode){
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if(state == PlayModeStateChange.EnteredPlayMode){
            if(SceneManager.GetActiveScene().buildIndex != 0){
                SceneManager.LoadScene(0);
            }
        }
    }
}
