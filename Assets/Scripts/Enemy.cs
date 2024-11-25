using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private char[] guessGrid; // Representa o tabuleiro do inimigo
    private List<int[]> enemyShips; // Lista de navios do inimigo
    private int guess; // Última tentativa do inimigo

    public GameObject enemyMissilePrefab; // Prefab do míssil
    public GameManager gameManager; // Referência ao GameManager

    void Start()
    {
        guessGrid = Enumerable.Repeat('o', 100).ToArray(); // Inicializa o tabuleiro com células vazias ('o')
        enemyShips = PlaceEnemyShips(); // Posiciona os navios do inimigo
    }

    public List<int[]> PlaceEnemyShips()
    {
        // Define os tamanhos dos navios (1x4, 2x3, 3x2, 4x1)
        List<int[]> shipSizes = new List<int[]>
        {
            new int[4],
            new int[3],
            new int[3],
            new int[2],
            new int[2],
            new int[2],
            new int[1],
            new int[1],
            new int[1],
            new int[1]
        };

        // Inicializa os números do tabuleiro
        int[] gridNumbers = Enumerable.Range(0, 100).ToArray();
        List<int[]> placedShips = new List<int[]>();

        foreach (int[] ship in shipSizes)
        {
            bool placed = false;

            while (!placed)
            {
                placed = true;
                int shipNose = UnityEngine.Random.Range(0, 100); // Posição inicial do navio
                int direction = UnityEngine.Random.Range(0, 2); // 0 = vertical, 1 = horizontal
                int step = direction == 0 ? 10 : 1; // Incremento por direção

                // Verifica se o navio pode ser colocado
                for (int i = 0; i < ship.Length; i++)
                {
                    int position = shipNose + (i * step);

                    // Verifica se está fora dos limites
                    if (position < 0 || position >= 100 || gridNumbers[position] == -1 ||
                        (direction == 1 && shipNose / 10 != position / 10))
                    {
                        placed = false;
                        break;
                    }
                }

                // Posiciona o navio se válido
                if (placed)
                {
                    for (int i = 0; i < ship.Length; i++)
                    {
                        int position = shipNose + (i * step);
                        ship[i] = position;
                        gridNumbers[position] = -1; // Marca como ocupado
                    }
                    placedShips.Add(ship);
                }
            }
        }
        return placedShips;
    }

    public void MissileHit(int hitPosition)
    {
        // Atualiza o estado do tabuleiro
        if (enemyShips.Any(ship => ship.Contains(hitPosition)))
        {
            guessGrid[hitPosition] = 'h'; // Acerto
            Debug.Log("Acertou!");
        }
        else
        {
            guessGrid[hitPosition] = 'm'; // Erro
            Debug.Log("Errou!");
        }

        // Finaliza o turno após 1 segundo
        Invoke("EndTurn", 1.0f);
    }

    private void EndTurn()
    {
        gameManager.EndEnemyTurn(); // Chama o GameManager para finalizar o turno do inimigo
    }
}
