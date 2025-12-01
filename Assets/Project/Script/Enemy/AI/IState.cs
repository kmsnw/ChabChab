using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Enemy AI용 state 인터페이스


namespace AI.FSM
{
    public interface IState
    {
        
        //수행 전 초기화
        void Enter();
        
        
        //상태 전환
        void Execute();
        
        
        //상태 종료
        void Exit();
    }
}




