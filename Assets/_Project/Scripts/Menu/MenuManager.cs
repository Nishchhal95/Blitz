using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Stack<Menu> menuStack = new Stack<Menu>();

    private void OnEnable()
    {
        Menu.OnMenuOpen += OpenMenu;
        Menu.OnMenuClosed += CLoseMenu;
    }

    private void OnDisable()
    {
        Menu.OnMenuOpen -= OpenMenu;
        Menu.OnMenuClosed -= CLoseMenu;
    }

    private void OpenMenu(Menu menu)
    {
        if (menuStack.Count > 0)
        {
            Menu lastActiveMenu = menuStack.Peek();
            lastActiveMenu.CloseWithoutNotify();
        }
        menuStack.Push(menu);
    }

    private void CLoseMenu(Menu menu)
    {
        menuStack.Pop();

        if (menuStack.Count > 0)
        {
            Menu lastActiveMenu = menuStack.Pop();
            lastActiveMenu.Open();
        }
    }
}
