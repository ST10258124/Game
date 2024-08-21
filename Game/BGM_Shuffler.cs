using System;
using Godot;

public class BGM_Shuffler
{
    private Random _random = new Random();

    int j, swap;
    string temp;

    public void Shuffle(string[] array)
    {
        // Store the last element
        string lastElement = array[array.Length - 1];

        // Shuffle the entire array
        for (int i = array.Length - 1; i > 0; i--)
        {
            j = _random.Next(i + 1);
            temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        // Check if the last element of the original array is now the first element
        if (array[0] == lastElement)
        {
            // Swap the first element with the something else
            swap = _random.Next(3);
            temp = array[0];
            array[0] = array[swap];
            array[swap] = temp;
        }
    }
}
