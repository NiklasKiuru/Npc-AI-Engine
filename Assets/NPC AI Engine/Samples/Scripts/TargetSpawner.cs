using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine.Samples
{
    public class TargetSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _playerTarget;
        [SerializeField] private LayerMask _ground;

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out var info, 100, _ground))
                {   
                    var copy = Instantiate(_playerTarget, info.point, Quaternion.identity);
                    var offset = copy.GetComponent<Collider>().bounds.extents.y;
                    copy.transform.position += new Vector3(0, offset, 0);
                }
            }
        }
    }

}
