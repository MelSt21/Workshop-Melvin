using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class DialougeManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialougeText;
    public Animator animator;
    private Queue<string> sentences;
    public PlatformerMovement thisPlayer;
    public TopDownMovement thisPlayerTopDown;
    // Start is called before the first frame update
    private void Start()
    {
        sentences = new Queue<string>();
        
    }

    public void StartDialouge(Dialouge dialouge)
    {
        thisPlayer.controlEnabled = false;
        thisPlayerTopDown.controlEnabled = false;
        animator.SetBool("IsOpen",true);
        nameText.text = dialouge.name;
        
        sentences.Clear();

        foreach (string sentence in dialouge.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentance();
    }

    public void DisplayNextSentance()
    {
        if (sentences.Count == 0)
        {
            EndDialouge();
            return;
        }
        
        string sentence=sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialougeText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialougeText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }
    
    public void EndDialouge()
    {
        Debug.Log($"Ending Dialouge");
        animator.SetBool("IsOpen",false);
        thisPlayer.controlEnabled = true;
    }
    
}
