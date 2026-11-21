
using UnityEngine;

public class PoliceLights : MonoBehaviour
{
    [SerializeField] GameObject redLight, blueLight;

    void Update()
    {
        bool chase = GameManager.Instance.IsChasing;
        bool red = Mathf.Sin(Time.time * 15) > 0;

        redLight.SetActive(chase && red);
        blueLight.SetActive(chase && !red);
    }
}
