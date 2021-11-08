/* Autor: Rafael Alberto Moreno Parra */
using System;
using System.Collections.Generic;

namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {
            //Lee los datos de un archivo plano
            int MaximosRegistros = 2000;
            double[] entrada = new double[MaximosRegistros + 1];
            double[] salidas = new double[MaximosRegistros + 1];

            //Qué valores de entrada se usarán para entrenamiento y cuáles para validación de "overfitting"
            bool[] entraValida = new bool[MaximosRegistros + 1];

            const string urlArchivo = "datos.tendencia";
            int ConjuntoEntradas = LeeDatosArchivo(urlArchivo, entrada, salidas);

            //Normaliza los valores entre 0 y 1 que es lo que requiere el perceptrón
            double minimoX = entrada[0], maximoX = entrada[0];
            double minimoY = salidas[0], maximoY = salidas[0];
            for (int cont = 0; cont < ConjuntoEntradas; cont++) {
                if (entrada[cont] > maximoX) maximoX = entrada[cont];
                if (salidas[cont] > maximoY) maximoY = salidas[cont];
                if (entrada[cont] < minimoX) minimoX = entrada[cont];
                if (salidas[cont] < minimoY) minimoY = salidas[cont];
            }

            for (int cont = 0; cont < ConjuntoEntradas; cont++) {
                entrada[cont] = (entrada[cont] - minimoX) / (maximoX - minimoX);
                salidas[cont] = (salidas[cont] - minimoY) / (maximoY - minimoY);
                entraValida[cont] = false;
            }

            //Marca al azar que valores usará para entrenar y cuáles para validar "overfitting"
            int porcentaje = 20;
            int totalValida = ConjuntoEntradas * porcentaje / 100;
            int cuentaValida = 0;
            Random aleatorio = new Random(100);
            do {
                int pos = aleatorio.Next(ConjuntoEntradas);
                if (entraValida[pos] == false) {
                    entraValida[pos] = true;
                    cuentaValida++;
                }
            } while (cuentaValida < totalValida);


            int numEntradas = 1; //Número de entradas
            int capa0 = 5; //Total neuronas en la capa 0
            int capa1 = 5; //Total neuronas en la capa 1
            int capa2 = 1; //Total neuronas en la capa 2
            
            Perceptron perceptron = new Perceptron(numEntradas, capa0, capa1, capa2);

            //Estas serán las entradas externas al perceptrón
            List<double> entradas = new List<double>();
            entradas.Add(0);

            //Estas serán las salidas esperadas externas al perceptrón
            List<double> salidaEsperada = new List<double>();
            salidaEsperada.Add(0);

            //Ciclo que entrena la red neuronal
            double anteriorAjuste = Double.MaxValue; //Para validar el "overfitting"
            int ciclos = 0;
            while (true) {
                ciclos++;
                //if (ciclos >= 80000) break;

                //Por cada ciclo, se entrena el perceptrón con todos los valores
                for (int conjunto = 0; conjunto < ConjuntoEntradas; conjunto++) {
                    //Si el valor es para validar no lo usa para entrenar
                    if (entraValida[conjunto] == true) continue;

                    //Entradas y salidas esperadas
                    entradas[0] = entrada[conjunto];
                    salidaEsperada[0] = salidas[conjunto];

                    //Primero calcula la salida del perceptrón con esas entradas
                    perceptron.calculaSalida(entradas);

                    //Luego entrena el perceptrón para ajustar los pesos y umbrales
                    perceptron.Entrena(entradas, salidaEsperada);
                }

                //Valida el "overfitting"
                double ajusteValidando = 0;
                for (int conjunto = 0; conjunto < ConjuntoEntradas; conjunto++) {
                    //Si el valor es para entrenar no lo usa para validar
                    if (entraValida[conjunto] == false) continue;

                    //Entradas y salidas esperadas
                    entradas[0] = entrada[conjunto];
                    salidaEsperada[0] = salidas[conjunto];
                    ajusteValidando += perceptron.CalculaAjuste(entradas, salidaEsperada);
                }

                //Console.WriteLine("Ciclo: " + ciclos.ToString() + " Ajuste: " + ajusteValidando.ToString());
                
                //Si el nuevo entrenamiento aleja la precisión entonces termina el entrenamiento
                if (ajusteValidando > anteriorAjuste) {
                    Console.WriteLine("Posible Overfitting en: " + ciclos.ToString());
                    break;
                }
                else
                    anteriorAjuste = ajusteValidando;
            }

            //Muestra el resultado del entrenamiento
            for (int conjunto = 0; conjunto < ConjuntoEntradas; conjunto++) {
                if (entraValida[conjunto] == false) continue;

                //Entradas y salidas esperadas
                entradas[0] = entrada[conjunto];
                salidaEsperada[0] = salidas[conjunto];

                //Calcula la salida del perceptrón con esas entradas
                perceptron.calculaSalida(entradas);

                //Muestra la salida
                perceptron.SalidaPerceptron(entradas, salidaEsperada);
            }

            Console.WriteLine("Finaliza");
            Console.ReadKey();
        }

        private static int LeeDatosArchivo(string urlArchivo, double[] entrada, double[] salida) {
            var archivo = new System.IO.StreamReader(urlArchivo);
            string leelinea;

            int limValores = 0;
            while ((leelinea = archivo.ReadLine()) != null) {
                double valX = TraerNumeroCadena(leelinea, ';', 1);
                double valY = TraerNumeroCadena(leelinea, ';', 2);
                entrada[limValores] = valX;
                salida[limValores] = valY;
                limValores++;
            }
            archivo.Close();
            return limValores;
        }

        //Dada una cadena con separaciones por delimitador, trae determinado ítem
        private static double TraerNumeroCadena(string linea, char delimitador, int numeroToken) {
            string numero = "";
            int numTrae = 0;
            foreach (char t in linea) {
                if (t != delimitador)
                    numero += t;
                else {
                    numTrae += 1;
                    if (numTrae == numeroToken) {
                        numero = numero.Trim();
                        if (numero == "") return 0;
                        return Convert.ToDouble(numero);
                    }
                    numero = "";
                }
            }
            numero = numero.Trim();
            if (numero == "") return 0;
            return Convert.ToDouble(numero);
        }
    }
}
