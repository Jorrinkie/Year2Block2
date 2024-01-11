using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class NoiseMovement : MonoBehaviour
{
    AudioSource audioSource;
    public float speedMultiplier;

    //default Mic
    public string selectedDevice;
    private Rigidbody rb;
    // Block for the Output Data  (audioSource.GetOutputData)()
    public static float[] samples = new float[128];
    [SerializeField] bool run = false;
    [SerializeField] bool isrunning = false;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        //look for microphones, it selects the default microphone, it sets the audioSource.clip to the default mic, loops it for 1 second at the sampleRate

        if (Microphone.devices.Length > 0)
        {
            selectedDevice = Microphone.devices[0].ToString();
            audioSource.clip = Microphone.Start(selectedDevice, true, 1, AudioSettings.outputSampleRate);
            audioSource.loop = true;

            //While the position of the mic in the recording is greater than 0, play the clip (the mic)
            while (!(Microphone.GetPosition(selectedDevice) > 0))
            {
                audioSource.Play();
            }
        }
    }


    void Update()
    {
        getOutputData();
    }

    /**
     * Load the block samples with data from the audioSource output
     * Average the values across the size of the block.
     * vals is the volume of the mic, used to control block height
     * Block height represents candle flame getting larger
     */
    void getOutputData()
    {

        Vector3 cameraForward = Camera.main.transform.forward;

        cameraForward.y = 0f;
        cameraForward.Normalize();

        audioSource.GetOutputData(samples, 0);

        float vals = 0.0f;

        for (int i = 0; i < 128; i++)
        {
            vals += Mathf.Abs(samples[i]);
        }
        vals /= 128.0f;
        Debug.Log(vals);
        if (vals > .007f)
        {
            if (isrunning == false) { StartCoroutine(Move()); }
            else
            {
                StopCoroutine(Move());
                StartCoroutine(Move());
            }
        }

        if (run)
        {
            run = false;
            //WHY NOT FORCE AAA
            //rb.MovePosition(transform.position + cameraForward * 1 * speedMultiplier * Time.deltaTime);

            rb.AddForce(cameraForward * vals * speedMultiplier * Time.deltaTime);
            
        }

    }

    IEnumerator Move()
    {
        isrunning = true;
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSeconds(.1f);
            run = true;
        }
        isrunning = false;

    }
}