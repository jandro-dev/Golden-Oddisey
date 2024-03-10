using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        // Verificar si el objeto con el que colisiona tiene el tag "Player"
        if (collider.CompareTag("Player"))
        {
            // Destruye la moneda
            Destroy(gameObject);
            collider.gameObject.GetComponent<PlayerControl>().cantidadMonedas++;
            collider.gameObject.GetComponent<PlayerControl>().TiempoTotal += 10;
        }
    }
}
