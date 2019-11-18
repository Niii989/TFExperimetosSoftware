using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpeechScript : MonoBehaviour
{
    public Text myTemp;
    public Button str;

    private KeywordRecognizer reconocerpalabras;
    private ConfidenceLevel nivel_confianza = ConfidenceLevel.Low;
    private Dictionary<string, Accion> palabrasaccion = new Dictionary<string, Accion>();   
    private KnotPoints exec;
    private EmergencyStop esc;


    private delegate void Accion();

    //start is called before the first frame update
    void Start()
    {
        exec = FindObjectOfType<KnotPoints>();
        esc = FindObjectOfType<EmergencyStop>();
        Debug.Log(exec.auxiliar);

        palabrasaccion.Add("clear", exec.ClearPoints);
        palabrasaccion.Add("put away", exec.SavePoint);
        palabrasaccion.Add("start", exec.GoThroughPoints);
        palabrasaccion.Add("download", exec.DownloadCodeMELFA);
        palabrasaccion.Add("stop", esc.Stop);

        reconocerpalabras = new KeywordRecognizer(palabrasaccion.Keys.ToArray(), nivel_confianza);
        reconocerpalabras.OnPhraseRecognized += OnKeywordsRecognized;
        reconocerpalabras.Start();
    }

    void StartSth()
    {
        
    }

    void EndSth()
    {

    }

    void OnDestroy()
    {
        if (reconocerpalabras != null && reconocerpalabras.IsRunning)
        {
            reconocerpalabras.Stop();
            reconocerpalabras.Dispose();
        }
    }

    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
    {
        myTemp.text = args.text;
        palabrasaccion[args.text].Invoke();
    }

}
