using System;
using System.IO;
using System.Linq;
using System.Text;


namespace course_work {

    class Program {
        static void Main(string[] args) {

            FileStream iStream = new FileStream("input.txt", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(iStream, Encoding.Default);

            LinkedList list = input(reader); 
            reader.Close();
            iStream.Close();

            sort(list);

            FileStream oStream = new FileStream("output.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(oStream, Encoding.Default);

            output(list, writer);
            writer.Close();
            oStream.Close();

            Console.WriteLine("Data was successfully written. Press any key to close...");
            Console.ReadKey();
        }

        public static LinkedList sort(LinkedList list) {

            for (int i = 0; i < list.Count; i++) {
                var sorted = list.First;
                for (int j = 0; j < i; j++)
                    sorted = sorted.Next;
                var current = sorted;

                int min = current.Data.getDimension();
                current = current.Next;

                while (current != null) {
                    if (current.Data.getDimension() < min)
                        min = current.Data.getDimension();

                    current = current.Next;
                }

                LinkedList temp = list;

                LinkedList.Node matchingNode = temp.findMatch(min, sorted);

                var dummy = matchingNode.Data;
                matchingNode.Data = sorted.Data;
                sorted.Data = dummy;

            }
            return list;
        }

        static public void output(LinkedList input, StreamWriter writer) {
            int i = 0;
            LinkedList.Node current = input.First;
            while (i < input.Count) {
                i++;
                current.Data.write(writer);
                current = current.Next;
            }
        }

        static LinkedList input(StreamReader reader) {
            LinkedList list = new LinkedList();
            string String, word = "";
            int charInString = 0, inputStrings = 0;
            while ((String = reader.ReadLine()) != null) {
                if (String.Length != 0)
                    inputStrings++;

                for (int i = 0; i < String.Length; i++) {
                    if (String[0] == '/') {
                        inputStrings--;
                        break;
                    }

                    if (Char.IsDigit(String[i]) && String[i] != 's' && String[i] != 'd' && String[i] != 't') {
                        word += String[i];
                        word += ";"; 
                        charInString = charInString + 2;
                    } else {
                        word = word.Remove(charInString - 1);
                        int[] splitted = word.Split(';').Select(int.Parse).ToArray();

                        if (String[i] == 't') {
                            int lengthCounter = 0;
                            int t = 1;
                            for (int s = splitted.Length; s > 0; s = s - t) {
                                lengthCounter++;
                                t++;
                            }

                            triangle_matrix X = new triangle_matrix(lengthCounter);
                            X.insertArray(list, X, splitted);
                        }
                        if (String[i] == 'd') {
                            diagonal_matrix X = new diagonal_matrix(splitted.Length);
                            X.insertArray(list, X, splitted);
                        }
                        if (String[i] == 's') {
                            square_matrix X = new square_matrix(inputStrings);
                            X.insertArray(list, X, splitted);
                        }

                        word = "";
                        inputStrings = 0;
                        charInString = 0;
                    }
                }
            }
            return list;
        }


        abstract public class matrix {
            public abstract int getDimension();
            public virtual void insertArray(LinkedList list, matrix X, int[] splitted) { }
            public abstract void write(StreamWriter writer);
        }

        public class triangle_matrix : matrix {
            private int dimension;
            public int[,] array;

            public triangle_matrix(int dim) {
                dimension = dim;
            }

            public override int getDimension() {
                return dimension;
            }

            public void insertArray(LinkedList list, triangle_matrix X, int[] splitted) {
                X.array = new int[X.getDimension(), X.getDimension()];
                int count = 0;
                for (int i = 0; i < X.getDimension(); i++)
                    for (int j = 0; j < X.getDimension(); j++)
                        if (j <= i) {
                            X.array[i, j] = splitted[count];
                            count++;
                        } else
                            X.array[i, j] = 0;
                list.Add(X);
            }

            public override void write(StreamWriter writer) {
                writer.WriteLine("The dimension of the matrix: {0}", dimension);

                for (int i = 0; i < dimension; i++) {
                    for (int j = 0; j < dimension; j++)
                        writer.Write("{0} ", array[i, j]);
                    writer.WriteLine();
                }
                writer.WriteLine();
            }
        }

        public class diagonal_matrix : matrix {
            private int dimension;
            public int[,] array;

            public diagonal_matrix(int dim) {
                dimension = dim;
            }

            public override int getDimension() {
                return dimension;
            }
            public void insertArray(LinkedList list, diagonal_matrix X, int[] splitted) {
                X.array = new int[X.getDimension(), X.getDimension()];
                for (int i = 0; i < X.getDimension(); i++)
                    for (int j = 0; j < X.getDimension(); j++) {
                        if (i == j)
                            X.array[i, j] = splitted[j];
                        else
                            X.array[i, j] = 0;
                    }
                list.Add(X);
            }

            public override void write(StreamWriter writer) {
                writer.WriteLine("The dimension of the matrix: {0}", dimension);
                for (int i = 0; i < dimension; i++) {
                    for (int j = 0; j < dimension; j++)
                        writer.Write("{0} ", array[i, j]);
                    writer.WriteLine();
                }
                writer.WriteLine();
            }
        }
        public class square_matrix : matrix {
            private int dimension;
            public int[,] array;

            public square_matrix(int dim) {
                dimension = dim;
            }

            public override int getDimension() {
                return dimension;
            }

            public void insertArray(LinkedList list, square_matrix X, int[] splitted) {
                X.array = new int[X.getDimension(), X.getDimension()];
                for (int i = 0; i < X.getDimension(); i++)
                    for (int j = 0; j < X.getDimension(); j++)
                        X.array[i, j] = splitted[i * X.getDimension() + j];
                list.Add(X);
            }
            
            public override void write(StreamWriter writer) {
                writer.WriteLine("The dimension of the matrix: {0}", dimension);
                for (int i = 0; i < dimension; i++) {
                    for (int j = 0; j < dimension; j++)
                        writer.Write("{0} ", array[i, j]);
                    writer.WriteLine();
                }
                writer.WriteLine();
            }
        }

        public class LinkedList {
            public LinkedList() {
                size = 0;
                First = Last = null;
            }

            public class Node {
                public matrix Data;
                public Node Next;
                public Node Prev;
            };

            public Node First;
            public Node Last;
            private uint size;

            public void Add(matrix value) {
                size++;
                Node tmp = new Node();
                tmp.Data = value;
                tmp.Next = tmp.Prev = null;
                if (First == null) {
                    First = tmp;
                    Last = tmp;
                } else {
                    Last.Next = tmp;
                    tmp.Prev = Last;
                    Last = tmp;
                }
            }

            public uint Count {
                get { return size; }
                set { size = value; }
            }

            public Node findMatch(int min, Node sorted) {
                var v = sorted;
                while (true) {
                    if (v.Data.getDimension() == min)
                        return v;
                    else
                        v = v.Next;
                }
            }
        }
    }
}
