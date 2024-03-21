using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    [EditorNode("Physics based search query. Saves results into a distance based stack object Stack<GameObject>")]
    public class SeekNode : LeafNode
    {
        [SerializeField, Tooltip("The layers targets can be found from")]
        private LayerMask _validLayers;

        [SerializeField, Tooltip("Layers that obstruct the view of the seeker")]
        private LayerMask _obstructionMask;

        [SerializeField, Tooltip("Radius to seek objects from")]
        private float _radius;

        [SerializeField, Tooltip("Maximum search targets per tick")]
        private int _maxTargets;

        [SerializeField, Tooltip("Offset from transforms position to use as center point")]
        private Vector3 _offset;

        [SerializeField, Tooltip("Local variable cache")]
        [CacheVariable(true)]
        private CacheVariable _localCacheName;

        private Collider[] _objectCache;
        private float[] _results;
        private Stack _stack;

        public SeekNode(int id) : base(id)
        {
        }

        protected SeekNode(int id, Position pos) : base(id, pos)
        {
        }

        public override INode Clone()
        {
            var newNode = new SeekNode(Id, Position);
            newNode._validLayers = _validLayers;
            newNode._obstructionMask = _obstructionMask;
            newNode._radius = _radius;
            newNode._maxTargets = _maxTargets;
            newNode._offset = _offset;
            newNode._localCacheName = _localCacheName;
            return newNode;
        }

        protected override void OnBuild()
        {
            _objectCache = new Collider[_maxTargets];
            _results = new float[_maxTargets];
            _stack = new Stack();
            Context.SetLocalVariable(_localCacheName.Name, _stack);
        }

        protected override void OnInit()
        {
            _stack.Clear();
        }

        protected override NodeStatus Tick()
        {   
            var pos = Context.Target.transform.position + _offset;
            Physics.OverlapSphereNonAlloc(pos, _radius, _objectCache, _validLayers, QueryTriggerInteraction.Collide);
            bool hasVisibleTargets = false;
            for(int i = 0; i < _maxTargets; i++)
            {
                _results[i] = _radius;

                if (_objectCache[i] != null)
                {
                    var obj = _objectCache[i].gameObject;
                    if (!Physics.Raycast(pos, obj.transform.position - pos, _radius, _obstructionMask))
                    {
                        hasVisibleTargets = true;
                        _results[i] = Vector3.Distance(pos, obj.transform.position);
                    }
                    else
                        _objectCache[i] = null;
                }
                else
                    hasVisibleTargets |= false;
            }
            if (!hasVisibleTargets)
                return NodeStatus.Failure;

            // Reorder the array from closest to farthests with insertion sort
            int j = 1;
            while (j < _maxTargets)
            {
                int k = j;
                while(k > 0 && _results[k - 1] > _results[k])
                {
                    (_results[k - 1], _results[k]) = (_results[k], _results[k - 1]);
                    (_objectCache[k - 1], _objectCache[k]) = (_objectCache[k], _objectCache[k - 1]);
                    k -= 1;
                }
                j++;
            }
            for(int i = 0; i < _maxTargets; i++ )
            {
                if (_objectCache[i] != null)
                    _stack.Push(_objectCache[i].gameObject);
                else
                    break;
            }

            return NodeStatus.Succes;
        }
    }
}
