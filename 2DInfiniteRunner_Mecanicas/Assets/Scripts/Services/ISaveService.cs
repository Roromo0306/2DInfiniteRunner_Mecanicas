using System.Collections.Generic;

public interface ISaveService : IService
{
    void SaveHighScore(string playerName, int score);
    List<(string name, int score)> LoadHighScores(int top = 10);
}
