using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerPrefsSaveService : ISaveService
{
    private const string HS_KEY = "RUNNER_HIGHSCORES";

    public void SaveHighScore(string playerName, int score)
    {
        var all = LoadAll();
        all.Add((playerName, score));
        var ordered = all.OrderByDescending(x => x.score).Take(10).ToList();
        // serialize simple: name:score;name:score;...
        var str = string.Join(";", ordered.Select(x => $"{x.name}:{x.score}"));
        PlayerPrefs.SetString(HS_KEY, str);
        PlayerPrefs.Save();
    }

    private List<(string name, int score)> LoadAll()
    {
        var raw = PlayerPrefs.GetString(HS_KEY, "");
        if (string.IsNullOrEmpty(raw)) return new List<(string, int)>();
        var parts = raw.Split(';');
        var list = new List<(string, int)>();
        foreach (var p in parts)
        {
            if (string.IsNullOrEmpty(p)) continue;
            var kv = p.Split(':');
            if (kv.Length != 2) continue;
            if (int.TryParse(kv[1], out var sc))
                list.Add((kv[0], sc));
        }
        return list;
    }

    public List<(string name, int score)> LoadHighScores(int top = 10)
    {
        return LoadAll().OrderByDescending(x => x.score).Take(top).ToList();
    }
}
