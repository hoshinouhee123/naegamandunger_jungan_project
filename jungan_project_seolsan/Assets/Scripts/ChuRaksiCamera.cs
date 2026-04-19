using UnityEngine;

public class ChuRaksiCamera : MonoBehaviour
{
    public GameObject Player;
    public GameObject camera;
    public GameObject MainCamera;
    public GameObject GameOverPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
        {
            camera.SetActive(true);
            
        }
        if(GameOverPanel.activeSelf)
        {
            MainCamera.SetActive(false);
            camera.SetActive(true);
        }
    }
}
