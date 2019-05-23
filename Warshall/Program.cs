using System;
using System.Text;

namespace Warshall
{

    struct Example
    {

        public int[,] AdjacencyMatrix;
        public int[,] WarshallSolution; //Known solution from Mathematica
        public int[,] GraphDistanceMatrixSolution; //Known solution from Mathematica with nVertex + 1 substituting for Infinity

    }

    class Program
    {
        static void Main(string[] args)
        {
            var exmpl = new Example
            {
                AdjacencyMatrix = new [,] {{0, 1, 0, 0, 0, 0, 0, 0, 0, 0}, {1, 0, 1, 0, 0, 0, 0, 0, 0, 0}, {0,
                    0, 0, 1, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, {0, 0,
                    1, 0, 0, 0, 0, 0, 0, 0}, {0, 1, 0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0,
                    0, 0, 0, 0, 1, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0,
                    0, 0, 1, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 1, 1, 0}},
                WarshallSolution = new[,] {{1, 1, 1, 1, 0, 0, 0, 0, 0, 0}, {1, 1, 1, 1, 0, 0, 0, 0, 0, 0}, {0,
                    0, 0, 1, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, {0, 0,
                    1, 1, 0, 0, 0, 0, 0, 0}, {1, 1, 1, 1, 0, 0, 0, 0, 0, 0}, {0, 0, 0,
                    0, 0, 0, 0, 1, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0,
                    0, 0, 1, 1, 0, 0}, {0, 0, 0, 0, 0, 0, 1, 1, 1, 0}},
                GraphDistanceMatrixSolution = new[,] {{0, 1, 2, 3, 11, 11, 11, 11, 11, 11}, {1, 0, 1, 2, 11, 11, 11, 11,
                    11, 11}, {11, 11, 0, 1, 11, 11, 11, 11, 11, 11}, {11, 11, 11, 0, 11,
                    11, 11, 11, 11, 11}, {11, 11, 1, 2, 0, 11, 11, 11, 11, 11}, {2, 1,
                    2, 3, 11, 0, 11, 11, 11, 11}, {11, 11, 11, 11, 11, 11, 0, 1, 11,
                    11}, {11, 11, 11, 11, 11, 11, 11, 0, 11, 11}, {11, 11, 11, 11, 11,
                    11, 1, 2, 0, 11}, {11, 11, 11, 11, 11, 11, 2, 1, 1, 0}}
            };

            //Check AreEqual is working
            Console.WriteLine(AreEqual(exmpl.AdjacencyMatrix, exmpl.AdjacencyMatrix)); //true
            Console.WriteLine(AreEqual(exmpl.AdjacencyMatrix, exmpl.GraphDistanceMatrixSolution)); //false

            Console.WriteLine($"There are {exmpl.AdjacencyMatrix.GetLength(0)} vertices");
            Console.WriteLine("Input adjacency matrix is:");
            Console.WriteLine(Print(exmpl.AdjacencyMatrix));
            
            var reachablityMatrix = ReachabilityMatrix(exmpl.AdjacencyMatrix);

            Console.WriteLine("Reachability matrix is (Warshall algorithm):");
            Console.WriteLine(Print(BoolToInt(reachablityMatrix)));

            Console.WriteLine("Equivalent to Mathematica solution?: " + AreEqual(BoolToInt(reachablityMatrix), exmpl.WarshallSolution));

            var gdm = GraphDistanceMatrix(exmpl.AdjacencyMatrix);
            Console.WriteLine("Graph Distance Matrix is (Floyd-Warshall algorithm):");
            Console.WriteLine(Print(gdm));

            Console.WriteLine("Equivalent to Mathematica solution?: " + AreEqual(gdm, exmpl.GraphDistanceMatrixSolution));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// Calculates the so-called Reachability matrix using the Warshall algorithm
        /// </summary>
        /// <param name="adjacencyMatrix">Weighted and directed edges of the graph. Must be square</param>
        /// <returns></returns>
        public static bool[,] ReachabilityMatrix(int[,] adjacencyMatrix)
        {
            if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
                throw new ArgumentException($"Matrix is not square", nameof(adjacencyMatrix));

            var nVertices = adjacencyMatrix.GetLength(0);

            var adjGraphBool = new bool[nVertices, nVertices];

            //convert int adjacency graph to booleans
            for (var p = 0; p < nVertices; p++)
            {
                for (var q = 0; q < nVertices; q++)
                    adjGraphBool[p, q] = adjacencyMatrix[p, q] != 0;
            }


            var prior = adjGraphBool;
            //Warshall solution
            for (var k = 0; k < nVertices; k++)
            {
                for (var j = 0; j < nVertices; j++)
                {
                    for (var i = 0; i < nVertices; i++)
                    {
                        prior[i, j] = prior[i, j] || (prior[i, k] && prior[k, j]);
                        
                    }
                }

            }

            return prior;
        }

        /// <summary>
        /// Calculates the distance between vertices using the Floyd-Warshall algorithm. Vertices which are not
        /// mutually reachable are assigned a distance of nVertices + 1.
        /// </summary>
        /// <param name="adjacencyMatrix">Weighted and directed edges of the graph. Must be square</param>
        /// <returns></returns>
        public static int[,] GraphDistanceMatrix(int[,] adjacencyMatrix)
        {
            if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
                throw new ArgumentException($"Matrix is not square", nameof(adjacencyMatrix));

            var nVertices = adjacencyMatrix.GetLength(0);

            //initialise distances
            //diagonals => 0 (no distance to oneself)
            //known edges => weight of edge
            //others => infinity
            //if infinity is represented with int.MaxValue, then problems will arise when infinity + infinity is calculated!
            //=> use nVertices + 1
            var solution = new int[nVertices, nVertices];

            for (var i = 0; i < nVertices; i++)
            {
                for (var j = 0; j < nVertices; j++)
                {
                    if (adjacencyMatrix[i, j] != 0)
                        solution[i, j] = adjacencyMatrix[i, j];
                    else if (i == j)
                        solution[i, j] = 0;
                    else
                        solution[i, j] = nVertices + 1; // see comment above

                }
            }

            //Floyd-Warshall: note loop goes k-j-i
            for (var k = 0; k < nVertices; k++)
            {
                for (var j = 0; j < nVertices; j++)
                {
                    for (var i = 0; i < nVertices; i++)
                    {
                        if (solution[i, j] > solution[i, k] + solution[k, j])
                            solution[i, j] = solution[i, k] + solution[k, j];
                    }
                }
            }

            return solution;
        }

        /// <summary>
        /// Convert a matrix of bools to a matrix of 1s and 0s
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int[,] BoolToInt(bool[,] input)
        {
            var result = new int[input.GetLength(0), input.GetLength(1)];
            for (var i = 0; i < input.GetLength(0); i++)
                for (var j = 0; j < input.GetLength(1); j++)
                    result[i, j] = input[i, j] ? 1 : 0;

            return result;
        }

        /// <summary>
        /// Formats a matrix for string output
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Print<T>(T[,] input)
        {
            var sb = new StringBuilder();
            for (var r = 0; r < input.GetLength(0); r++)
            {
                for (var c = 0; c < input.GetLength(1); c++)
                {
                    sb.Append($"|{input[r, c].ToString(),8}");
                }

                if (r == input.GetLength(0) - 1)
                    sb.Append("|");
                else
                    sb.AppendLine("|");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats a matrix for string output
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Print<T>(T[,,] input)
        {
            var sb = new StringBuilder();
            for (var k = 0; k < input.GetLength(0); k++)
            {
                sb.AppendLine($"k = {k}");
                for (var r = 0; r < input.GetLength(1); r++)
                {
                    for (var c = 0; c < input.GetLength(2); c++)
                    {
                        sb.Append($"|{input[k, r, c].ToString(),8}");
                    }

                    if (r == input.GetLength(1) - 1 && k == input.GetLength(0) - 1)
                        sb.Append("|");
                    else
                        sb.AppendLine("|");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Compares two matrices for size (rows x columns) and elements, applying Equals(...) comparisons to each element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool AreEqual<T>(T[,] left, T[,] right) where T : IComparable<T>
        {
            if (left.GetLength(0) != right.GetLength(0))
                return false;

            if (left.GetLength(1) != right.GetLength(1))
                return false;

            for (var i = 0; i < left.GetLength(0); i++)
            {
                for (var j = 0; j < left.GetLength(1); j++)
                {
                    if (!left[i, j].Equals(right[i, j]))
                    {
                        return false;
                    }
                }
            }

            return true;

        }
    }
}
