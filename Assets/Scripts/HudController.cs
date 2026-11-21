using UnityEngine;
using TMPro;
public class HudController : MonoBehaviour
{
[SerializeField] CarController player;
[SerializeField] TMP_Text speedText, scoreText, statusText, noticeText;
[SerializeField] GameObject pausePanel;
    void Update()
    {
        GameManager game = GameManager.Instance;
        ScoreManager score = ScoreManager.Instance;
        speedText.text = $"{player.SpeedKph:0} km/h";
        scoreText.text = $"Score {score.Score} / {score.Multiplier:0.0}x\n" +
        $"Distance {score.Distance:0} m / Section {game.StageIndex + 1}";
        statusText.text = $"Minimum {game.MinimumSpeed:0} km/h\n" +
        (player.SpeedKph < game.MinimumSpeed ? "BELOW MINIMUM: 1x\n" : "") +
        game.PoliceStatus;
        noticeText.text = game.Notice;
        pausePanel.SetActive(game.IsPaused);
    }
}