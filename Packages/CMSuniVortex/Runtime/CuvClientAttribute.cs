
using System;
using System.Diagnostics;

namespace CMSuniVortex
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class)]
    public class CuvClientAttribute : Attribute
    {
        public string ClientName { get; }

        public CuvClientAttribute(string clientName)
        {
            ClientName = clientName;
        }
    }
}