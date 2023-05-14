namespace Proyecto;
public class Solicitud{
    private int pisoActual{get;}
    private int pisoDestino{get;}
    private int peso {get;}
    /// <summary>
    /// Calcular en base al origen y al destino
    /// </summary>
    private Direccion direccion{get;}

    /// <summary>
    /// Define la prioridad en la cola del piso a través de un número entero.
    /// Mayor el numero = mayor prioridad
    /// </summary>
    private int prioridad{get;}

    public Solicitud(int pisoActual, int pisoDestino, int peso){
        this.pisoActual = pisoActual;
        this.pisoDestino = pisoDestino;
        this.peso = peso;
        this.direccion = this.getDireccion(pisoActual,pisoDestino);
    }

/// <summary>
/// Devuelve si va para arriba o para abajo en base al piso origen y el piso destino
/// </summary>
/// <param name="origen">Es el pisoActual de la solicitud, es decir, el origen.</param>
/// <param name="destino">Es el piso al que se dirige la solicitud.</param>
/// <returns>ARRIBA o ABAJO</returns>
    private Direccion getDireccion(int origen, int destino){
        if(origen<destino){
            return Direccion.ARRIBA;
        }
        else{
            return Direccion.ABAJO;
        }
    }

   /* public Elevator submitRequest(){
        return ElevatorController.getInstance().selectElevator(this);*/
}