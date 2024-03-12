using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine.Editor
{
    internal class BranchContainer : ScriptableObject
    {
        public Dictionary<string, Branch> Templates { get; } = new Dictionary<string, Branch>();
    }
}
