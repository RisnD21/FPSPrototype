using UnityEngine;
using DG.Tweening;
using System;

//控制所有面板的開關，並更新 PlayerInput 中的狀況
public class InGameMenu : MonoBehaviour
{
    public void OpenMenu()
    {
        PopUpMenuAnim();
    }

    public void CloseMenu()
    {
        CloseMenuAnim();
    }

    void PopUpMenuAnim()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
    }

    void CloseMenuAnim()
    {
        transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
    }




}