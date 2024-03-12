using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
namespace Aikom.AIEngine.Editor
{
    public class Branch
    {
        public string Name { get; set; }
        
        public Branch(string name)
        {
            Name = name;
        }
    }
}
