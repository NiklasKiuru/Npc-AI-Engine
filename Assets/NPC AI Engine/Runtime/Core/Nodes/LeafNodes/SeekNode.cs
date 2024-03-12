using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    [Serializable]
    public class SeekNode : LeafNode
    {
        [ExposedVariable("Valid layers", "The layers targets can be found from")]
        private LayerMask _validLayers;

        [ExposedVariable("Obstruction layers", "Layers that obstruct the view of the seeker")]
        private LayerMask _obstructionMask;

        [ExposedVariable("Radius", "Radius to seek objects from")]
        private float _radius;

        [ExposedVariable("Max targets", "Maximum search targets per tick")]
        private int _maxTargets;

        [ExposedVariable("Offset", "Offset from transforms position to use as center point")]
        private Vector3 _offset;

        [ExposedVariable("Local variable cache")]
        private string _localCacheName;

        private Collider[] _objectCache;
        private float[] _results;

        protected override void OnBuild()
        {
        }

        protected override void OnInit()
        {
            if (Context.GetLocalVariable<Collider[]>(_localCacheName) == null)
            {
                _objectCache = new Collider[_maxTargets];
                _results = new float[_maxTargets];
            }

            Context.SetLocalVariable(_localCacheName, _objectCache);
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
            return NodeStatus.Succes;
        }

        private void CreateArray()
        {
            
        }
    }
}
