using System;
using UnityEngine;

public static class FFTUtility
{
    public static void FFT(Complex[] buffer)
    {
        int n = buffer.Length;
        int bits = (int)Mathf.Log(n, 2);

        // битовая перестановка
        for (int j = 1, i = 0; j < n; j++)
        {
            int bit = n >> 1;
            for (; (i & bit) != 0; bit >>= 1)
                i &= ~bit;
            i |= bit;

            if (j < i)
            {
                var temp = buffer[j];
                buffer[j] = buffer[i];
                buffer[i] = temp;
            }
        }

        // основные шаги FFT
        for (int len = 2; len <= n; len <<= 1)
        {
            float ang = -2 * Mathf.PI / len;
            Complex wlen = new Complex(Mathf.Cos(ang), Mathf.Sin(ang));

            for (int i = 0; i < n; i += len)
            {
                Complex w = new Complex(1, 0);

                for (int j = 0; j < len / 2; j++)
                {
                    Complex u = buffer[i + j];
                    Complex v = buffer[i + j + len / 2] * w;

                    buffer[i + j] = u + v;
                    buffer[i + j + len / 2] = u - v;

                    w *= wlen;
                }
            }
        }
    }
}

public struct Complex
{
    public float r, i;

    public Complex(float real, float imag = 0f)
    {
        r = real; i = imag;
    }

    public static Complex operator +(Complex a, Complex b) => new Complex(a.r + b.r, a.i + b.i);
    public static Complex operator -(Complex a, Complex b) => new Complex(a.r - b.r, a.i - b.i);
    public static Complex operator *(Complex a, Complex b) =>
        new Complex(a.r * b.r - a.i * b.i, a.r * b.i + a.i * b.r);

    public float Magnitude => Mathf.Sqrt(r * r + i * i);
}