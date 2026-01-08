using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public bool hasStart = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        if (hasStart)
            return;

        GameManager.Instance.stateMachine.Trigger<StateChangeTrigger>();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
