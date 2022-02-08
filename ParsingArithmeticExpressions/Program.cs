using System;

namespace ParsingArithmeticExpressions
{
    class MyStack<T>
    {
        private int top = -1;// amount of items
        private T[] arrStack;//stack items
        
        public MyStack(int size) // making empty stack
        {
            arrStack = new T[size];
        }
        
        public void StackPush(T e) // add item
        {
            top++;
            arrStack[top] = e;
        }
        
        public T StackPop() // return an item with deletion
        {
            if (top == -1)
            {
                throw new InvalidOperationException("Stack is empty");
            }
            T arrStack1 = arrStack[top];
            top--;
            return arrStack1;
        }
        
        public T StackPeek() //view top item without deleting
        {
            if (top == -1)
            {
                throw new InvalidOperationException("Stack is empty");
            }
            return arrStack[top];
        }
    }
    class Program
    {
        static bool BauerZamelsonAlgorithm(string str, out double result)
        {
            result = -1;
            MyStack<double> stackE = new MyStack<double>(str.Length);
            MyStack<char> stackT = new MyStack<char>(str.Length);
            str += "$";
            stackT.StackPush('$');
            byte[,] jumpTable = new byte[6, 7] { { 6, 1, 1, 1, 1, 1, 5},
                                                 { 5, 1, 1, 1, 1, 1, 3},
                                                 {4, 1, 2, 2, 1, 1, 4},
                                                 {4, 1, 2, 2, 1, 1, 4},
                                                 {4, 1, 4, 4, 2, 2, 4},
                                                 {4, 1, 4, 4, 2, 2, 4}};

            char[] operationsArray = new char[7] { '$', '(', '+', '-', '*', '/', ')' };
            int i = 0;
            bool end = true;

            while (end && i < str.Length)
            {
                if (str[i] == '(')
                {
                    stackT.StackPush('(');
                    i++;
                }
                try
                {
                    stackE.StackPush(selectingTheNextOperand(str, ref i));
                    end = stepF(jumpTable, stackE, stackT, operationsArray, str, ref i);
                }
                catch
                {
                    return false;
                }
                i++;
            }
            result = stackE.StackPop();
            return true;
        }

        static bool stepF(byte[,] jumpTable, MyStack<double> stackE, MyStack<char> stackT, char[] operationsArray, string str, ref int i) // algorithm step
        {
            byte caseSwitch = jumpTable[findIndex(operationsArray, stackT.StackPeek()), findIndex(operationsArray, str[i])];
            switch (caseSwitch)
            {
                case 1: stackT.StackPush(str[i]); break;
                case 2:
                    stackE.StackPush(performAnOperation(findIndex(operationsArray, stackT.StackPop()), stackE.StackPop(), stackE.StackPop()));
                    stackT.StackPush(str[i]);
                    break;
                case 3:
                    stackT.StackPop();
                    i++;
                    stepF(jumpTable, stackE, stackT, operationsArray, str, ref i);
                    break;
                case 4:
                    stackE.StackPush(performAnOperation(findIndex(operationsArray, stackT.StackPop()), stackE.StackPop(), stackE.StackPop()));
                    stepF(jumpTable, stackE, stackT, operationsArray, str, ref i);
                    break;
                case 5: throw new InvalidOperationException("Error f5");
                case 6: return false;
            }
            return true;
        }

        static double performAnOperation(int operation, double firstNo, double secondNo)
        {
            switch (operation)
            {
                case 2: return secondNo + firstNo;
                case 3: return secondNo - firstNo;
                case 4: return secondNo * firstNo;
                case 5: return secondNo / firstNo;
                default: return 0;
            }
        }

        static double selectingTheNextOperand(string str, ref int indx) // select the following number from the row
        {
            double sign = 1;
            string operand = "";
            if (indx == 0 && str[0] == '-') // if at the beginning of the line is a negative number
            {
                sign = -1;
                indx++;
            }

            while (indx < str.Length - 1 && (str[indx] >= '0' && str[indx] <= '9' || str[indx] == ','))
            {
                operand += str[indx]; indx++;
            }
            if (!(double.TryParse(operand, out double res)))
            {
                throw new InvalidOperationException("Conversion Error.");
            }
            return res * sign;
        }

        static int findIndex(char[] arr, char a) // operation index
        {
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i] == a) return i;
            return -1;
        }

        static void test(string str)
        {
            if (BauerZamelsonAlgorithm(str, out double res1)) Console.WriteLine(res1);
            else Console.WriteLine("The entered data is incorrect");
        }

        static void Main(string[] args)
        {
            // A few examples to check
            string str1 = "16,0-17+110", // answer: 109
                str2 = "10-2*2+3*1", // answer: 9
                str3 = "14-(21-1)/5", // answer: 10
                str4 = "4+1//3"; // answer: The entered data is incorrect

            test(str1);
            test(str2);
            test(str3);
            test(str4);

            Console.Write("Press any key to finish.");
            Console.ReadKey();

        }
    }
}
