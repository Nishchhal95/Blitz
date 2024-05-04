using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeMenu : Menu
{
    [SerializeField] private List<ButtonToMenu> buttonToMenus = new List<ButtonToMenu>();

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var buttonToMenu in buttonToMenus)
        {
            buttonToMenu.OpenMenuButton.onClick.AddListener(buttonToMenu.Menu.Open);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (var buttonToMenu in buttonToMenus)
        {
            buttonToMenu.OpenMenuButton.onClick.RemoveListener(buttonToMenu.Menu.Open);
        }
    }
}

[System.Serializable]
public class ButtonToMenu
{
    public Button OpenMenuButton;
    public Menu Menu;
}
