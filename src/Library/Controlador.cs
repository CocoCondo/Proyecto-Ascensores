namespace Proyecto;

public class Controlador
{
    /// <summary>
    /// Mutex para bloquear la cola de solicitudes al operar sobre ella
    /// </summary>
    /// <returns></returns>
    public Mutex mutexColaSolicitudes = new Mutex();
    /// <summary>
    /// Cantidad de pisos que tiene el edificio
    /// </summary>
    public static int numPisos = 10;

    /// <summary>
    /// Cantidad de ascensores para el edificio
    /// </summary>
    private static int cantAscensores = 2;

    /// <summary>
    /// Variable que modifica el estado de operación del controlador.
    /// </summary>
    /// <value>True o False</value>
    private bool stopControlador { set; get; }
    /// <summary>
    /// /// Aqui se guardan las solicitudes en una cola.
    /// Esto sirve para mantenerlo >>FIFO
    /// </summary>
    /// <typeparam name="Solicitud"></typeparam>
    /// <returns></returns>
    public Queue<int> colaPisos { get; private set; } = new Queue<int>(); //Adentro tiene Piso.numPiso's
    private List<Ascensor> listaAscensores = new List<Ascensor>(cantAscensores);
    public Piso[] pisosEdificio { get; private set; } = new Piso[numPisos]; //Creo un array con de tamaño cantPisos. Cada piso tiene su propia cola
    /// <summary>
    /// Crea un atributo instance para instanciar el Singleton
    /// </summary>
    private static Controlador instance;

    /// <summary>
    /// Constructor de Controlador
    /// </summary>
    private Controlador()
    {
        stopControlador = false;
        for (int i = 0; i < numPisos; i++)
        {
            pisosEdificio[i] = new Piso(i); //Inicializo un piso por cada piso del array
        }
    }
    /// <summary>
    /// Método para agregar solicitudes a los pisos del edificio
    /// </summary>
    /// <param name="piso">El INT que identifica el número de piso desde el que se pide el ascensor</param>
    /// <param name="solicitud">El objeto SOLICITUD en cuestión</param>
    public void agregarSolicitudes(int piso, Solicitud solicitud)
    {
        this.pisosEdificio[piso].colaSolPiso.Enqueue(solicitud, solicitud.prioridad);
        this.colaPisos.Enqueue(piso);
    }

    /// <summary>
    /// Singleton para el Controlador
    /// </summary>
    /// <returns>Instancia del controlador</returns>
    public static Controlador GetInstance()
    {
        if (instance == null)
        {
            instance = new Controlador();
        }
        return instance;
    }

    public void IniciarAscensores()
    {
        for (int i = 0; i < cantAscensores; i++)
        {
            Ascensor ascensor = new Ascensor(i);
            this.listaAscensores.Add(ascensor);
            Thread asc = new Thread(new ThreadStart(ascensor.Run));
            asc.Start();
        }
    }

    // pisoSolicitud sale de colaPisos
    private Ascensor buscarAscensor(int pisoSolicitud)
    {
        int mayor = int.MaxValue;
        Ascensor resultado = null;
        foreach (Ascensor ascensor in listaAscensores)
        {
            if (ascensor.pesoAscensor > 300)
            { //Si el ascensor tiene 300 KG (Osea está al borde), ignora la solicitud entrante
                continue;
            }
            else if (ascensor.direccion == Direccion.STOP)   //Si el ascensor está detenido...
            {
                if (Math.Abs(ascensor.pisoActual - pisoSolicitud) < mayor)  //Si el valor abs. de la dif. de piso es la menor
                {
                    mayor = ascensor.pisoActual;    //Se sobreescribe mayor para comparar los demás      
                    resultado = ascensor;   //Tomamos ese ascensor                                       
                }
            }
            else if (ascensor.direccion == Direccion.ARRIBA && pisoSolicitud > ascensor.pisoActual) //Si el ascensor va para arriba y está abajo del piso origen
            {
                if (Math.Abs(ascensor.pisoActual - pisoSolicitud) < mayor)  //Si el valor abs. de la dif. de piso es la menor
                {
                    resultado = ascensor;   //Tomamos ese ascensor
                }
            }
            else if (ascensor.direccion == Direccion.ARRIBA && pisoSolicitud < ascensor.pisoActual)
            {
                continue; //Si está yendo para arriba y ya pasó el piso, lo ignora como un campeón
            }
            /*else //Para todo lo demás
            { 
                if (Math.Abs(ascensor.pisoActual - pisoSolicitud) < mayor)  //Si el valor abs. de la dif. de piso es la menor
                {
                    resultado = ascensor;   //Tomamos ese ascensor
                }
            } A ver si teniendo esto comentado no se rompe todo ======*/
        }
        return resultado; //Retorna el ascensor
    }

    // MAIN
    /// <summary>
    /// Metodo run para correr el hilo principal del controlador
    /// </summary>
    public void run()
    {
        IniciarAscensores();
        while (true)
        {
            // proteger lectura de colaSolicitudes
            mutexColaSolicitudes.WaitOne();
            if(colaPisos.Count() != 0)
            {
                int solPisoActual = colaPisos.Dequeue(); //Toma el primer elemento de la cola (FIFO)
                mutexColaSolicitudes.ReleaseMutex();
                Ascensor ascensor = buscarAscensor(solPisoActual); //Busco el ascensor que esté más cerca de la solicitud
                if (ascensor != null)
                {
                    ascensor.listaParadas.Add(solPisoActual); //Agrega la solicitud a la lista de paradas del ascensor seleccionado
                }
                mutexColaSolicitudes.WaitOne();
            }
            mutexColaSolicitudes.ReleaseMutex();

        }
    }
}

/*
==================== RESUMEN DE COSAS ============
[x]Tiene que haber un array de tamaño cantPisos para representar el edificio
[x]    Cada elto. del array tiene un objeto "Piso"
    
[x]Tiene que haber una clase Piso
[x]    Contiene una *cola DE PRIORIDAD* de "Solicitud"

Ascensor tiene que levantar las solicitudes hechas en el piso al llegar al piso requerido (esto es, cuando el controlador le dijo que vaya a ese piso)
[x]    tiene que tener una cola de solicitudes
[x]    tiene que controlar el peso cada vez que levanta una solicitud
    Si quedan solicitudes en la cola del piso
[x]        tiene que agregar la solicitud de piso a la cola de solicitudes del controlador
[x]    Si ascensor está subiendo
[x]    en el run: if(hay solicitudes) hace todo lo que tiene que hacer
[x]                else: Direccion = Direccion.STOP
*/

// ASUMIMOS que todas las solicitudes, salvo las del piso 0, van a bajar