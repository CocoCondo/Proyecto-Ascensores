using System.Diagnostics;
using System.Text;

namespace Proyecto;
public class Solicitud
{
    public String idSolicitud { get; private set; }
    public int pisoActual { get; private set; }
    public int pisoDestino { get; private set; }
    public int peso { get; private set; }
    /// <summary>
    /// Define la prioridad en la cola del piso a través de un número entero.
    /// Mayor el numero = mayor prioridad
    /// </summary>
    public int prioridad { get; set; }
    public int tEspera { get; private set; }
    Controlador controlador = Controlador.GetInstance();
    /// <summary>
    /// Mutex para que la solicitud quede esperando a terminar el viaje
    /// El ascensor le da la señal a la solicitud de que el viaje terminó
    /// </summary>
    /// <returns></returns>
    public Mutex mutexTerminaViaje = new Mutex();
    public Semaphore semTerminaViaje = new Semaphore(0,1);


    //========================================Definiciones de recolección de datos============================
    public double tTotalEspera { get; set; } = 0.0;
    public double tTotalViaje { get; set; } = 0.0;
    public double tSolicitado { get; private set; } = 0.0;

    //========================================FIN Def de recolección de datos=================================

    public Solicitud(int pisoActual, int pisoDestino, int peso, int priodidad, String idSolicitud, int tEspera)
    {
        this.pisoActual = pisoActual;
        this.pisoDestino = pisoDestino;
        this.peso = peso;
        this.prioridad = priodidad;
        this.idSolicitud = idSolicitud;
        this.tEspera = tEspera * 1000; //Multiplicado por 1000 para pasar de ms a segundos
    }

    public void Run()
    {
        Thread.Sleep(this.tEspera);
        controlador.mutexColaSolicitudes.WaitOne();
        controlador.agregarSolicitudes(this.pisoActual, this);
        Console.WriteLine(Math.Floor(Controlador.sw.Elapsed.TotalSeconds) + "seg>" + "SOLICITUD: " + this.idSolicitud + " desde el piso " + this.pisoActual);
        this.tSolicitado = Math.Floor(Controlador.sw.Elapsed.TotalSeconds);
        controlador.mutexColaSolicitudes.ReleaseMutex();
        
        semTerminaViaje.WaitOne();

        this.tTotalViaje = tTotalViaje-tTotalEspera;
        this.tTotalEspera = tTotalEspera-tSolicitado;
        
        controlador.mutexCSV.WaitOne(); //Mutex para modificar el CSV
        this.imprimirCSV(tTotalEspera, tTotalViaje); //Agrega al archivo CSV los datos de la solicitud
        controlador.mutexCSV.ReleaseMutex();
    }

    private void imprimirCSV(double totalEspera, double totalViaje)
    {
        StringBuilder datosAImprimir = new StringBuilder();
        datosAImprimir.Append(String.Format("{0},{1},{2},{3},{4},{5},{6}", this.idSolicitud,totalEspera,totalViaje,this.peso,this.pisoActual,this.pisoDestino,this.prioridad));
        var archivo = @"resultados.csv";
        File.AppendAllText(archivo,datosAImprimir.ToString()+Environment.NewLine);
    }
    /*Hay que ver lo de la interfaz gráfica*/
}