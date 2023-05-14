namespace Proyecto;
public class Ascensor
{
    private int id{get;}
    private int pesoAscensor{get;set;}
    private int pisoActual{get;set;}
    private bool operando{get;set;}
    private List<int> paradas;
    public Ascensor(int id){
        this.id = id;
        this.pesoAscensor = 0;
        this.pisoActual = 0;
        this.operando = false;
        this.paradas = new List<int>();
    }

    public int calcularPesoAscensor(){
        Console.WriteLine("Calculando peso del ascensor");
        return 0;
    }
    public void mover(int piso){
        Console.WriteLine("Moviendose al piso x");        
    }

    public void agregarParada(int piso){
        Console.WriteLine("Agregando parada");
    }

}

