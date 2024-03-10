using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorMonedas : MonoBehaviour
{
    public GameObject coin;
    public int cantidadMonedas = 10;
    
    void Start()
    {
        CrearMonedas();
    }

    void CrearMonedas()
    {
        float limiteMinX = 30f;
        float limiteMaxX = 170f;
        float limiteMinZ = 30f;
        float limiteMaxZ = 165f;
        float distanciaCoin = 30f;
        float altura = 1.5f;

        List<Vector3> posicionMonedas = new List<Vector3>();

        for (int i = 0; i < cantidadMonedas; i++)
        {
            Vector3 nuevaPosicion;

            do {
                float randomX = Random.Range(limiteMinX, limiteMaxX);
                float randomZ = Random.Range(limiteMinZ, limiteMaxZ);

                nuevaPosicion = new Vector3(randomX, altura, randomZ);
            } 
            while (PosicionEsCercana(nuevaPosicion, posicionMonedas, distanciaCoin));

            GameObject nuevaInstancia = Instantiate(coin, nuevaPosicion, Quaternion.Euler(90f,180f,0f));

            nuevaInstancia.AddComponent<Moneda>();

            posicionMonedas.Add(nuevaPosicion);

        }
    }

    // Metodo para saber si la posicion de la siguiente moneda a crear es demasiado cercana entre las generadas
    bool PosicionEsCercana(Vector3 nuevaPosicion, List<Vector3> posicionesExistentes, float separacionMinima)
    {
        foreach (Vector3 posicionExistente in posicionesExistentes)
        {
            if (Vector3.Distance(nuevaPosicion, posicionExistente) < separacionMinima)
            {
                return true; // Comprueba si la posicion es cercana. Si lo es genera otra posicion
            }
        }
        return false; // No genera una posicion nueva si cumple con la separacion
    }


}
