using UnityEngine;
using TMPro;
public class EndScreen : MonoBehaviour
{
    [SerializeField] TMP_Text resultText;
    void Start()
    {
        GameManager game = GameManager.Instance;
        resultText.text = $"Score: {game.FinalScore}\n" +
        $"Distance: {game.FinalDistance:0} m\nTop speed: {game.FinalTopSpeed:0} km/h\n" +
        $"Close calls: {game.FinalNearMisses} / Escapes: {game.FinalEscapes}\n" +
        $"Earned: {game.MoneyEarned} credits\nWallet: {game.Wallet}\nBest: {game.BestScore}";
    }
}