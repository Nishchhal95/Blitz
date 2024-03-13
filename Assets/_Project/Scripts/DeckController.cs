using System;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public event Action OnDeckClicked;
    private void OnMouseUp()
    {
        OnDeckClicked?.Invoke();
    }
}
