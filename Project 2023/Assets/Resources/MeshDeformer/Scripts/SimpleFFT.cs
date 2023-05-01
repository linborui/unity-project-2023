using UnityEngine;

public class SimpleFFT : MonoBehaviour {
    public AudioSource audioSource;

    private int m_samples = 64;
    private MeshDeformer deformer;
    private float[] m_spectrum;
    public float[] Spectrum { get { return m_spectrum; } }

    private void Start()
    {
        m_spectrum = new float[m_samples];
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null) {
            enabled = false;
            Debug.LogWarning(
                "No AudioSource set and no AudioSource attached to GameObject.\n" +
                "Music or sound will not be used for the mesh deformation.");

            return;
        }

        deformer = GetComponent<MeshDeformer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            else
                audioSource.Play();
        }
        audioSource.GetSpectrumData(m_spectrum, 0, FFTWindow.BlackmanHarris);
    }
}
