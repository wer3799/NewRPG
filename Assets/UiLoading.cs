using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiLoading : MonoBehaviour
{
    [SerializeField]
    private Image tipIcon;
    [SerializeField]
    private TextMeshProUGUI description;

    private IEnumerator Start()
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(2);
        asyncOper.allowSceneActivation = false;

        while (!asyncOper.isDone)
        {
            yield return null;

            if (asyncOper.progress >= 0.9f)
            {
                asyncOper.allowSceneActivation = true;
                break;
            }
        }
    }

}
