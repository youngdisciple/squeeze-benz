using UnityEngine;
public class DifficultyController : MonoBehaviour
{
[SerializeField] Transform player;
[SerializeField] float secondSection = 600;
[SerializeField] float thirdSection = 1400;
float startZ;
void Start() { startZ = player.position.z; }
void Update()
{
if (!GameManager.Instance.IsRunning) return;
float distance = Mathf.Max(0, player.position.z - startZ);
int stage = distance >= thirdSection ? 2 :
distance >= secondSection ? 1 : 0;
GameManager.Instance.SetStage(stage, 50 + stage * 15);
}
}