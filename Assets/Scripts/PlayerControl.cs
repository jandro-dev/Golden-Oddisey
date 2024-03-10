using System.Collections;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    private CharacterController controlador;
    private Animator animacion;
    private float velocidad = 8.0f;
    private float gravity = 0.0f;
    private bool cameraInside = false;
    private Camera camara; // Camara principal
    private Camera camaraChico; // Camara del chico
    private float velocidadRotacion = 2.0f; // Velocidad de rotación de la cámara
    private float rotacionVerticalX = 0f;
    private bool juegoIniciado = false;
    private bool mostrarInstrucciones = true;
    public int cantidadMonedas;
    private int totalMonedas;
    private float tiempo;
    private float tiempoTotal; // Tiene un getter y un setter

    public float TiempoTotal
    {
        get { return tiempoTotal; }
        set { tiempoTotal = value; }
    }

    void Start()
    {
        // Inicializacion para monedas y tiempos
        cantidadMonedas = 0;
        totalMonedas = 10;
        tiempo = 0.0f;
        tiempoTotal = 30.0f;

        // Inicializaciones para el juego (jugador y camara)
        controlador = GetComponent<CharacterController>();
        animacion = GetComponent<Animator>();
        camara = GameObject.Find("CamaraJuego").GetComponent<Camera>();
        camaraChico = GameObject.Find("CamaraChico").GetComponent<Camera>();
        camaraChico.enabled = false; // Desactivacion de la camaraChico para evitar problemas de doble camara
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !juegoIniciado)
        {
            juegoIniciado = true;
            mostrarInstrucciones = false;
        }
    }

    void OnGUI()
    {
        if (!juegoIniciado)
        {
            if (mostrarInstrucciones)
            {
                GUI.Box(new Rect(50, 30, 250, 250),
                    "\nRecoge las 10 monedas antes \nde que se acabe el tiempo.\n" +
                    "`Cada moneda suma 10 segundos\n\n " +
                    "Puedes alternar entre 1ª y 3ª persona\n utilizando la tecla SHIFT.\n\n" +
                    "En 1ª persona solo te puedes mover \npara adelante y con el raton\n\n" +
                    "En 3ª persona te puedes mover en \ncualquier direccion sin giro de camara\n\n" +
                    "Presiona R para comenzar");
            }
        }
        else
        {
            if (tiempo >= tiempoTotal)
            {
                SceneManager.LoadScene(3); // Escena Juego Perdido
            }
            else
            {
                if (cantidadMonedas < totalMonedas)
                {
                    GUI.Label(new Rect(10, 10, 150, 100), "Monedas: " + cantidadMonedas + " / " + totalMonedas);
                    GUI.Label(new Rect(10, 30, 150, 100), "Tiempo: " + (int)(tiempoTotal - tiempo));
                }
                else
                {
                    SceneManager.LoadScene(2); // Escena Juego Ganado
                }
            }
        }
    }

    void LateUpdate()
    {
        if(juegoIniciado)
        {
            // Contador del tiempo
            tiempo += Time.deltaTime;

            // Gravedad del jugador
            gravity -= 9.81f * Time.deltaTime;

            // Inicializacion del movimiento ( no se mueve aqui )
            Vector3 movimiento = Vector3.zero;

            // Aplicar gravedad al movimiento vertical
            movimiento.y = gravity;

            // Coger la posicion del jugador
            Vector3 posicionPlayer = controlador.gameObject.transform.position;

            // Cambiar la gravedad si el jugador esta tocando el suelo
            if (controlador.isGrounded) gravity = 0;

            // Girar la cámara con el ratón si está en primera persona
            if (cameraInside)
            {
                // Movimiento del jugador solo hacia adelante
                if(Input.GetKey(KeyCode.W))
                {
                    movimiento = transform.forward * velocidad;

                    // Limites hasta los que llega el jugador
                    movimiento = LimiteJugador(posicionPlayer,movimiento);

                    // Iniciar la animacion de correr
                    animacion.Play("run");
                }
                else
                {
                    // Iniciar la animacion estando quieto
                    animacion.Play("idle");
                }

                // Asignacion de los ejes de rotacion del raton
                float mouseX = Input.GetAxis("Mouse X") * velocidadRotacion;
                float mouseY = Input.GetAxis("Mouse Y") * velocidadRotacion;

                // Rotar la cámara horizontalmente
                transform.Rotate(0, mouseX, 0);
                
                // Actualizar la rotación vertical de la cámara
                rotacionVerticalX -= mouseY;

                // Limitar la rotación vertical
                rotacionVerticalX = Mathf.Clamp(rotacionVerticalX, -20f, 30f);

                // Aplicar la rotación vertical a la cámara
                camaraChico.transform.localEulerAngles = new Vector3(rotacionVerticalX, 0, 0);
            }
            else // En tercera persona solo se mueve el personaje sin movimiento de camara
            {
                Vector3 inputMovimiento = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

                if (inputMovimiento != Vector3.zero)
                {
                    animacion.Play("run");

                    // Rotar el personaje hacia la dirección del movimiento
                    transform.forward = inputMovimiento.normalized;

                    // Aplicar el movimiento en la dirección de la tecla pulsada
                    movimiento = transform.forward * velocidad;

                    movimiento = LimiteJugador(posicionPlayer,movimiento);
                }
                else
                {
                    animacion.Play("idle");
                }  
            }

            // Movimiento del jugador
            controlador.Move(movimiento * Time.deltaTime);

            // Posicion camara 1/3 persona
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (cameraInside)
                {
                    // Rotación inclinada de la cámara cuando está en tercera persona
                    camara.transform.rotation = Quaternion.Euler(new Vector3(21.8f, 0f, 0f));
                    cameraInside = false;

                    // Cambio de camara a la principal para 3ª persona
                    camaraChico.enabled = false;
                    camara.enabled = true;
                }
                else
                {
                    // Cambio de camara a la del chico para 1ª persona
                    camara.enabled = false;
                    camaraChico.enabled = true;

                    cameraInside = true;

                    // Prueba con una sola camara ( no funciona el posicionamiento )
                    // Adaptación de la cámara para la primera persona
                    //camara.transform.position = posicionPlayer + new Vector3(0.1f, 2.7f, 0f);
                    // Reestablecer rotacion de la cámara cuando está en primera persona
                    //camara.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                }
            }
        }
    }
    

    Vector3 LimiteJugador(Vector3 posicionPlayer, Vector3 movimiento) 
    {
        Vector3 movimientoLimite = movimiento;

        // Restringir movimiento en el eje X
        if (posicionPlayer.x <= 26f && movimientoLimite.x < 0f)
        {
            movimientoLimite.x = 0f;
        }
        else if (posicionPlayer.x >= 179f && movimientoLimite.x > 0f)
        {
            movimientoLimite.x = 0f;
        }

        // Restringir movimiento en el eje Z
        if (posicionPlayer.z <= 24f && movimientoLimite.z < 0f)
        {
            movimientoLimite.z = 0f;
        }
        else if (posicionPlayer.z >= 168f && movimientoLimite.z > 0f)
        {
            movimientoLimite.z = 0f;
        }

        return movimientoLimite;
    }
   
}