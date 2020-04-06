/* Autor: Rafael Alberto Moreno Parra */
using System;

namespace Neuronal {
    class Program {
        static void Main(string[] args) {
            //Único generador de números aleatorios
            Random azar = new Random();

            //Tabla de verdad AND
            int[,] entradas = { { 1, 1 }, { 1, 0 }, { 0, 1 }, { 0, 0 } };
            int[] salidas = { 1, 0, 0, 0 };

            //Inicializa los pesos al azar
            double P0 = azar.NextDouble();
            double P1 = azar.NextDouble();
            double U = azar.NextDouble();
            
            //Variable que mantiene el proceso activo de buscar los pesos
            bool proceso = true;

            //Cuenta el número de iteraciones
            int iteracion = 0;

            //Hasta que aprenda la tabla AND
            while (proceso) {
                iteracion++;

                //Optimista, en esta iteración se encuentra los pesos
                proceso = false;

                //Va por todas las reglas de la tabla AND
                for (int cont = 0; cont <= 3; cont++) {

                    //Calcula el valor de entrada a la función
                    double operacion = entradas[cont, 0] * P0 + entradas[cont, 1] * P1 + U;

                    //La función de activación
                    int salidaEntera;
                    if (operacion > 0.7)
                        salidaEntera = 1;
                    else 
                        salidaEntera = 0;

                    //Si la salida no coincide con lo esperado, cambia los pesos al azar
                    if (salidaEntera != salidas[cont]) { 
                        P0 = azar.NextDouble();
                        P1 = azar.NextDouble();
                        U = azar.NextDouble();
                        proceso = true; //Y sigue buscando
                    }
                }
            }

            //Muestra que el perceptrón simple aprendió.
            for (int cont = 0; cont <= 3; cont++) { 
                double operacion = entradas[cont, 0] * P0 + entradas[cont, 1] * P1 + U;
                
                //La función de activación
                int salidaEntera;
                if (operacion > 0.7)
                    salidaEntera = 1;
                else
                    salidaEntera = 0;

                //Imprime
                Console.WriteLine("Entradas: " + entradas[cont, 0].ToString() + " y " + entradas[cont, 1].ToString() + " = " +
                salidas[cont].ToString() + " perceptron: " + salidaEntera.ToString());
            }

            Console.WriteLine("Pesos encontrados P0= " + P0.ToString() + " P1= " + P1.ToString() + " U= " + U.ToString());
            Console.WriteLine("Iteraciones requeridas: " + iteracion.ToString());
            Console.ReadKey();
        }
    }
}
