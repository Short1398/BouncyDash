using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionButton : MonoBehaviour
{
    Button selfButton;

    [SerializeField]
    string scene;

    // Start is called before the first frame update
    void Start()
    {
        selfButton = gameObject.GetComponent<Button>();
        selfButton.onClick.AddListener(LoadNewScene);
    }

    void LoadNewScene()
    {
        SceneManager.LoadScene(scene);
    }
}
