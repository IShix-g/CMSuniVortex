
using System;

namespace CMSuniVortex
{
    public interface ICuvUpdateChecker
    {
        bool IsUpdateAvailable();
        void CheckForUpdate(string buildPath, Action<bool, string> successAction, Action<string> failureAction);
    }
}