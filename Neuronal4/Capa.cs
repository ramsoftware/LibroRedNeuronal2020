/* Autor: Rafael Alberto Moreno Parra */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1 {
    class Capa {
        public List<Neurona> neuronas; //Las neuronas que tendrá la capa
        public List<double> salidas; //Almacena las salidas de cada neurona

        public Capa(Random azar, int totalNeuronas, int totalEntradas) {
            neuronas = new List<Neurona>();
            salidas = new List<double>();
            //Genera las neuronas
            for (int cont = 0; cont < totalNeuronas; cont++) {
                neuronas.Add(new Neurona(azar, totalEntradas));
                salidas.Add(0);
            }
        }

        //Calcula las salidas de cada neurona de la capa
        public void CalculaCapa(List<double> entradas) {
            for (int cont = 0; cont < neuronas.Count; cont++) {
                salidas[cont] = neuronas[cont].calculaSalida(entradas);
            }
        }

        //Actualiza los pesos y umbrales de las neuronas
        public void actualiza() {
            for (int cont = 0; cont < neuronas.Count; cont++) {
                neuronas[cont].actualiza();
            }
        }
    }
}
