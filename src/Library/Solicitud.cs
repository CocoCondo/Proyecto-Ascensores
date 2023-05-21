namespace Proyecto;
public class Solicitud{
    public static int idSolGlobal = 0;
    public int idSolicitud {get; private set;}
    public int pisoActual{get; private set;}
    public int pisoDestino{get; private set;}
    public int peso {get; private set;}
    public int prioridad {get; private set;}

    /// <summary>
    /// Define la prioridad en la cola del piso a través de un número entero.
    /// Mayor el numero = mayor prioridad
    /// </summary>

    public Solicitud(int pisoActual, int pisoDestino, int peso, int priodidad){
        this.pisoActual = pisoActual;
        this.pisoDestino = pisoDestino;
        this.peso = peso;
        this.prioridad = priodidad;
        this.idSolicitud = idSolGlobal;
        idSolGlobal++;
    }
}