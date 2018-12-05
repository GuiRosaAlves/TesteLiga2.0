using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuController3 : MonoBehaviour
{
    [SerializeField] private List<GameObject> _menuList;
    [SerializeField] private Animator _fadeAnimator;
    [SerializeField] private Image _fadeImage;
    [Header("Configurações")]
    [SerializeField] private Text _soundEnabledText;
    [Header("Ranking")]
    [SerializeField] private Text[] _slotRankText;
    [SerializeField] private Text[] _slotNameText;
    [SerializeField] private Text[] _slotScoreText;
    [Header("Game Over")]
    [SerializeField] private Text _scoreTxt;
    [SerializeField] private GameObject _scoreLabel;
    [SerializeField] private GameObject _highScore;
    [SerializeField] private InputField _nameInput;
    private string SoundEnabledText { get { return _soundEnabledText.text; } set { _soundEnabledText.text = value; } }
    private string ScoreTxt { get { return _scoreTxt.text; } set { _scoreTxt.text = value; } }

    private string nextSceneBuffer = "";

    protected void Awake()
    {
        UpdateConfigs();
    }

    protected void Start()
    {
//        if (GameManager.instance)
//        {
//            GameManager.instance.CurrMenu = _menuList[0].name;
//            if (GameManager.instance.NextMenu == null && _menuList.Count > 0)
//            {
//                GameManager.instance.NextMenu = _menuList[0].name;
//                GameManager.instance.CurrMenu = GameManager.instance.NextMenu;
//            }
//            else
//            {
//                UpdateMenu();
//            }
//        }
    }

    public void StartFadeIn()
    {
        if (_fadeAnimator && _fadeImage)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            Debug.Log("There are null references in the editor!");
        }
    }

    public IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.1f);
        if (_fadeAnimator)
            _fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitUntil(() => _fadeImage.color.a == 1);

        UpdateMenu();
        if (nextSceneBuffer != "")
        {
//            GameManager.instance.NextSceneName = nextSceneBuffer;
            nextSceneBuffer = "";
        }

        StopAllCoroutines();
    }

    public void ChangeMenu(string nextMenu)
    {
//        GameManager.instance.NextMenu = nextMenu;
        StartFadeIn();
    }

    public void ChangeScene(string nextScene)
    {
        nextSceneBuffer = nextScene;
        StartFadeIn();
    }

    protected void UpdateMenu()
    {
//        if (GameManager.instance.CurrMenu != GameManager.instance.NextMenu)
//        {
//            GameObject currMenu = GetMenu(GameManager.instance.CurrMenu);
//            GameObject nextMenu = GetMenu(GameManager.instance.NextMenu);
//            if (currMenu && nextMenu)
//            {
//                nextMenu.SetActive(true);
//                GameManager.instance.CurrMenu = GameManager.instance.NextMenu;
//                currMenu.SetActive(false);
//                _fadeAnimator.SetTrigger("FadeOut");
//            }
//
//            if (GameManager.instance.CurrMenu == "GameOver")
//            {
//                if (HighScoreManager.AddScore(HighScoreManager.PlayerBuffer))
//                {
//                    _scoreLabel.SetActive(false);
//                    _highScore.SetActive(true);
//                    _nameInput.ActivateInputField();
//                }
//                else
//                {
//                    _highScore.SetActive(false);
//                    _scoreLabel.SetActive(true);
//                }
//                ScoreTxt = HighScoreManager.PlayerBuffer.score + "";
//            }
//        }
    }

    protected GameObject GetMenu(string filter)
    {
        for (int i = 0; i < _menuList.Count; i++)
        {
            if (_menuList[i].name == filter)
            {
                return _menuList[i];
            }
        }
        return null;
    }

    public void ValidateInput(InputField input)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            HighScoreManager3.PlayerBuffer.Name = input.text;
            HighScoreManager3.SaveScore();
        }
        else
        {
            input.ActivateInputField();
        }
    }

    public void UpdateScoreBoard()
    {
        List<PlayerStat> scoreBoard = HighScoreManager3.ScoreBoard;
        for (int i = 0; i < scoreBoard.Count; i++)
        {
            _slotRankText[i].gameObject.SetActive(true);
            _slotRankText[i].text = (i+1) + ".";
            _slotNameText[i].gameObject.SetActive(true);
            _slotNameText[i].text = scoreBoard[i].Name;
            _slotScoreText[i].gameObject.SetActive(true);
            _slotScoreText[i].text = scoreBoard[i].Score + " Pts";
        }
    }

    public void UpdateConfigs()
    {
        if (AudioManager.instance)
        {
            SoundEnabledText = AudioManager.instance.SoundState ? "LIGADA" : "DESLIGADA";
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}