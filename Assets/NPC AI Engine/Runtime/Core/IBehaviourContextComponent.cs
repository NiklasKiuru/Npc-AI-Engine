using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public interface IBehaviourContextComponent
    {
        public object GetValue(string name);
        public void SetValue(string name, object value);
    }

}
