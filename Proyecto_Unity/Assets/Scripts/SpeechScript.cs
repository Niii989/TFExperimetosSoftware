using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpeechScript : MonoBehaviour
{
    public Text myTemp;

    private KeywordRecognizer reconocerpalabras;
    private ConfidenceLevel nivel_confianza = ConfidenceLevel.Low;
    private Dictionary<string, Accion> palabrasaccion = new Dictionary<string, Accion>();
    private Kinematics exec;
    private EmergencyStop esc;


    private delegate void Accion();

    //start is called before the first frame update
    void Start()
    {
        exec = FindObjectOfType<Kinematics>();
        esc = FindObjectOfType<EmergencyStop>();

        palabrasaccion.Add("start", exec.StartKinematics);
        palabrasaccion.Add("stop", esc.Stop);

        reconocerpalabras = new KeywordRecognizer(palabrasaccion.Keys.ToArray(), nivel_confianza);
        reconocerpalabras.OnPhraseRecognized += OnKeywordsRecognized;
        reconocerpalabras.Start();
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
