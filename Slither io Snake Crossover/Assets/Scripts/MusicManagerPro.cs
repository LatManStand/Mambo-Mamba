using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerPro : MonoBehaviour
{
    public static MusicManagerPro Instance;


    private void Awake()
    {
       if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
