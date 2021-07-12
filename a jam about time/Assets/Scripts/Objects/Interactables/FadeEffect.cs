using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeEffect : MonoBehaviour
{
    public GameObject item;

    public float fadeTime;
    public bool revealed;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && !revealed){
            StartCoroutine(FadeIn());
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player" && revealed){
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn(){
        revealed = true;
        float addValue = (1/fadeTime)*Time.deltaTime;
        if (item.GetComponent<TMP_Text>() != null){
            while (item.GetComponent<TMP_Text>().alpha < 1){
                item.GetComponent<TMP_Text>().alpha += addValue;
                yield return new WaitForEndOfFrame();
            }
        }   else {
            while (item.GetComponent<SpriteRenderer>().color.a < 1){
                Color color = item.GetComponent<SpriteRenderer>().color;
                color.a += addValue;
                item.GetComponent<SpriteRenderer>().color = color;
                yield return new WaitForEndOfFrame();
            }
        }
        
    }

    IEnumerator FadeOut(){
        revealed = false;
        float addValue = (1/fadeTime)*Time.deltaTime;
        if (item.GetComponent<TMP_Text>() != null){
            while (item.GetComponent<TMP_Text>().alpha > 0){
                item.GetComponent<TMP_Text>().alpha -= addValue;
                yield return new WaitForEndOfFrame();
            }
        }   else {
            while (item.GetComponent<SpriteRenderer>().color.a >0 ){
                Color color = item.GetComponent<SpriteRenderer>().color;
                color.a -= addValue;
                item.GetComponent<SpriteRenderer>().color = color;
                yield return new WaitForEndOfFrame();
            }
        }
    }

}
