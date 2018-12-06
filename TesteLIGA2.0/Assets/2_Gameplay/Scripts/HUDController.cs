using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Fading")]
    [SerializeField] private Animator _fadeAnimator;
    [Header("Game")]
    [SerializeField] private Animator _bgAnimator;
    [SerializeField] private Character _player;
    [SerializeField] private Text _healthUI;
    [SerializeField] private Text _bombsUI;
    [SerializeField] private Text _scoreUI;
    [Header("GameOver")] 
    [SerializeField] private GameObject _gameOverPanel;
    
    protected void Awake()
    {
        _player.OnTakeDamage += UpdateHealth;
        _player.OnScorePoint += UpdateScore;
        _player.OnUseBomb += UpdateBombs;
        _player.OnPickupBombs += UpdateBombs;
        _player.OnPlayerDeath += GameOver;
    }

    protected void Start()
    {
//        EnemyManager.instance.OnEnemyDeath += _player.ScorePoint; //TODO: Change the OnEnemyDeath event to be in the Enemy script not the EnemyManager

        UpdateHealth();
        UpdateBombs();
        UpdateScore();
    }

    private void GameOver()
    {
        if (_App.RankingManager)
        {
            _App.RankingManager.StatBuffer = new PlayerStat(){ Name = "", Score = _player.Score };
            if (_App.RankingManager.CheckHighScore(_App.RankingManager.StatBuffer))
            {
                _gameOverPanel.SetActive(true);
            }
            else
            {
                StartFadeIn();
            }

            _App.Instance.nextMenuItem = MenuItems.GameOver;
        }
    }

    public void ChangeName(Text txt)
    {
        _App.RankingManager.StatBuffer.Name = txt.text;
    }
    
    public void StartFadeIn()
    {
        StartCoroutine("Fade");
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(0.1f);
        if (_fadeAnimator != null)
            _fadeAnimator.SetTrigger("Exit");
        
        WaitUntil _waitUntil = new WaitUntil(() => _fadeAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Exit"));
        yield return _waitUntil;
        if(_App.SceneSwapManager)
            _App.SceneSwapManager.ChangeScene(1);
    }

    public void UpdateHealth()
    {
        _healthUI.text = _player.CurrHP + "";

        if (_bgAnimator)
        {
            _bgAnimator.SetInteger("Health", _player.CurrHP);
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }
    }

    public void UpdateBombs()
    {
        _bombsUI.text = _player.CurrBombUses + "";
    }

    public void UpdateScore()
    {
        _scoreUI.text = _player.Score + "";
    }
}