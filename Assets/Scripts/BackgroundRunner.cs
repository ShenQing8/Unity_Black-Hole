using UnityEngine;

public class BackgroundRunner : MonoBehaviour
{
    private void Awake()
    {
        Application.runInBackground = true;

        Application.targetFrameRate = 120;
    }
}
