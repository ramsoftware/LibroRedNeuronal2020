/* Autor: Rafael Alberto Moreno Parra */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1 {
    class Neurona {
        public List<double> pesos; //Los pesos para cada entrada
        public List<double> nuevospesos; //Nuevos pesos dados por el algoritmo de "backpropagation"
        public double umbral; //El peso del umbral
        public double nuevoumbral; //Nuevo umbral dado por el algoritmo de "backpropagation"

        //Inicializa los pesos y umbral con un valor al azar
        public Neurona(Random azar, int totalEntradas) {
            pesos = new List<double>();
            nuevospesos = new List<double>();
            for (int cont = 0; cont < totalEntradas; cont++) {
                pesos.Add(azar.NextDouble());
                nuevospesos.Add(0);
            }
            umbral = azar.NextDouble();
            nuevoumbral = 0;
        }

        //Calcula la salida de la neurona dependiendo de las entradas
        public double calculaSalida(List<double> entradas) {
            double valor = 0;
            for (int cont = 0; cont < pesos.Count; cont++) {
                valor += entradas[cont] * pesos[cont];
            }
            valor += umbral;
            return 1 / (1 + Math.Exp(-valor));
        }

        //Reemplaza viejos pesos por nuevos
        public void actualiza() {
            for (int cont = 0; cont < pesos.Count; cont++) {
                pesos[cont] = nuevospesos[cont];
            }
            umbral = nuevoumbral;
        }
    }
}
