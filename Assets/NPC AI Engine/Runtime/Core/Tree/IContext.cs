using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    public interface IContext
    {
        public object GetLocalVariable(string name);
        public void SetLocalVariable(string name, object value);
        public object GetGlobalVariable(string name);
    }

}
