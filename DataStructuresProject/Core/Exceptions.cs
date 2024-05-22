using System;

namespace DataStructuresProject.Core
{
#pragma warning disable IDE0290 // Use primary constructor
    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException(int key) :
            base($"The key \"{key}\" was already present in the binary tree.")
        { }
    }
#pragma warning restore IDE0290 // Use primary constructor
}
