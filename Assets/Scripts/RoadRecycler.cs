using UnityEngine;

public class RoadRecycler : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform[] sections;
    [SerializeField] float sectionLength = 100;
    void Update()
    {
        if (!GameManager.Instance.IsRunning) return;
            foreach (Transform section in sections)
                if (section.position.z + sectionLength < player.position.z - 80)
                    section.position += Vector3.forward * sectionLength * sections.Length;

    }
}