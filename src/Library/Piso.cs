namespace Proyecto;

public class Piso{
    public PriorityQueue<Solicitud, int> colaSolPiso = new PriorityQueue<Solicitud,int>(); //Al hacer .enqueue hay que pasar los parametros: (Solicitud, Solicitud.prioridad)
    public int numPiso{get; private set;}
    public Piso(int numPiso){
        this.numPiso = numPiso;
    }
}