using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : GenericSingleton<SceneController>
{
    public Image loadImage;

    public bool asyncIsDone;

    public void GoToScene(string sceneName)
    {
        StartCoroutine(StartLoad(sceneName));
    }

    public void GameExit()
    {
        Application.Quit();
    }

    IEnumerator StartLoad(string sceneName)
    {
        SceneManager.LoadSceneAsync("Loading");

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = true;

        while (loadImage == null)
        {
            if (loadImage != null)
            {
                break;
            }
        
            yield return new WaitForFixedUpdate();
        }

        while (!asyncOperation.isDone)
        {
            loadImage.fillAmount = asyncOperation.progress;

            if (asyncOperation.progress >= 0.8f)
            {
                asyncIsDone = true;

                asyncOperation.allowSceneActivation = true;
                break;
            }
            
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }
}
