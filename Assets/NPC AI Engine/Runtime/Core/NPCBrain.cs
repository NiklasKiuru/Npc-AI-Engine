using Aikom.AIEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public class NPCBrain : MonoBehaviour, IExecutionContext
    {
        [SerializeField] private float _delta;
        [SerializeField] private TreeAsset _asset;
        [SerializeField] TickType _tickType;
        private BehaviourTree _tree;
        private WaitForSeconds _delay;

        private float _tickTime;

        public bool IsActive 
        { 
            get 
            { 
                if( _tree == null )
                    return false;
                return _tree.IsActive;
            } 
        }

        public float TickDelay => _tickTime;

        private void Start ()   
        {   
            // **TEMP**
            _tickTime = _delta;
            if(_asset != null )
                _tree  = new BehaviourTree(_asset, gameObject, this);
            _tree.Initialize();
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

#if UNITY_EDITOR
        /// <summary>
        /// Gets all process status codes within the tree (Editor only)
        /// </summary>
        /// <param name="dic"></param>
        public void GetStates(ref Dictionary<int, NodeStatus> dic)
        {
            
        }
#endif
    }

}
