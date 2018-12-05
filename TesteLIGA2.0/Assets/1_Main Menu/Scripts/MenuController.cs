using System;
using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MenuPanel _currPanel;
    public Action OnFadeIn;
    private WaitUntil _waitUntil;

    public void ChangeMenu(MenuPanel nextPanel)
    {
        OnFadeIn = () =>
        {
            _currPanel.ResetAnimState();

            _currPanel.gameObject.SetActive(false);
            nextPanel.gameObject.SetActive(true);
            _currPanel = nextPanel;
        };

        _waitUntil = new WaitUntil(() => _currPanel._anim.GetCurrentAnimatorStateInfo(0).IsTag("Exit"));
        StartCoroutine("FadeIn");
    }

    public IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.1f);
        if (_currPanel != null)
            _currPanel._anim.SetTrigger("Exit");

        yield return _waitUntil;

        if (OnFadeIn != null)
            OnFadeIn();
    }
}