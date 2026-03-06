using System.Collections.Generic;
using UnityEngine;

public class CustomerHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private List<GameObject> _tomatoes;

    public void PlayAnim(string name)
    {
        _animator.Play(name);
    }

    public void ShowTomatoes(bool show)
    {
        foreach (var tomato in _tomatoes)
        {
            tomato.SetActive(show);
        }
    }
}
