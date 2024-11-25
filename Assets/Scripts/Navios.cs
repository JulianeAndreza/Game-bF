using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navios : MonoBehaviour
{
    public float xOffset = 0; // Offset horizontal
    public float zOffset = 0; // Offset vertical
    private float nextZRotation = 90f; // Próximo ângulo de rotação
    private GameObject clickedTile; // Célula clicada
    public int shipSize; // Tamanho do navio
    private int hitCount = 0; // Contagem de acertos no navio

    private Material[] allMaterials; // Materiais do navio
    private List<GameObject> touchTiles = new List<GameObject>(); // Células em contato
    private List<Color> allColors = new List<Color>(); // Cores originais dos materiais

    void Start()
    {
        allMaterials = GetComponent<Renderer>().materials;
        foreach (var material in allMaterials)
        {
            allColors.Add(material.color);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Celula"))
        {
            // Adiciona as células tocadas à lista
            if (!touchTiles.Contains(collision.gameObject))
            {
                touchTiles.Add(collision.gameObject);
            }
        }
    }

    public void ClearTileList()
    {
        touchTiles.Clear(); // Limpa a lista de células tocadas
    }

    public Vector3 GetOffsetVec(Vector3 tilePos)
    {
        // Retorna a posição ajustada com os offsets
        return new Vector3(tilePos.x + xOffset, 2, tilePos.z + zOffset);
    }

    public void RotateShip()
    {
        if (clickedTile == null) return;

        touchTiles.Clear();
        transform.localEulerAngles += new Vector3(0, 0, nextZRotation);
        nextZRotation *= -1; // Alterna a rotação entre 90° e -90°

        // Ajusta os offsets para refletir a nova orientação
        float temp = xOffset;
        xOffset = zOffset;
        zOffset = temp;

        // Reposiciona o navio com base na nova rotação
        SetPosition(clickedTile.transform.position);
    }

    public void SetPosition(Vector3 newVec)
    {
        // Reposiciona o navio em relação à célula clicada
        ClearTileList();
        transform.localPosition = new Vector3(newVec.x + xOffset, 2, newVec.z + zOffset);
    }

    public void SetClickedTile(GameObject tile)
    {
        // Define a célula clicada para posicionamento
        clickedTile = tile;
    }

    public bool OnGameBoard()
    {
        // Verifica se o navio está posicionado corretamente no tabuleiro
        return touchTiles.Count == shipSize;
    }

    public bool HitCheckSank()
    {
        // Incrementa a contagem de acertos e verifica se o navio foi afundado
        hitCount++;
        return hitCount >= shipSize;
    }

    public void FlashColor(Color tempColor)
    {
        // Altera a cor do navio temporariamente para indicar um evento (ex.: acerto)
        foreach (var material in allMaterials)
        {
            material.color = tempColor;
        }
        Invoke("ResetColor", 0.5f); // Reseta a cor após 0.5s
    }

    private void ResetColor()
    {
        // Restaura a cor original dos materiais
        for (int i = 0; i < allMaterials.Length; i++)
        {
            allMaterials[i].color = allColors[i];
        }
    }
}
