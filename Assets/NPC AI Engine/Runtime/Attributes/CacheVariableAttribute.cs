using System;

namespace Aikom.AIEngine
{   

    [AttributeUsage(AttributeTargets.Field)]
    public class CacheVariableAttribute : Attribute
    {   
        public bool RestrictSelection { get; private set; }
        public CacheSpace Space { get; private set; }
        public CacheVariableAttribute(bool restrictSelection = false, CacheSpace defaultSpace = CacheSpace.Local) 
        { 
            RestrictSelection = restrictSelection;
            Space = defaultSpace;
        }
    }

}
