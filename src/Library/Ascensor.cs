﻿namespace Proyecto;
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
                Console.WriteLine(Math.Floor(Controlador.sw.Elapsed.TotalSeconds)+"seg>ASC. "+ this.id +" > MOVER: desde piso " +pisoActual+ " al piso " + pisoDestino);
                if (pisoActual - pisoDestino < 0)
                {
                    pisoActual++;
                }
                else
                {
                    pisoActual--;
                }
                Thread.Sleep(1000); //El ascensor tarda 1 seg entre piso y piso
            }
        }
        StringBuilder solBajas = new StringBuilder();
        solBajas.Append("ASC. "+id+" > FIN: llegó al piso " + pisoActual);
        if (this.listaSol[pisoActual] != null)
        {
            solBajas.Append(" > BAJA: ");
            foreach (Solicitud s in this.listaSol[pisoActual])
            {
                solBajas.Append(s.idSolicitud + ",");
                s.tTotalViaje = Math.Floor(Controlador.sw.Elapsed.TotalSeconds); //Capturo el tiempo de espera de la solicitud a subirse al ascensor
                s.semTerminaViaje.Release();//Libero el semaforo del temporizador de la solicitud para capturar el tiempo que demoró en el viaje.
                //Thread.Sleep(1000); //Si el pasajero baja, tarda un segundo
            }
            this.listaSol[pisoActual].Clear();
        }
        Console.WriteLine(Math.Floor(Controlador.sw.Elapsed.TotalSeconds)+"seg>"+solBajas);
    }

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
                else
                {
                    listaParadas.Sort();
                    this.mover(listaParadas.First());
                    listaParadas.RemoveAt(0);
                }

                controlador.mutexColaSolicitudes.WaitOne();
                Piso pisoActual = controlador.pisosEdificio[this.pisoActual]; //Para poder fijarme si hay sols pendientes en el piso:
                controlador.mutexColaSolicitudes.ReleaseMutex();
                while (pisoActual.colaSolPiso.Count != 0)
                {
                    Solicitud solicitud = pisoActual.colaSolPiso.Dequeue(); //Hago que un pasajero entre al ascensor!
                    if (getPeso() + solicitud.peso < PESO_MAXIMO)
                    {
                        if (solicitud.pisoDestino > MAX_PISOS)
                        {
                            Console.WriteLine(Math.Floor(Controlador.sw.Elapsed.TotalSeconds)+"seg>Solicitud al Piso " + solicitud.pisoDestino + " incorrecta. Abortando...");
                            continue;
                        }
                        Console.WriteLine(Math.Floor(Controlador.sw.Elapsed.TotalSeconds)+"seg>ASC. "+this.id+" > SUBE: En el Piso " + solicitud.pisoActual +", el Pasajero " + solicitud.idSolicitud + " con Prioridad " + solicitud.prioridad + " y Destino " + solicitud.pisoDestino);
                        Thread.Sleep(1000); //1 seg para que suba un pasajero
                        solicitud.tTotalEspera = Math.Floor(Controlador.sw.Elapsed.TotalSeconds); //Capturo el tiempo de espera de la solicitud a subirse al ascensor
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
                        Console.WriteLine(Math.Floor(Controlador.sw.Elapsed.TotalSeconds)+"seg> ASC. "+this.id+" > PESO: " + solicitud.idSolicitud + " se bajó del ascensor al superarse el peso límite.");
                        Thread.Sleep(1000); //Si el pasajero baja, tarda un segundo
                        controlador.mutexColaSolicitudes.WaitOne();
                        solicitud.prioridad = solicitud.prioridad - 1;
                        pisoActual.colaSolPiso.Enqueue(solicitud, solicitud.prioridad); //Se devuelve la solicitud a la cola del piso CON MAYOR PRIORIDAD (ENVEJECIMIENTO)
                        controlador.colaPisos.Enqueue(pisoActual.numPiso); //Como quedan llamadas pendientes, le dice al controlador que mande otro ascensor
                        Console.WriteLine(Math.Floor(Controlador.sw.Elapsed.TotalSeconds)+"seg> NUEVA PRIORIDAD DE " + solicitud.idSolicitud + " " + solicitud.prioridad);
                        controlador.mutexColaSolicitudes.ReleaseMutex();
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

