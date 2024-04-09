using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State{
    Idle = 0,
    Accelerated = 1,
    Jump = 2,
    Glide = 3,
};

public class CharacterState : MonoBehaviour{
    public GameObject[] states;
    private State current;

    public void ChangeState(State next){
        if(next == current) return;

        states[(int)current].SetActive(false);
        current = next;
        states[(int)current].SetActive(true);
    }
}
