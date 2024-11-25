using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject painelInit;
    [SerializeField] private GameObject Config;



    public void Jogar()
    {
        SceneManager.LoadScene("Niveis");
    }

    public void AbrirConfig()
    {
        painelInit.SetActive(false);
        painelConfig.SetActive(true);
    }

    public void FecharConfig()
    {
        painelConfig.setActive(false);
        painelInit.setActive(true);
    }

    public void Niveis()
    {
        SceneManager.LoadScene("Game");
    }


}
