using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace AI.FSM
{
    public interface IState
    {
        
        //수행 전 초기화작업
        void Enter();
        
        
        //상태 전환
        void Execute();
        
        
        //상태 종료
        void Exit();
    }
}




