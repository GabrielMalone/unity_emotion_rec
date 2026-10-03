using UnityEngine;

public class AudioAnalyzer : MonoBehaviour
{
    [Header("Audio Source")]
    // the audio source that is playing whatever song we have loaded
    public AudioSource audioSource;

    [Header("Spectrum Settings")]
    // how much enegery is present at different frequencies right now?
    // we will try 512 to hold a spectrum of frequnecy -- 
    // but what each bin represents frequency wise is based on audio sample rate
    // higher number = more 
    public int spectrumSize = 2048;

    // FFT = Fast Fourier Transform
    // takes chunks of audio and converts it into information about which frequencies are present
    // BlackmanHarris algo is a way to get clearer readings of these frequencies
    // song -> take small chunk -> apply blackmanharris -> FFT (convert waves into specific frequencies) -> sort frequences into bins
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("Live Audio Data")]
    public float volume;
    public float bass;
    public float lowMid;
    public float mid;
    public float highMid;
    public float treble;
    public float dominantFrequency;

    private float[] spectrum;
    private float[] samples;

    [Header("Beat Detection")]
    public static bool beatDetected;
    public static bool trebleDetected;

    public float beatThreshold = 0.02f;
    public float beatCooldown = 0.15f;
    private float beatPulse = 1f;

    public float minSpeed = 5f;
    public float maxSpeed = 25f;
    public float maxSpinSpeed = 5f;
    public float beatSpeedMultiplier = 1.5f;

    private float previousBass;
    private float lastBeatTime;
    public float bassIncrease;

    private float previousTreble;
    private float lastTrebleTime;
    public float trebleIncrease;

    public float trebleThreshold = 0.0001f;
    public float targetSpeed;

    [Header("Volume")]
    public float minVolume = 0.05f;
    public float maxVolume = 1f;

    [Header("Frequency")]
    public float minFrequency = 50f;
    public float maxFrequency = 3000f;

    [Header("Treble")]
    public float minTreble = 0.001f;
    public float maxTreble = 0.05f;

    public Color currentColor;

    [Header("Smoothing")]
    public float colorSpeed = 0.25f;


    void Start()
    {

        // create new array with 512 slots (or whatever we set the spectrum size to)
        // each slow in the array represents a small range of frequencies
        // for example spectrum[0] will hold a value telling us how strong the very lowest frequency is
        // spectrumSize = 512 means we're asking Unity to divide the frequency spectrum into 512 chunks 
        // so we can measure how strong each chunk is.

        spectrum = new float[spectrumSize];
        samples = new float[spectrumSize];
    }

    void Update()
    {
        AnalyzeAudio();
        DetectBeat();
        DetectTreble();
        UpdateMovement();
        FrequencyColorChange();
    }

    void AnalyzeAudio()
    {
        if (audioSource == null)
            return;

        // -------------------------------------------------
        // FREQUENCY DATA
        // -------------------------------------------------

        // this will fill the array 'spectrum' that we are passing in
        // thus don't need to return anything
        // so just saying analyuze the current audio and put the results into the spectrum array
        // magnitude of the audio at each frequency range (we decide to split into x number of frequency slots)
        audioSource.GetSpectrumData(
            spectrum,
            0,
            fftWindow
        );

        // Frequency bands

        // turn those 512 spectrum bins into broader frequency ranges:
        // how much energy is in each frequency range rightnow
        // e.g. for bass -> go through the spectrum bins that correspond to 20-250hz 
        // and average their values and 
        // store that in bass
        bass = GetFrequencyRange(20f, 250f);
        lowMid = GetFrequencyRange(250f, 500f);
        mid = GetFrequencyRange(500f, 2000f);
        highMid = GetFrequencyRange(2000f, 4000f);
        treble = GetFrequencyRange(6000, 12000f);

        dominantFrequency = GetDominantFrequency();

        // -------------------------------------------------
        // RAW WAVEFORM / VOLUME
        // -------------------------------------------------

        //gives actual audio waveform samples
        audioSource.GetOutputData(samples, 0);

        float total = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            total += samples[i] * samples[i];
            // square to not get muddied data from negative values
        }

        // RMS volume
        // average, then square root to return us to roughly same scale as before
        volume = Mathf.Sqrt(total / samples.Length);
    }

    float GetFrequencyRange(float minFrequency, float maxFrequency)
    {
        // how many measurements per second (e.g. 48000)
        float sampleRate = AudioSettings.outputSampleRate;

        // the highest frequency that can be represented accurately at that sample rate
        // so 48k / 2 = 24k meaning our 512 sampelrate bins cover 0hz -> 24k hz
        float nyquist = sampleRate / 2f;

        // find the first bin for this frequency
        // e.g. 20hz / 24k * 512 = 0.45 floored = 0
        int minIndex = Mathf.FloorToInt(
            minFrequency / nyquist * spectrum.Length
        );
        // find the last bine for this frequency range
        // e.g. 250hz / 24k *512 = 5.3 ceiled to 6
        // so just frequence / max frequncy * num bins
        int maxIndex = Mathf.CeilToInt(
            maxFrequency / nyquist * spectrum.Length
        );

        minIndex = Mathf.Clamp(
            minIndex,
            0,
            spectrum.Length - 1
        );

        maxIndex = Mathf.Clamp(
            maxIndex,
            0,
            spectrum.Length - 1
        );

        float total = 0f;
        int count = 0;

        for (int i = minIndex; i <= maxIndex; i++)
        {
            // count up all the values in the range of indexes we just found
            total += spectrum[i];
            count++;
        }

        if (count == 0)
            return 0f;

        // return the average value
        return total / count;
    }

    public float GetDominantFrequency()
    {
        int strongestIndex = 0;
        float strongestValue = 0f;


        // simple max calculator
        for (int i = 0; i < spectrum.Length; i++)
        {
            if (spectrum[i] > strongestValue)
            {
                strongestValue = spectrum[i];
                strongestIndex = i;
            }
        }

        float sampleRate = AudioSettings.outputSampleRate;
        float nyquist = sampleRate / 2f;

        float frequency =
            // nyquist / spectrum.length = how wide each bin is in our spectrum array of 512 bins
            strongestIndex * nyquist / spectrum.Length;
        // so say strongest index = 8 and each bin is 46.8 hz then 8 * 46.8 = 375hz

        return frequency;
    }

    void DetectBeat()
    {
        // How much did the bass increase since last frame?
        bassIncrease = bass - previousBass;

        // Assume no beat this frame
        beatDetected = false;

        // If bass suddenly jumped AND enough time has passed
        // since the previous beat...
        if (bassIncrease > beatThreshold &&
            Time.time - lastBeatTime > beatCooldown)
        {
            beatDetected = true;
            lastBeatTime = Time.time;
            Debug.Log("Beat detected!!");
        }

        // Save current bass for comparison next frame
        previousBass = bass;
    }

    void DetectTreble()
    {
        trebleIncrease = treble - previousTreble;
        trebleDetected = false;

        if (trebleIncrease > trebleThreshold &&
            Time.time - lastTrebleTime > beatCooldown)
        {
            trebleDetected = true;
            lastTrebleTime = Time.time;
            Debug.Log("Treble detected!!");

        }
        previousTreble = treble;
    }


    void UpdateMovement()
    {
        float volumeAmount = Mathf.InverseLerp(
            minVolume,
            maxVolume,
            volume
        );

        targetSpeed = Mathf.Lerp(
            minSpeed,
            maxSpeed,
            volumeAmount
        );

        if (beatDetected)
        {
            beatPulse = beatSpeedMultiplier;
        }

        beatPulse = Mathf.Lerp(
            beatPulse,
            1f,
            10 * Time.deltaTime

        );

        targetSpeed *= beatPulse;

    }

    void FrequencyColorChange()
    {
        // -------------------------------
        // FREQUENCY -> COLOR
        // -------------------------------

        float frequency = dominantFrequency;
        // how far is the value between a minimum and a maximum
        // min and max frequency really depends on the song I guess
        float hue = Mathf.InverseLerp(
            minFrequency,
            maxFrequency,
            frequency
        );

        // -------------------------------
        // CREATE COLOR
        // -------------------------------
        // hue, saturation, value to RGB
        Color targetColor = Color.HSVToRGB(
            hue,
            1f,
            1f
        );


        // -------------------------------
        // SMOOTH COLOR CHANGE
        // -------------------------------

        currentColor = Color.Lerp(
            currentColor,
            targetColor,
            colorSpeed * Time.deltaTime
        );

    }


}