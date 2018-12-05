using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance; //singleton

    [Header("Fading")]
    [SerializeField] private Animator _fadeAnimator;
    [SerializeField] private Image _fadeImage;
    [Header("Game")]
    [SerializeField] private Animator _bgAnimator;
    [SerializeField] private PlayerController _player;
    [SerializeField] private Text _healthUI;
    public string HealthUI { get { return _healthUI.text; } set { _healthUI.text = value; } }
    [SerializeField] private Text _bombsUI;
    public string BombsUI { get { return _bombsUI.text; } set { _bombsUI.text = value; } }
    [SerializeField] private Text _scoreUI;
    public string ScoreUI { get { return _scoreUI.text; } set { _scoreUI.text = value; } }

    protected void Awake()
    {
        instance = this;

        _player.OnTakeDamage += UpdateHealth;
        _player.OnUseBomb += UpdateBombs;
        _player.OnPickupBombs += UpdateBombs;
        _player.OnPlayerDeath += GameOver;
    }

    protected void Start()
    {
        EnemyManager.instance.OnEnemyDeath += UpdateScore;

        UpdateHealth();
        UpdateBombs();
        UpdateScore(0);
    }

    public void GameOver()
    {
            HighScoreManager3.PlayerBuffer = new PlayerStat(){Name = "", Score = _player.Score};
            ChangeMenu("GameOver");
    }

    public void UpdateHealth()
    {
        HealthUI = _player.CurrHealth + "";

        if (_bgAnimator)
        {
            _bgAnimator.SetInteger("Health", _player.CurrHealth);
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }
    }

    public void UpdateBombs()
    {
        BombsUI = _player.CurrBombUses + "";
    }

    public void UpdateScore(int points)
    {
        _player.Score += points;
        ScoreUI = _player.Score + "";
    }


    protected void ChangeMenu(string nextMenu)
    {
//        GameManager.instance.NextMenu = nextMenu;
//        Debug.Log(GameManager.instance.NextMenu);
        if (_fadeAnimator && _fadeImage)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }
    }

    protected IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.1f);
        if (_fadeAnimator)
            _fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitUntil(() => _fadeImage.color.a == 1);

//        GameManager.instance.NextSceneName = "Menu_inicial";

        StopAllCoroutines();
    }
}