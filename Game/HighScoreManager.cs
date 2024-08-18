using System;
using System.IO;
using System.Text;
using Godot;

public class HighScoreManager
{
    private string _filePath = "highscore.save";

    byte[] key = new byte[32];
    byte[] iv = new byte[16];

    //line 10 and 11. . .don't even ask. hopefully nash covers this sometime

    StringEncryptor aes;

    // Call this method to replace a high score if it's higher than the current one
    public void ReplaceHighScore(int newHighScore)
    {
        aes = new StringEncryptor(key, iv);
        string[] lines = ReadHighScoreFile();
        int currentHighScore = GetCurrentHighScore(lines);

        // Check if the new high score is higher than the current high score
        if (newHighScore > currentHighScore)
        {
            // Convert the new high score to a string and replace the line
            lines[0] = newHighScore.ToString();
        } else {
            lines[0] = currentHighScore.ToString();
        }

        lines[0] = aes.EncryptString(lines[0]);
        WriteHighScoreFile(lines);
        //score is 32 bit integer so max score will never exceed 2 147 483 647
    }

    // Reads the high score file and returns all lines
    public string[] ReadHighScoreFile()
    {
        // Ensure the file exists
        if (!File.Exists(_filePath))
        {
            string defaultScore = aes.EncryptString("0") + "\nnice try";
            File.WriteAllText(_filePath, defaultScore); // Initialize with a default high score of 0
        }

        return File.ReadAllLines(_filePath);
    }

    // Writes the provided lines back to the high score file
    private void WriteHighScoreFile(string[] lines)
    {
        File.WriteAllLines(_filePath, lines);
    }

    // Retrieves the current high score from the file lines
    private int GetCurrentHighScore(string[] lines)
    {
        // Assuming the first line is the high score
        // if (lines.Length > 0 && int.TryParse(aes.DecryptString(lines[0]), out int highScore))
        // {
        //     return highScore;
        // }

        // return 0; // Default to 0 if no score is present or parsing fails

        try{
            int.TryParse(aes.DecryptString(lines[0]), out int highScore);
            return highScore;
        } catch {
            return 0;
        }
    }
}