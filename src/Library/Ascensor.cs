namespace Proyecto;
using System;
using System.Text;

public class Ascensor
{
    /// <summary>
    /// Peso máximo soportado por el ascensor
    /// </summary>
    private int PESO_MAXIMO = 400;
    Controlador controlador = Controlador.GetInstance();
    private int MAX_PISOS = Controlador.numPisos;
    public int id { get; }
    public int pesoAscensor { get; private set; }
    public int pisoActual { get; private set; }
    public bool operando { get; private set; }
    public Direccion direccion { get; private set; }
    public List<int> listaParadas { get; private set; }
    public LinkedList<Solicitud>[] listaSol = new LinkedList<Solicitud>[Controlador.numPisos];
    private static Mutex mutexSolicitudPiso = new Mutex();

    public Ascensor(int id)
    {
        this.id = id;
        this.pesoAscensor = 0;
        this.pisoActual = 0;
        this.operando = false;
        this.direccion = Direccion.STOP;
        this.listaParadas = new List<int>();
        for (int i = 0; i<listaSol.Length;i++){
            listaSol[i] = new LinkedList<Solicitud>();
        }
    }

    public int getPeso()
    {
        int peso = 0;
        for (int i = 0; i < listaSol.Length; i++)
        {
            if (listaSol[i] != null)
            {
                foreach (Solicitud s in listaSol[i])
                {
                    peso += s.peso;
                }
            }
        }
        return peso;
    }

    public void mover(int pisoDestino)
    {
        if (this.pisoActual != pisoDestino)
        {
            int pisosRestantes = Math.Abs(this.pisoActual - pisoDestino);
            for (int i = 0; i < pisosRestantes; i++)
            {
                Console.WriteLine("MOVE: Ascensor " + this.id + " viaja desde el piso: " + pisoActual
                        + " al piso: " + pisoDestino);
                if (pisoActual - pisoDestino < 0)
                {
                    pisoActual++;
                }
                else
                {
                    pisoActual--;
                }
                Thread.Sleep(1000);
            }
        }
        StringBuilder solBajas = new StringBuilder();
        solBajas.Append("FIN: Elevator " + id + " llegó al piso: " + pisoActual)
        .Append("- Se bajan:");
        if (this.listaSol[pisoActual] != null)
        {
            foreach (Solicitud s in this.listaSol[pisoActual])
            {
                solBajas.Append(s.idSolicitud + ",");
            }
            this.listaSol[pisoActual].Clear();
        }
        Console.WriteLine(solBajas);
    }

    //ACA VA A CORRER TODO EL PROGRAMA DEL ASCENSOR
    public void Run()
    {
        Console.WriteLine("Ascensor: " + this.id + " iniciado");
        while (true)
        {
            if (listaParadas.Count() != 0)
            {
                if (this.direccion == Direccion.ABAJO)
                {
                    listaParadas.Sort();
                    listaParadas.Reverse();
                    this.mover(listaParadas.First());
                    listaParadas.RemoveAt(0);
                }
                else //if (this.direccion == Direccion.ARRIBA)
                {
                    listaParadas.Sort();
                    this.mover(listaParadas.First());
                    listaParadas.RemoveAt(0);
                }

                // proteger lectura de pisosEdificio?
                mutexSolicitudPiso.WaitOne();
                Piso pisoActual = controlador.pisosEdificio[this.pisoActual]; //Para poder fijarme si hay sols pendientes en el piso:
                mutexSolicitudPiso.ReleaseMutex();
                while (pisoActual.colaSolPiso.Count != 0)
                {
                    Solicitud solicitud = pisoActual.colaSolPiso.Dequeue(); //Hago que un pasajero entre al ascensor!
                    if (getPeso() + solicitud.peso < PESO_MAXIMO)
                    {
                        if (solicitud.pisoDestino > MAX_PISOS)
                        {
                            Console.WriteLine("Solicitud al piso:" + solicitud.pisoDestino + " incorrecta. Abortando...");
                            continue;
                        }
                        Console.WriteLine("SUBE: Pasajero " + solicitud.idSolicitud + " Prioridad: " + solicitud.prioridad + "- O:" + solicitud.pisoActual + "/D:" + solicitud.pisoDestino);
                        if (!this.listaParadas.Contains(solicitud.pisoDestino))
                        {
                            this.listaParadas.Add(solicitud.pisoDestino); //Agregado el destino a la cola
                        }
                        this.listaSol[solicitud.pisoDestino].AddLast(solicitud);
                        if (this.listaParadas.First() < this.pisoActual)
                        { //Si el primer destino es para ABAJO:
                            this.direccion = Direccion.ABAJO;
                        }
                        else if (this.listaParadas.First() > this.pisoActual)
                        { //Si el primer destino es para ARRIBA:{
                            this.direccion = Direccion.ARRIBA;
                        }
                        else
                        {
                            this.direccion = Direccion.STOP;
                        }
                    }
                    else //Si el ascensor está lleno....
                    {
                        Console.WriteLine("El pasajero " + solicitud.idSolicitud + " se bajó del ascensor al superarse el peso límite.");
                        pisoActual.colaSolPiso.Enqueue(solicitud, solicitud.prioridad); //Se devuelve la solicitud a la cola del piso
                        mutexSolicitudPiso.WaitOne();
                        controlador.colaSolPisos.Enqueue(pisoActual.numPiso); //Como quedan llamadas pendientes, le dice al controlador que mande otro ascensor
                        mutexSolicitudPiso.ReleaseMutex();
                        break; //Sale del bucle porque el ascensor está lleno, por lo tanto no puede aceptar más solicitudes
                    }
                }

            }
            else
            {
                this.direccion = Direccion.STOP; //Si la lista de solicitudes está vacia, el ascensor queda en modo STOP
            }
        }
    }

}

