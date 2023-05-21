namespace Proyecto;
using System;
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

    //ACA VA A CORRER TODO EL PROGRAMA DEL ASCENSOR
    public void AscensorEjecutando(){
        Console.WriteLine("Ascensor iniciado");
    }

    // HELPERS
    private void CalcularPesoAscensor(){
        Console.WriteLine("Calculando peso del ascensor");
    }
    private void Mover(int piso){
        Console.WriteLine("Moviendose al piso x");        
    }
    private void AgregarParada(int piso){
        Console.WriteLine("Agregando parada");
    }

}

