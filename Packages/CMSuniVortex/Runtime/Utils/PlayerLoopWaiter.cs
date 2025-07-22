
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.LowLevel;
using UnityEngine.Pool;

namespace CMSuniVortex
{
    public static class PlayerLoopWaiter
    {
        static readonly List<(Func<bool> Condition, TaskCompletionSource<bool> Source, CancellationToken Token)> s_asyncWaitConditions = new();
        
        static PlayerLoopSystem s_cachedPlayerLoop;
        static bool s_isRegistered;
        
        public static Task StartAsync(Func<bool> condition, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            s_asyncWaitConditions.Add((condition, tcs, cancellationToken));
            
            if (!s_isRegistered)
            {
                RegisterToPlayerLoop();
                s_isRegistered = true;
            }
            return tcs.Task;
        }
        
        static void RegisterToPlayerLoop()
        {
            if (s_cachedPlayerLoop.subSystemList == default)
            {
                s_cachedPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            }
            
            var playerLoop = s_cachedPlayerLoop;
            var customSystem = new PlayerLoopSystem
            {
                type = typeof(PlayerLoopWaiter),
                updateDelegate = ProcessConditions
            };

            for (var i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type != typeof(UnityEngine.PlayerLoop.Update))
                {
                    continue;
                }
                
                var subSystems = playerLoop.subSystemList[i].subSystemList;
                if (Array.Exists(subSystems, system => system.type == typeof(PlayerLoopWaiter)))
                {
                    return;
                }

                var newSubSystems = new PlayerLoopSystem[subSystems.Length + 1];
                Array.Copy(subSystems, newSubSystems, subSystems.Length);
                newSubSystems[subSystems.Length] = customSystem;
                playerLoop.subSystemList[i].subSystemList = newSubSystems;
                PlayerLoop.SetPlayerLoop(playerLoop);
                s_cachedPlayerLoop = playerLoop;
                return;
            }
        }
        
        static void ProcessConditions()
        {
            var removalIndices = ListPool<int>.Get();
            try
            {
                for (var i = 0; i < s_asyncWaitConditions.Count; i++)
                {
                    var target = s_asyncWaitConditions[i];

                    if (target.Token.IsCancellationRequested)
                    {
                        target.Source.SetCanceled();
                        removalIndices.Add(i);
                    }
                    else if (target.Condition.Invoke())
                    {
                        target.Source.SetResult(true);
                        removalIndices.Add(i);
                    }
                }

                for (var j = removalIndices.Count - 1; j >= 0; j--)
                {
                    var index = removalIndices[j];
                    if (index < s_asyncWaitConditions.Count - 1)
                    {
                        s_asyncWaitConditions[index] = s_asyncWaitConditions[^1];
                    }
                    s_asyncWaitConditions.RemoveAt(s_asyncWaitConditions.Count - 1);
                }

                if (s_asyncWaitConditions.Count == 0)
                {
                    UnregisterFromPlayerLoop();
                    s_isRegistered = false;
                }
            }
            finally
            {
                ListPool<int>.Release(removalIndices);
            }
        }
 
        static void UnregisterFromPlayerLoop()
        {
            if (s_cachedPlayerLoop.subSystemList == default)
            {
                s_cachedPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            }

            var playerLoop = s_cachedPlayerLoop;

            for (var i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type != typeof(UnityEngine.PlayerLoop.Update))
                {
                    continue;
                }
                
                var subSystems = playerLoop.subSystemList[i].subSystemList;
                var index = Array.FindIndex(subSystems, system => system.type == typeof(PlayerLoopWaiter));
                if (index == -1)
                {
                    return;
                }

                var newSubSystems = new PlayerLoopSystem[subSystems.Length - 1];
                if (index > 0) Array.Copy(subSystems, 0, newSubSystems, 0, index);
                if (index < subSystems.Length - 1) Array.Copy(subSystems, index + 1, newSubSystems, index, subSystems.Length - index - 1);

                playerLoop.subSystemList[i].subSystemList = newSubSystems;
                PlayerLoop.SetPlayerLoop(playerLoop);
                s_cachedPlayerLoop = playerLoop;
                return;
            }
        }

    }
}