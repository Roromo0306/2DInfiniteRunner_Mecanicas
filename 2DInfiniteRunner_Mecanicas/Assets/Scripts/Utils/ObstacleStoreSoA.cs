using UnityEngine;
using System;

public class ObstacleStoreSoA
{
    public int capacity = 256;
    public int count = 0;
    public float[] posX;
    public float[] posY;
    public float[] widths;   
    public float[] speeds;
    public bool[] active;

    public ObstacleStoreSoA(int capacity = 256)
    {
        this.capacity = capacity;
        posX = new float[capacity];
        posY = new float[capacity];
        widths = new float[capacity];
        speeds = new float[capacity];
        active = new bool[capacity];
    }

    public int Add(float x, float y, float width, float speed)
    {
        if (count >= capacity) Expand();
        int i = count++;
        posX[i] = x; posY[i] = y; widths[i] = width; speeds[i] = speed; active[i] = true;
        return i;
    }

    void Expand()
    {
        int newCap = capacity * 2;
        Array.Resize(ref posX, newCap);
        Array.Resize(ref posY, newCap);
        Array.Resize(ref widths, newCap);
        Array.Resize(ref speeds, newCap);
        Array.Resize(ref active, newCap);
        capacity = newCap;
    }

    public void RemoveAt(int i)
    {
        // swap back to keep dense array
        int last = count - 1;
        if (i < 0 || i >= count) return;
        posX[i] = posX[last];
        posY[i] = posY[last];
        widths[i] = widths[last];
        speeds[i] = speeds[last];
        active[i] = active[last];
        count--;
    }
}