using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Manager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject celulaPrefab;
    public GameObject gridContainer;

    [Header("UI Elements")]
    public TextMeshProUGUI tentativasText;
    public TextMeshProUGUI mensagemText;
    public GameObject painelDificuldade;
    public GameObject painelJogo;
    public GameObject painelFimJogo;
    public TextMeshProUGUI textoFimJogo;

    private const int TAMANHO_TABULEIRO = 10;
    private Celula[,] tabuleiro;
    private char[,] tabuleiroMaquina;
    private int tentativasRestantes;
    private int naviosRestantes;
    private bool jogoEmAndamento = false;

    private void Start()
    {
        painelDificuldade.SetActive(true);
        painelJogo.SetActive(false);
        painelFimJogo.SetActive(false);
    }

    public void IniciarJogo(int dificuldade)
    {
        switch (dificuldade)
        {
            case 1: tentativasRestantes = 20; break; // F�cil
            case 2: tentativasRestantes = 10; break; // M�dio
            case 3: tentativasRestantes = 6; break;  // Dif�cil
        }

        painelDificuldade.SetActive(false);
        painelJogo.SetActive(true);

        InicializarTabuleiro();
        PosicionarNavios();
        jogoEmAndamento = true;
        AtualizarUI();
    }

    private void InicializarTabuleiro()
    {
        tabuleiro = new Celula[TAMANHO_TABULEIRO, TAMANHO_TABULEIRO];
        tabuleiroMaquina = new char[TAMANHO_TABULEIRO, TAMANHO_TABULEIRO];

        // Limpa o grid container
        foreach (Transform child in gridContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Cria o grid visual
        for (int i = 0; i < TAMANHO_TABULEIRO; i++)
        {
            for (int j = 0; j < TAMANHO_TABULEIRO; j++)
            {
                GameObject novaCelula = Instantiate(celulaPrefab, gridContainer.transform);
                Celula componenteCelula = novaCelula.GetComponent<Celula>();
                componenteCelula.Inicializar(i, j, this);
                tabuleiro[i, j] = componenteCelula;
                tabuleiroMaquina[i, j] = '~';
            }
        }
    }

    private bool PosicaoValida(int linha, int coluna, int tamanho, bool horizontal)
    {
        if (horizontal)
        {
            if (coluna + tamanho > TAMANHO_TABULEIRO) return false;
            for (int j = coluna; j < coluna + tamanho; j++)
            {
                if (tabuleiroMaquina[linha, j] != '~') return false;
                // Verifica c�lulas adjacentes
                for (int di = -1; di <= 1; di++)
                {
                    for (int dj = -1; dj <= 1; dj++)
                    {
                        int novaLinha = linha + di;
                        int novaColuna = j + dj;
                        if (novaLinha >= 0 && novaLinha < TAMANHO_TABULEIRO &&
                            novaColuna >= 0 && novaColuna < TAMANHO_TABULEIRO)
                        {
                            if (tabuleiroMaquina[novaLinha, novaColuna] != '~')
                                return false;
                        }
                    }
                }
            }
        }
        else
        {
            if (linha + tamanho > TAMANHO_TABULEIRO) return false;
            for (int i = linha; i < linha + tamanho; i++)
            {
                if (tabuleiroMaquina[i, coluna] != '~') return false;
                // Verifica c�lulas adjacentes
                for (int di = -1; di <= 1; di++)
                {
                    for (int dj = -1; dj <= 1; dj++)
                    {
                        int novaLinha = i + di;
                        int novaColuna = coluna + dj;
                        if (novaLinha >= 0 && novaLinha < TAMANHO_TABULEIRO &&
                            novaColuna >= 0 && novaColuna < TAMANHO_TABULEIRO)
                        {
                            if (tabuleiroMaquina[novaLinha, novaColuna] != '~')
                                return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void PosicionarNavio(int tamanho, char simbolo)
    {
        bool posicionado = false;
        while (!posicionado)
        {
            int linha = Random.Range(0, TAMANHO_TABULEIRO);
            int coluna = Random.Range(0, TAMANHO_TABULEIRO);
            bool horizontal = Random.Range(0, 2) == 0;

            if (PosicaoValida(linha, coluna, tamanho, horizontal))
            {
                if (horizontal)
                {
                    for (int j = coluna; j < coluna + tamanho; j++)
                    {
                        tabuleiroMaquina[linha, j] = simbolo;
                    }
                }
                else
                {
                    for (int i = linha; i < linha + tamanho; i++)
                    {
                        tabuleiroMaquina[i, coluna] = simbolo;
                    }
                }
                posicionado = true;
            }
        }
    }

    private void PosicionarNavios()
    {
        // 1 navio de tamanho 4
        PosicionarNavio(4, '4');

        // 2 navios de tamanho 3
        for (int i = 0; i < 2; i++)
            PosicionarNavio(3, '3');

        // 3 navios de tamanho 2
        for (int i = 0; i < 3; i++)
            PosicionarNavio(2, '2');

        // 4 navios de tamanho 1
        for (int i = 0; i < 4; i++)
            PosicionarNavio(1, '1');

        naviosRestantes = 10;
    }

    public void ProcessarTiro(int linha, int coluna)
    {
        if (!jogoEmAndamento || tabuleiro[linha, coluna].JaAtacada)
            return;

        bool acertou = tabuleiroMaquina[linha, coluna] != '~';

        if (acertou)
        {
            tabuleiro[linha, coluna].MarcarAcerto();
            mensagemText.text = "Acertou um navio!";
            VerificarNavioDestruido(linha, coluna);
        }
        else
        {
            tabuleiro[linha, coluna].MarcarErro();
            mensagemText.text = "�gua!";
            tentativasRestantes--;
        }

        AtualizarUI();
        VerificarFimJogo();
    }

    private void VerificarNavioDestruido(int linha, int coluna)
    {
        char tipoNavio = tabuleiroMaquina[linha, coluna];
        bool navioDestruido = true;

        // Verifica horizontalmente
        for (int j = 0; j < TAMANHO_TABULEIRO; j++)
        {
            if (tabuleiroMaquina[linha, j] == tipoNavio && !tabuleiro[linha, j].JaAtacada)
            {
                navioDestruido = false;
                break;
            }
        }

        // Verifica verticalmente se n�o foi encontrado horizontalmente
        if (navioDestruido)
        {
            for (int i = 0; i < TAMANHO_TABULEIRO; i++)
            {
                if (tabuleiroMaquina[i, coluna] == tipoNavio && !tabuleiro[i, coluna].JaAtacada)
                {
                    navioDestruido = false;
                    break;
                }
            }
        }

        if (navioDestruido)
        {
            mensagemText.text = "Navio destru�do!";
            naviosRestantes--;
        }
    }

    private void AtualizarUI()
    {
        tentativasText.text = $"Tentativas restantes: {tentativasRestantes}";
    }

    private void VerificarFimJogo()
    {
        if (naviosRestantes <= 0 || tentativasRestantes <= 0)
        {
            jogoEmAndamento = false;
            painelFimJogo.SetActive(true);

            if (naviosRestantes <= 0)
                textoFimJogo.text = "Parab�ns! Voc� venceu!";
            else
                textoFimJogo.text = "Game Over! Suas tentativas acabaram.";
        }
    }

    public void ReiniciarJogo()
    {
        painelFimJogo.SetActive(false);
        painelDificuldade.SetActive(true);
        painelJogo.SetActive(false);
    }
}

// Celula.cs
public class Celula : MonoBehaviour
{
    public Image background;
    public bool JaAtacada { get; private set; }

    private int linha;
    private int coluna;
    private BatalhaNavalManager manager;
    private Button botao;

    public void Inicializar(int l, int c, BatalhaNavalManager mgr)
    {
        linha = l;
        coluna = c;
        manager = mgr;
        JaAtacada = false;

        botao = GetComponent<Button>();
        botao.onClick.AddListener(() => Clicar());
    }

    private void Clicar()
    {
        if (!JaAtacada)
        {
            manager.ProcessarTiro(linha, coluna);
        }
    }

    public void MarcarAcerto()
    {
        JaAtacada = true;
        background.color = Color.red;
        botao.interactable = false;
    }

    public void MarcarErro()
    {
        JaAtacada = true;
        background.color = Color.blue;
        botao.interactable = false;
    }
}
