using System;
using System.Collections.Generic;

class SLRParser
{
    Stack<int> stack = new Stack<int>();
    List<string> input = new List<string> { "a", "a", "b", "b", "$" }; // Input "aabb$"

    // Action table: Maps (state, terminal) to an action (shift, reduce, or accept)
    Dictionary<(int, string), string> actionTable = new Dictionary<(int, string), string>();
    // Goto table: Maps (state, non-terminal) to a new state
    Dictionary<(int, string), int> gotoTable = new Dictionary<(int, string), int>();

    public static void Main()
    {
        SLRParser parser = new SLRParser();
        parser.SetupTables();
        parser.PrintParsingTable(); // Print parsing table before parsing
        parser.Parse();
    }

    private void SetupTables()
    {
        // Define action table for grammar
        actionTable[(0, "a")] = "S3";
        actionTable[(0, "b")] = "S4";
        actionTable[(1, "$")] = "Accept";
        actionTable[(2, "$")] = "R1";   // S -> AA
        actionTable[(3, "a")] = "S3";
        actionTable[(3, "b")] = "S4";
        actionTable[(3, "$")] = "R2";   // A -> aA
        actionTable[(4, "$")] = "R3";   // A -> b
        actionTable[(5, "$")] = "R1";   // S -> AA
        actionTable[(5, "a")] = "S3";
        actionTable[(5, "b")] = "S4";

        // Define goto table based on reachable non-terminals
        gotoTable[(0, "A")] = 1;
        gotoTable[(3, "A")] = 2;
        gotoTable[(5, "A")] = 2;
    }

    public void Parse()
    {
        stack.Push(0); // Start in state 0
        int pointer = 0; // Input pointer

        while (true)
        {
            int state = stack.Peek();
            string symbol = input[pointer];

            if (actionTable.TryGetValue((state, symbol), out string action))
            {
                if (action.StartsWith("S")) // Shift action
                {
                    int newState = int.Parse(action.Substring(1));
                    stack.Push(newState);
                    pointer++; // Move to the next input symbol
                    Console.WriteLine($"Shift: {symbol} -> State {newState}");
                }
                else if (action.StartsWith("R")) // Reduce action
                {
                    int prodNum = int.Parse(action.Substring(1));
                    PerformReduction(prodNum);
                }
                else if (action == "Accept")
                {
                    Console.WriteLine("Input is accepted.");
                    break;
                }
            }
            else
            {
                Console.WriteLine($"Error: Parsing table entry not found for state {state} and symbol {symbol}.");
                break;
            }
        }
    }

    private void PerformReduction(int prodNum)
    {
        int newState;
        switch (prodNum)
        {
            case 1: // Production S -> AA
                for (int i = 0; i < 2; i++) stack.Pop();
                if (gotoTable.TryGetValue((stack.Peek(), "S"), out newState))
                {
                    stack.Push(newState);
                    Console.WriteLine("Reduced using S -> AA");
                }
                else
                {
                    Console.WriteLine("Error: gotoTable entry not found for state {0} and non-terminal 'S'", stack.Peek());
                }
                break;

            case 2: // Production A -> aA
                for (int i = 0; i < 2; i++) stack.Pop();
                if (gotoTable.TryGetValue((stack.Peek(), "A"), out newState))
                {
                    stack.Push(newState);
                    Console.WriteLine("Reduced using A -> aA");
                }
                else
                {
                    Console.WriteLine("Error: gotoTable entry not found for state {0} and non-terminal 'A'", stack.Peek());
                }
                break;

            case 3: // Production A -> b
                stack.Pop(); // Pop 1 symbol (b)
                if (gotoTable.TryGetValue((stack.Peek(), "A"), out newState))
                {
                    stack.Push(newState);
                    Console.WriteLine("Reduced using A -> b");
                }
                else
                {
                    Console.WriteLine("Error: gotoTable entry not found for state {0} and non-terminal 'A'", stack.Peek());
                }
                break;
        }
    }

    // Method to print the complete parsing table (actionTable and gotoTable)
    private void PrintParsingTable()
    {
        Console.WriteLine("\nACTION TABLE:");
        foreach (var entry in actionTable)
        {
            Console.WriteLine($"State {entry.Key.Item1}, Symbol '{entry.Key.Item2}' -> {entry.Value}");
        }

        Console.WriteLine("\nGOTO TABLE:");
        foreach (var entry in gotoTable)
        {
            Console.WriteLine($"State {entry.Key.Item1}, Non-terminal '{entry.Key.Item2}' -> State {entry.Value}");
        }
        Console.WriteLine();
    }
}
