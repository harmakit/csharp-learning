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

            for (int i = 1; i < list.Count; i++) {

                var ai = list.First;
                var aiminus1 = list.First;
                bool start = false;
                var check = list.First;


                int k = 0;
                while (k < i) {
                    ai = ai.Next;
                    k++;
                }
                aiminus1 = ai.Prev;

                for (int count = 0; count < i; count++) {

                    if (check.Data.get_molar_mass() > ai.Data.get_molar_mass()) {
                        start = true;
                        break;
                    }
                    check = check.Next;
                }

                if (start) {
                    var x = ai.Data;
                    int left = 0;
                    int right = i - 1;
                    do {
                        int sred = (left + right) / 2;
                        var asred = list.First;
                        if (sred != 0) {
                            for (int counter = 0; counter < sred; counter++)
                                asred = asred.Next;
                        }
                        if (asred.Data.get_molar_mass() < x.get_molar_mass())
                            left = sred + 1;
                        else
                            right = sred - 1;

                    } while (left <= right);

                    for (int j = i - 1; j >= left; j--) {

                        var aj = list.First;
                        var ajplus1 = list.First;

                        for (int count = 0; count < j; count++)
                            aj = aj.Next;
                        ajplus1 = aj.Next;

                        ajplus1.Data = aj.Data;

                    }

                    var aleft = list.First;
                    for (int count = 0; count < left; count++)
                        aleft = aleft.Next;
                    aleft.Data = x;
                }
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
            string String, word = "", massString = "";
            int mass;
            bool atom = false, inorganic = false, organic = true, massCount = false;
            while ((String = reader.ReadLine()) != null) {
                if (String.Length != 0)

                    for (int i = 0; i < String.Length; i++) {
                        if (String[0] == '/') {
                            break;
                        }

                        if ((Char.IsLetterOrDigit(String[i]) || String[i] == ',') && (massCount == false)) { //счет digit
                            if (String[i] == ',') { atom = true; organic = false; };
                            word += String[i];
                        } else {
                            if (String[i] == '-' || massCount == true) {
                                massCount = true;
                                if (Char.IsDigit(String[i])) {
                                    if (massCount) massString += String[i];
                                    if (String[i + 1] == '*') massCount = false;

                                }
                            } else {
                                if (Char.IsLetter(word[0])) {
                                    organic = false;
                                    inorganic = true;
                                }
                                mass = int.Parse(massString);
                                if (inorganic) {
                                    inorganic_matter X = new inorganic_matter(mass);
                                    X.insertAattribute(list, X, word);
                                }
                                if (atom) {
                                    atom X = new atom(mass);
                                    X.insertAattribute(list, X, word);
                                }
                                if (organic) {
                                    organic_matter X = new organic_matter(mass);
                                    X.insertAattribute(list, X, word);
                                }

                                word = "";
                                massString = "";
                                atom = false;
                                massCount = false;
                                organic = true;
                                inorganic = false;
                            }
                        }
                    }
            }
            return list;
        }


        abstract public class matter {
            public abstract int get_molar_mass();
            public virtual void insertAattribute(LinkedList list, matter X, string inputAtt) { }
            public abstract void write(StreamWriter writer);
        }

        public class organic_matter : matter {
            private int molar_mass;
            public int attribute; 

            public organic_matter(int mass) {
                molar_mass = mass;
            }

            public override int get_molar_mass() {
                return molar_mass;
            }

            public void insertAattribute(LinkedList list, organic_matter X, string inputAtt) {
                X.attribute = int.Parse(inputAtt);
                list.Add(X);
            }

            public override void write(StreamWriter writer) {
                writer.WriteLine("Organic matter\nMolar mass: {0}\nAmount of hydrogen atoms: {1}", molar_mass, attribute);
                writer.WriteLine();
            }
        }

        public class inorganic_matter : matter {
            private int molar_mass;
            public string attribute; 

            public inorganic_matter(int mass) {
                molar_mass = mass;
            }

            public override int get_molar_mass() {
                return molar_mass;
            }
            public void insertAattribute(LinkedList list, inorganic_matter X, string inputAtt) {
                X.attribute = inputAtt;
                list.Add(X);
            }

            public override void write(StreamWriter writer) {
                writer.WriteLine("Inorganic matter\nMolar mass: {0}\nName: {1}", molar_mass, attribute);
                writer.WriteLine();
            }
        }
        public class atom : matter {
            private int molar_mass;
            public double attribute; 

            public atom(int mass) {
                molar_mass = mass;
            }

            public override int get_molar_mass() {
                return molar_mass;
            }

            public void insertAattribute(LinkedList list, atom X, string inputAtt) {
                X.attribute = Double.Parse(inputAtt);
                list.Add(X);
            }

            public override void write(StreamWriter writer) {
                writer.WriteLine("Atom\nMolar mass: {0}\nElectronegativity: {1}", molar_mass, attribute);
                writer.WriteLine();
            }
        }

        public class LinkedList {
            public LinkedList() {
                size = 0;
                First = Last = null;
            }

            public class Node {
                public matter Data;
                public Node Next;
                public Node Prev;
            };

            public Node First;
            public Node Last;
            private uint size;

            public void Add(matter value) {
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
        }
    }
}
