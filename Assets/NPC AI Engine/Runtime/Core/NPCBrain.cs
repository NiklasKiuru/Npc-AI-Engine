using Aikom.AIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class NPCBrain : MonoBehaviour
    {
        [SerializeField] private float _delta;
        private BehaviourTree _tree;
        private WaitForSeconds _delay;


        private void Start ()   
        {   
            _delay = new WaitForSeconds(_delta);
            _tree  = new BehaviourTree();

        }
        
        private float _timePassed;
        private void Update()
        {
            _timePassed += Time.deltaTime;
            if( _timePassed > _delta)
            {
                _timePassed = 0;
                _tree.Process();
            }
        }
    }

}
