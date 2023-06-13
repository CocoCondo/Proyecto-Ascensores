using System.Diagnostics;

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
    /// <summary>
    /// Sirve para guardar los tiempos de espera que se van pasando en el archivo de solicitudes
    /// </summary>
    /// <typeparam name="int">Tiempo</typeparam>
    /// <returns></returns>
    public Stack<int> stackEspera { get; set; } = new Stack<int>();
    public static Stopwatch sw;
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
        sw = Stopwatch.StartNew();
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
    /// Agrega una instancia de espera al controlador
    /// </summary>
    /// <param name="tEspera">Tiempo en segundos</param>
    public void agregarEspera(int tEspera)
    {
        this.stackEspera.Push(tEspera); //Agrega el tiempo que va a esperar en segundos al stack
        this.colaPisos.Enqueue(-1); //Agrega el -1 a la cola de pisos
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
                    mayor = ascensor.pisoActual;    //Se sobreescribe mayor para comparar los demás      
                    resultado = ascensor;   //Tomamos ese ascensor
                }
            }
            else if (ascensor.direccion == Direccion.ABAJO && pisoSolicitud < ascensor.pisoActual) //Si el ascensor va para arriba y está abajo del piso origen
            {
                if (Math.Abs(ascensor.pisoActual - pisoSolicitud) < mayor)  //Si el valor abs. de la dif. de piso es la menor
                {
                    mayor = ascensor.pisoActual;    //Se sobreescribe mayor para comparar los demás      
                    resultado = ascensor;   //Tomamos ese ascensor
                }
            }
            else
            {
                continue; //Si ya pasó el piso, lo ignora
            }
        }
        if (resultado == null){
            var random = new Random();
            var index = random.Next(listaAscensores.Count);
            resultado = listaAscensores[index]; //Si se satura el buscarascensor, agarra un ascensor al azar
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
            if (colaPisos.Count() != 0)
            {
                int solPisoActual = colaPisos.Dequeue(); //Toma el primer elemento de la cola (FIFO)
                mutexColaSolicitudes.ReleaseMutex();
                if (solPisoActual == -1)
                { //-1 va a ser el código para decirle a controlador que voy a pasarle un tiempo de espera
                    int tEspera = stackEspera.Pop(); //Toma el tiempo de espera que se pasó a la pila de tiempos
                    Thread.Sleep(tEspera * 1000); //Lo multiplico por 1000 para que pueda pasarle en segundos, así es más facil
                }
                else
                {
                    Ascensor ascensor = buscarAscensor(solPisoActual); //Busco el ascensor que esté más cerca de la solicitud
                    if (ascensor != null)
                    {
                        if(!ascensor.listaParadas.Contains(solPisoActual)){
                            ascensor.listaParadas.Add(solPisoActual); //Agrega la solicitud a la lista de paradas del ascensor seleccionado
                        }
                    }
                }
            }
            else
            {
                mutexColaSolicitudes.ReleaseMutex();
            }
        }
    }
}