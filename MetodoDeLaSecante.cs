using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

class MetodoDeLaSecante {
    static double EvaluarFuncion(string expresion, double x) {
        try {
            expresion=ConvertirExponentes(expresion);
            expresion=expresion.Replace("x", x.ToString(CultureInfo.InvariantCulture));

            using (DataTable dt = new DataTable()) {
                dt.Columns.Add("expr", typeof(double), expresion);
                DataRow row = dt.NewRow();
                dt.Rows.Add(row);
                return (double) row["expr"];
            }
        } catch (Exception ex) {
            throw new ArgumentException($"Error al evaluar: {expresion}. Detalle: {ex.Message}");
        }
    }

    static string ConvertirExponentes(string expresion) {
        var regex = new Regex(@"([\d\.x]+)\^(\d+)");
        Match match = regex.Match(expresion);

        while (match.Success) {
            string baseVal = match.Groups[1].Value;
            int exp = int.Parse(match.Groups[2].Value);

            if (exp>10)
                throw new ArgumentException("El exponente no puede ser mayor a 10");

            string replacement = "";
            for (int i = 0; i<exp; i++) {
                if (i>0)
                    replacement+="*";
                replacement+=baseVal;
            }

            expresion=expresion.Replace(match.Value, $"({replacement})");
            match=regex.Match(expresion);
        }

        return expresion;
    }

    static double MetodoSecante(string funcion, double x0, double x1, double tolerancia, int maxIteraciones) {
        double x_n = x0;    // x(n)
        double x_n1 = x1;   // x(n+1)
        double error = double.MaxValue;
        int iteracion = 0;

        Console.WriteLine("\nProceso de iteraciones:");
        Console.WriteLine("n\tx(n)\t\tx(n+1)\t\tx(n+2)\t\tf(x(n))\t\tf(x(n+1))\tError");

        double f_n = EvaluarFuncion(funcion, x_n);
        double f_n1 = EvaluarFuncion(funcion, x_n1);

        while (error>tolerancia&&iteracion<maxIteraciones) {
            if (Math.Abs(f_n1-f_n)<double.Epsilon) {
                Console.WriteLine("División por cero en el método de la secante.");
                return double.NaN;
            }

            // Calcular nueva aproximación x(n+2)
            double x_n2 = x_n1-f_n1*(x_n1-x_n)/(f_n1-f_n);
            error=Math.Abs(x_n2-x_n1);

            // Mostrar resultados de esta iteración
            Console.WriteLine($"{iteracion}\t{x_n:F8}\t{x_n1:F8}\t{x_n2:F8}\t{f_n:F8}\t{f_n1:F8}\t{error:F8}");

            // Preparar siguiente iteración
            x_n=x_n1;
            x_n1=x_n2;
            f_n=f_n1;
            f_n1=EvaluarFuncion(funcion, x_n1);
            iteracion++;
        }

        if (iteracion==maxIteraciones) {
            Console.WriteLine("Se alcanzó el número máximo de iteraciones sin converger.");
        }

        return x_n1;
    }

    static void Main(string[] args) {
        bool continuar = true;

        Console.WriteLine("Calculadora del Método de la Secante");
        Console.WriteLine("-----------------------------------");

        while (continuar) {
            try {
                Console.WriteLine("\nIngrese la función a evaluar (use 'x' como variable):");
                Console.WriteLine("Ejemplos válidos:");
                Console.WriteLine("x^2 - 4");
                Console.WriteLine("3*x^3 - 2*x^2 + x - 5");
                Console.Write("\nFunción: ");
                string funcion = Console.ReadLine();

                Console.Write("\nIngrese el primer valor inicial (x0): ");
                double x0 = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                Console.Write("Ingrese el segundo valor inicial (x1): ");
                double x1 = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                Console.Write("\nIngrese la tolerancia (ej. 0.0001): ");
                double tolerancia = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

                Console.Write("Ingrese el número máximo de iteraciones: ");
                int maxIteraciones = int.Parse(Console.ReadLine());

                double raiz = MetodoSecante(funcion, x0, x1, tolerancia, maxIteraciones);

                Console.WriteLine("\nResultado final:");
                if (!double.IsNaN(raiz)) {
                    Console.WriteLine($"Raíz aproximada: {raiz:F8}");
                    Console.WriteLine($"Valor de f({raiz:F8}): {EvaluarFuncion(funcion, raiz):F8}");
                }

                // Preguntar si desea continuar
                Console.Write("\n¿Desea realizar otra operación? (S/N): ");
                string respuesta = Console.ReadLine().Trim().ToUpper();

                continuar=(respuesta=="S");
            } catch (Exception ex) {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.Write("¿Desea intentar nuevamente? (S/N): ");
                string respuesta = Console.ReadLine().Trim().ToUpper();

                continuar=(respuesta=="S");
            }
        }

        Console.WriteLine("\n¡Gracias por usar nuestra calculadora del método de la secante!");
        Console.WriteLine("Presione cualquier tecla para salir...");
        Console.ReadKey();
    }
}