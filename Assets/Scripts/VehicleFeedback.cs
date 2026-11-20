
using UnityEngine;

public class VehicleFeedback : MonoBehaviour
{
    [SerializeField] CarController player;
    [SerializeField] Transform body;
    [SerializeField] Transform[] wheels;
    [SerializeField] Renderer[] brakeLights;

    Material[] lightMaterials;

    void Start()
    {
        lightMaterials = new Material[brakeLights.Length];

        for (int i = 0; i < brakeLights.Length; i++)
            lightMaterials[i] = brakeLights[i].material;
    }

    void Update()
    {
        if (!GameManager.Instance.IsRunning) return;

        body.localRotation = Quaternion.Euler(
            player.Braking ? 2 : 0,
            0,
            -player.Steering * 3
        );

        foreach (Transform wheel in wheels)
            wheel.Rotate(
                0,
                player.SpeedKph / 3.6f * Time.deltaTime * 140,
                0,
                Space.Self
            );

        foreach (Material material in lightMaterials)
            material.color = player.Braking
                ? Color.red
                : new Color(0.3f, 0, 0);
    }

    void OnDestroy()
    {
        if (lightMaterials == null) return;

        foreach (Material material in lightMaterials)
            Destroy(material);
    }
}
