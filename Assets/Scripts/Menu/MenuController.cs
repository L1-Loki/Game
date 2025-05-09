using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu; 
    public GameObject subMenu;      
    void Start()
    {
        // hiển thị menu, ẩn submenu
        mainMenu.SetActive(true);
        subMenu.SetActive(false);
    }

    // nhấn nút để mở submenu
    public void OpenSubMenu()
    {
        mainMenu.SetActive(false); 
        subMenu.SetActive(true);   
    }

    // quay lại menu chính từ submenu
    public void BackToMainMenu()
    {
        subMenu.SetActive(false);  
        mainMenu.SetActive(true);  
    }
}
