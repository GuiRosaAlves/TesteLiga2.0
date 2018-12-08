using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MenuItems{ MainMenu, Ranking, Configs, GameOver}
public class MenuController : MonoBehaviour
{
    [SerializeField] private EventSystem _evtSys;
    [SerializeField] private MenuPanel[] _panels;
    public Action OnFadeIn;
    private MenuPanel _currPanel;
    private WaitUntil _waitUntil;

    public void Awake()
    {
        _currPanel = _App.Instance ? _panels[(int) _App.Instance.nextMenuItem] : _panels[(int) MenuItems.MainMenu];

        if (!_currPanel.gameObject.activeSelf)
            _currPanel.gameObject.SetActive(true);
        
        _evtSys.SetSelectedGameObject(_currPanel.Btns[0].gameObject);
    }
    
    public void ChangeMenu(MenuPanel nextPanel)
    {
        OnFadeIn = () =>
        {
            _currPanel.ResetAnimState();

            _currPanel.gameObject.SetActive(false);
            nextPanel.gameObject.SetActive(true);
            _evtSys.SetSelectedGameObject(nextPanel.Btns[0].gameObject);
            _currPanel = nextPanel;
        };
        
        _waitUntil = new WaitUntil(() => _currPanel.Anim.GetCurrentAnimatorStateInfo(0).IsTag("Exit"));
        StartCoroutine("FadeIn");
    }
    
    public void ChangeMenu(MenuItems nextPanel)
    {
        OnFadeIn = () =>
        {
            _currPanel.ResetAnimState();

            _currPanel.gameObject.SetActive(false);
            _panels[(int)nextPanel].gameObject.SetActive(true);
            _evtSys.SetSelectedGameObject(_panels[(int)nextPanel].Btns[0].gameObject);
            _currPanel = _panels[(int)nextPanel];
        };
        
        _waitUntil = new WaitUntil(() => _currPanel.Anim.GetCurrentAnimatorStateInfo(0).IsTag("Exit"));
        StartCoroutine("FadeIn");
    }

    public IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (_currPanel != null)
            _currPanel.Anim.SetTrigger("Exit");

        yield return _waitUntil;

        if (OnFadeIn != null)
            OnFadeIn();
    }
}