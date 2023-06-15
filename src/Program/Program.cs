using Proyecto;
Simulacion sim = new Simulacion();
sim.procesarSolicitudes("solprueba.txt");
Controlador controlador = Controlador.GetInstance();
controlador.run();