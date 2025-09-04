    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SpeedSystem : MonoBehaviour
{
    public VideoPlayer vid;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Q)){
            OneX();
        }
        if(Input.GetKey(KeyCode.W)){
            DoubleX();
        }
        if(Input.GetKey(KeyCode.E)){
            PentaX();
        }
        if(Input.GetKey(KeyCode.R)){
            NanaX();
        }
        if(Input.GetKey(KeyCode.F)){
            jump();
        }
    }
    void OneX(){
        vid.playbackSpeed = 1f;
    }
    void DoubleX(){
        vid.playbackSpeed = 2f;
    }
    void PentaX(){
        vid.playbackSpeed = 5f;
    }
    void NanaX(){
        vid.playbackSpeed = 7f;
    }
    void jump(){
        vid.time = 890;
    }
}
