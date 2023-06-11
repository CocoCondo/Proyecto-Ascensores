namespace Proyecto;
public class Solicitud{
    public String idSolicitud {get; private set;}
    public int pisoActual{get; private set;}
    public int pisoDestino{get; private set;}
    public int peso {get; private set;}
    /// <summary>
    /// Define la prioridad en la cola del piso a través de un número entero.
    /// Mayor el numero = mayor prioridad
    /// </summary>
    public int prioridad {get; private set;}
    public int tEspera {get; private set;}
    Controlador controlador = Controlador.GetInstance();

    public Solicitud(int pisoActual, int pisoDestino, int peso, int priodidad, String idSolicitud, int tEspera){
        this.pisoActual = pisoActual;
        this.pisoDestino = pisoDestino;
        this.peso = peso;
        this.prioridad = priodidad;
        this.idSolicitud = idSolicitud;
        this.tEspera = tEspera*1000; //Multiplicado por 1000 para pasar de ms a segundos
    }

    public void Run(){
        Thread.Sleep(this.tEspera);
        controlador.mutexColaSolicitudes.WaitOne();
        controlador.agregarSolicitudes(this.pisoActual, this);
        Console.WriteLine(Math.Floor(Controlador.sw.Elapsed.TotalSeconds)+ "seg>"+"SOLICITUD: "+this.idSolicitud+" desde el piso " + this.pisoActual);
        controlador.mutexColaSolicitudes.ReleaseMutex();
    }

    /* 
        Hay que agregar envejecimiento. La idea podría ser poner un stopwatch que se active al "agregarSolicitud" y que haya un método que el ascensor
        al levantar al pasajero desactive.
        Se van activando a medida que suben o bajan de los ascensores para ir grabando los tiempos.
        Cada solicitud tenga un mutex propio en el Run para controlar los stopwatch.
        Al finalizar el viaje, la clase solicitud llama a metodo <<IMPRIMIR EN CSV>> para tener en un archivo compatible con excel
        los tiempos de los viajes
            Tiempo de espera al ascensor
            Tiempo de viaje dentro del ascensor
    */

    /*Hay que ver lo de la interfaz gráfica*/
}