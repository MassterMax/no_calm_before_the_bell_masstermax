using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour
{
    [SerializeField] Text playText;
    // Start is called before the first frame update
    float updated = 0f;
    float updateInterval = 0.2f;
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        SetRandomColor();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void SetRandomColor() {
        updated = Mathf.Max(0, updated - Time.deltaTime);
        if (updated == 0) {
            playText.color = UnityEngine.Random.ColorHSV();
            updated = updateInterval;
        }
    }
}
