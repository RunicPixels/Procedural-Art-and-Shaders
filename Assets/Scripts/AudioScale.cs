using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScale : MonoBehaviour
{
    // Tried Frequency bands on my own, scaling was a bit iffy, so now using the same method as Peerplay's tutorials.
    
    protected AudioSource _audioSource;
    public static float[] AudioSamples = new float[512];
    public static float[] FrequencyBands = new float[8];
    public static float[] BandBuffer = new float[8];
    private float[] _bufferDecrease = new float[8];
    
    public static float[] FrequencyBands16 = new float[16];
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CreateAudioSpectrum();
        MakeFrequencyBands();
        DoBandBuffer();
        MakeFrequencyBands16();
    }
    
    void CreateAudioSpectrum()
    {
        _audioSource.GetSpectrumData(AudioSamples, 0, FFTWindow.Blackman);
        
    }

    void DoBandBuffer()
    {
        for(int i = 0; i < BandBuffer.Length; i++)
        {
            if (FrequencyBands[i] > BandBuffer[i])
            {
                BandBuffer[i] = FrequencyBands[i];
                _bufferDecrease[i] = 0.001f;
            }

            if (FrequencyBands[i] < BandBuffer[i])
            {
                BandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.15f;
            }
        }
    }
    
    void MakeFrequencyBands()
    {
        /*
        0 - 2 = 86
        1 - 4
        2 - 8 
        3 - 16
        4 - 32
        5 - 64
        6 - 128
        7 - 256
        */

        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int) Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += AudioSamples[count] * (count + 1);
                count++;
            }

            average /= count;

            FrequencyBands[i] = average * 10;
        }
    }
    
    void MakeFrequencyBands16()
    {
        /*
        0 - 2 = 86
        1 - 4
        2 - 8 
        3 - 16
        4 - 32
        5 - 64
        6 - 128
        7 - 256
        8 -
        9 -
        10 -
        11 -
        12 -
        13 - 
        14 -
        15 - 
        16 -
        */

        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int) Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += AudioSamples[count];
                count++;
            }

            average /= count;

            FrequencyBands[i] = average * 10;
        }
    }
    
}

/* Old Code

        GetSampleValues(out var values, spectrumData, range * 8, (range + 1) * 8);

        return values.Max() * yGlobalModifier;
*/