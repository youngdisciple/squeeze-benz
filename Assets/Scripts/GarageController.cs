using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GarageController : MonoBehaviour
{
    [SerializeField] TMP_Text walletText, messageText;
    [SerializeField] TMP_Text[] carTexts;
    [SerializeField] Button[] buyButtons, selectButtons, upgradeButtons;

    void Start() { Refresh(); }

    void Update() { messageText.text = GameManager.Instance.Notice; }

    public void Buy(int i) { GameManager.Instance.BuyCar(i); Refresh(); }
    public void Select(int i) { GameManager.Instance.SelectCar(i); Refresh(); }
    public void Upgrade(int i) { GameManager.Instance.UpgradeCar(i); Refresh(); }

    void Refresh()
    {
        GameManager game = GameManager.Instance;
        walletText.text = "Credits: " + game.Wallet;
        for (int i = 0; i < game.CarCount; i++)
        {
            CarProfile car = game.GetCar(i);
            carTexts[i].text = $"{car.name} / {car.price} credits\n" +
                $"Speed {car.speed}/5 Acceleration {car.acceleration}/5\n" +
                $"Handling {car.handling}/5 Braking {car.braking}/5 Traction {car.traction}/5\n" +
                (game.SelectedCar == i ? "SELECTED" : game.Owns(i) ? "OWNED" : "LOCKED") +
                (game.HasUpgrade(i) ? " / Acceleration +20%" : "");
            buyButtons[i].interactable = game.CanBuy(i);
            selectButtons[i].interactable = game.Owns(i) && game.SelectedCar != i;
            upgradeButtons[i].interactable = game.CanUpgrade(i);
        }
    }
}
