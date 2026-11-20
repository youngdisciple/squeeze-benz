
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameAudio : MonoBehaviour
{
    [SerializeField] AudioSource engine, siren, effects;
    [SerializeField] AudioClip horn, impact, reward;

    GameManager game;
    CarController player;
    float nextHorn;

    void Start()
    {
        game = GameManager.Instance;

        game.Crashed += PlayImpact;
        game.BonusAwarded += PlayReward;

        SceneManager.sceneLoaded += SceneChanged;

        FindPlayer();
    }

    void SceneChanged(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        player = FindAnyObjectByType<CarController>();
    }

    void Update()
    {
        bool active = game.IsRunning && player != null;

        SetLoop(engine, active);
        SetLoop(siren, active && game.IsChasing);

        if (!active) return;

        engine.pitch = 0.7f + player.SpeedKph / 120;

        if (Input.GetKeyDown(KeyCode.H) && Time.time >= nextHorn)
        {
            if (horn != null)
                effects.PlayOneShot(horn);

            nextHorn = Time.time + 0.4f;
        }
    }

    void SetLoop(AudioSource source, bool active)
    {
        if (active && !source.isPlaying && source.clip != null)
            source.Play();

        if (!active && source.isPlaying)
            source.Stop();
    }

    void PlayImpact()
    {
        if (impact != null)
            effects.PlayOneShot(impact);
    }

    void PlayReward()
    {
        if (reward != null)
            effects.PlayOneShot(reward);
    }

    void OnDestroy()
    {
        if (game == null) return;

        game.Crashed -= PlayImpact;
        game.BonusAwarded -= PlayReward;

        SceneManager.sceneLoaded -= SceneChanged;
    }
}
