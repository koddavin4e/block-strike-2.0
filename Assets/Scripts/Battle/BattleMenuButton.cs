
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMenuButton : MonoBehaviour
{
    public void OpenBattle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BattleScene");
    }
}
