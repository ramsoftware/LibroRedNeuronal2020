/* Autor: Rafael Alberto Moreno Parra */
using System;
using System.Collections.Generic;

namespace AplicacionConsola {
	class Program {
		static void Main() {
			//Lee los datos de un archivo plano
			int MaximosRegistros = 2000;
			double[] entrada = new double[MaximosRegistros + 1];
			double[] salidas = new double[MaximosRegistros + 1];
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
			}

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
			int totalCiclos = 10000; //Ciclos de entrenamiento
			for (int ciclo = 1; ciclo <= totalCiclos; ciclo++) {

				//Por cada ciclo, se entrena el perceptrón con todos los valores
				for (int conjunto = 0; conjunto < ConjuntoEntradas; conjunto++) {
					//Entradas y salidas esperadas
					entradas[0] = entrada[conjunto];
					salidaEsperada[0] = salidas[conjunto];

					//Primero calcula la salida del perceptrón con esas entradas
					perceptron.calculaSalida(entradas);

					//Luego entrena el perceptrón para ajustar los pesos y umbrales
					perceptron.Entrena(entradas, salidaEsperada);
				}
			}

			Console.WriteLine("Entrada normalizada | Salida esperada normalizada | Salida perceptrón normalizada");
			for (int conjunto = 0; conjunto < ConjuntoEntradas; conjunto++) {
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

		//Lee los datos del archivo plano
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

	class Perceptron {
		public List<Capa> capas;

		//Imprime los datos de las diferentes capas
		public void SalidaPerceptron(List<double> entradas, List<double> salidaesperada) {
			for (int cont = 0; cont < entradas.Count; cont++) {
				Console.Write(entradas[cont].ToString() + " ");
			}
			Console.Write(" | ");
			for (int cont = 0; cont < salidaesperada.Count; cont++) {
				Console.Write(salidaesperada[cont].ToString() + " ");
			}
			Console.Write(" | ");
			for (int cont = 0; cont < capas[2].salidas.Count; cont++) {
				Console.Write(capas[2].salidas[cont].ToString());
			}
			Console.WriteLine(" ");
		}

		//Crea las diversas capas
		public Perceptron(int numEntrada, int capa0, int capa1, int capa2) {
			Random azar = new Random();
			capas = new List<Capa>();
			capas.Add(new Capa(azar, capa0, numEntrada)); //Crea la capa 0
			capas.Add(new Capa(azar, capa1, capa0)); //Crea la capa 1
			capas.Add(new Capa(azar, capa2, capa1)); //Crea la capa 2
		}

		//Dada las entradas al perceptrón, se calcula la salida de cada capa.
		//Con eso se sabrá que salidas se obtienen con los pesos y umbrales actuales.
		//Esas salidas son requeridas para el algoritmo de entrenamiento.
		public void calculaSalida(List<double> entradas) {
			capas[0].CalculaCapa(entradas);
			capas[1].CalculaCapa(capas[0].salidas);
			capas[2].CalculaCapa(capas[1].salidas);
		}

		//Con las salidas previamente calculadas con unas determinadas entradas
		//se ejecuta el algoritmo de entrenamiento "Backpropagation"  
		public void Entrena(List<double> entradas, List<double> salidaEsperada) {
			int capa0 = capas[0].neuronas.Count;
			int capa1 = capas[1].neuronas.Count;
			int capa2 = capas[2].neuronas.Count;

			//Factor de aprendizaje
			double alpha = 0.4;

			//Procesa pesos capa 2
			for (int j = 0; j < capa1; j++) //Va de neurona en neurona de la capa 1
				for (int i = 0; i < capa2; i++) { //Va de neurona en neurona de la capa de salida (capa 2)
					double Yi = capas[2].salidas[i]; //Salida de la neurona de la capa de salida
					double Si = salidaEsperada[i]; //Salida esperada
					double a1j = capas[1].salidas[j]; //Salida de la capa 1
					double dE2 = a1j * (Yi - Si) * Yi * (1 - Yi); //La fórmula del error
					capas[2].neuronas[i].nuevospesos[j] = capas[2].neuronas[i].pesos[j] - alpha * dE2; //Ajusta el nuevo peso
				}

			//Procesa pesos capa 1
			for (int j = 0; j < capa0; j++) //Va de neurona en neurona de la capa 0
				for (int k = 0; k < capa1; k++) { //Va de neurona en neurona de la capa 1
					double acum = 0;
					for (int i = 0; i < capa2; i++) { //Va de neurona en neurona de la capa 2
						double Yi = capas[2].salidas[i]; //Salida de la capa 2
						double Si = salidaEsperada[i]; //Salida esperada
						double W2ki = capas[2].neuronas[i].pesos[k];
						acum += W2ki * (Yi - Si) * Yi * (1 - Yi); //Sumatoria
					}
					double a0j = capas[0].salidas[j];
					double a1k = capas[1].salidas[k];
					double dE1 = a0j * a1k * (1 - a1k) * acum;
					capas[1].neuronas[k].nuevospesos[j] = capas[1].neuronas[k].pesos[j] - alpha * dE1;
				}

			//Procesa pesos capa 0
			for (int j = 0; j < entradas.Count; j++) //Va de entrada en entrada
				for (int k = 0; k < capa0; k++) { //Va de neurona en neurona de la capa 0
					double acumular = 0;
					for (int p = 0; p < capa1; p++) { //Va de neurona en neurona de la capa 1
						double acum = 0;
						for (int i = 0; i < capa2; i++) { //Va de neurona en neurona de la capa 2
							double Yi = capas[2].salidas[i];
							double Si = salidaEsperada[i]; //Salida esperada
							double W2pi = capas[2].neuronas[i].pesos[p];
							acum += W2pi * (Yi - Si) * Yi * (1 - Yi); //Sumatoria interna
						}
						double W1kp = capas[1].neuronas[p].pesos[k];
						double a1p = capas[1].salidas[p];
						acumular += W1kp * a1p * (1 - a1p) * acum; //Sumatoria externa
					}
					double xj = entradas[j];
					double a0k = capas[0].salidas[k];
					double dE0 = xj * a0k * (1 - a0k) * acumular;
					double W0jk = capas[0].neuronas[k].pesos[j];
					capas[0].neuronas[k].nuevospesos[j] = W0jk - alpha * dE0;
				}


			//Procesa umbrales capa 2
			for (int i = 0; i < capa2; i++) { //Va de neurona en neurona de la capa de salida (capa 2)
				double Yi = capas[2].salidas[i]; //Salida de la neurona de la capa de salida
				double Si = salidaEsperada[i]; //Salida esperada
				double dE2 = (Yi - Si) * Yi * (1 - Yi);
				capas[2].neuronas[i].nuevoumbral = capas[2].neuronas[i].umbral - alpha * dE2;
			}

			//Procesa umbrales capa 1
			for (int k = 0; k < capa1; k++) { //Va de neurona en neurona de la capa 1
				double acum = 0;
				for (int i = 0; i < capa2; i++) { //Va de neurona en neurona de la capa 2
					double Yi = capas[2].salidas[i]; //Salida de la capa 2
					double Si = salidaEsperada[i];
					double W2ki = capas[2].neuronas[i].pesos[k];
					acum += W2ki * (Yi - Si) * Yi * (1 - Yi);
				}
				double a1k = capas[1].salidas[k];
				double dE1 = a1k * (1 - a1k) * acum;
				capas[1].neuronas[k].nuevoumbral = capas[1].neuronas[k].umbral - alpha * dE1;
			}

			//Procesa umbrales capa 0
			for (int k = 0; k < capa0; k++) { //Va de neurona en neurona de la capa 0
				double acumular = 0;
				for (int p = 0; p < capa1; p++) { //Va de neurona en neurona de la capa 1
					double acum = 0;
					for (int i = 0; i < capa2; i++) { //Va de neurona en neurona de la capa 2
						double Yi = capas[2].salidas[i];
						double Si = salidaEsperada[i];
						double W2pi = capas[2].neuronas[i].pesos[p];
						acum += W2pi * (Yi - Si) * Yi * (1 - Yi);
					}
					double W1kp = capas[1].neuronas[p].pesos[k];
					double a1p = capas[1].salidas[p];
					acumular += W1kp * a1p * (1 - a1p) * acum;
				}
				double a0k = capas[0].salidas[k];
				double dE0 = a0k * (1 - a0k) * acumular;
				capas[0].neuronas[k].nuevoumbral = capas[0].neuronas[k].umbral - alpha * dE0;
			}

			//Actualiza los pesos
			capas[0].actualiza();
			capas[1].actualiza();
			capas[2].actualiza();
		}
	}

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