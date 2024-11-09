
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Pool;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace CMSuniVortex.Editor
{
    public sealed class PackageInstaller : IDisposable
    {
        public bool IsProcessing{ get; private set; }

        bool _isDisposed;
        CancellationTokenSource _tokenSource;
        readonly ObjectPool<EditorAsync> _pool;

        public PackageInstaller()
        {
            _pool = new ObjectPool<EditorAsync>(
                createFunc: () => new EditorAsync(),
                actionOnGet: (editorAsync) => {  },
                actionOnRelease: (editorAsync) => editorAsync.Reset(),
                actionOnDestroy: (editorAsync) => editorAsync.Dispose(),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        public static void OpenPackageManager()
            => EditorApplication.ExecuteMenuItem("Window/Package Manager");

        public async Task<IEnumerable<PackageInfo>> GetInfos(CancellationToken token = default)
        {
            var op = _pool.Get();
            try
            {
                IsProcessing = true;
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                
                var request = Client.List();
                await op.StartAsync(() => request.IsCompleted, _tokenSource.Token);
                return request.Result;
            }
            finally
            {
                _pool.Release(op);
                if (_tokenSource != default)
                {
                    _tokenSource.Dispose();
                    _tokenSource = default;
                }
                IsProcessing = false;
            }
        }
        
        public async Task<PackageInfo> GetInfoByName(string packageName, CancellationToken token = default)
        {
            var infos = await GetInfos(token);
            foreach (var info in infos)
            {
                if (info.name.Contains(packageName, StringComparison.OrdinalIgnoreCase))
                {
                    return info;
                }
            }
            return default;
        }
        
        public async Task InstallAsync(string packageUrl, string packageName = default, CancellationToken token = default, bool showProgressBar = true)
        {
            var op = _pool.Get();
            try
            {
                if (showProgressBar)
                {
                    EditorUtility.DisplayProgressBar("Package operations", "Please wait...", 0.2f);
                }
                
                var isUpdate = false;
                var packageID = string.Empty;
                var version = string.Empty;
                if(!string.IsNullOrEmpty(packageName))
                {
                    var info = await GetInfoByName(packageName, token);
                    if (info != default)
                    {
                        packageID = info.packageId;
                        version = info.version;
                        isUpdate = true;
                    }
                }

                if (showProgressBar)
                {
                    EditorUtility.DisplayProgressBar("Package operations", isUpdate ? "Update package..." : "Install Package...", 0.5f);
                }
                
                if (isUpdate)
                {
                    packageID = packageID.Split('@')[0];
                }
                
                IsProcessing = true;
                var request = Client.Add(isUpdate ? packageID : packageUrl);
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                await op.StartAsync(() => request.IsCompleted, _tokenSource.Token);

                switch (request.Status)
                {
                    case StatusCode.Success:
                        var msg = default(string);
                        if (isUpdate)
                        {
                            if (request.Result.version == version)
                            {
                                msg = "You have the latest version. ID: " + request.Result.packageId;
                            }
                            else
                            {
                                msg = "Updated ID: " + request.Result.packageId + ", New version: " + request.Result.version + ", Prev version: " + version;
                            }
                        }
                        else
                        {
                            msg = "Installed ID: " + request.Result.packageId + ", Version: " + request.Result.version;
                        }
                        Debug.Log(msg + ".\nYou can check the details in the Package Manager.");
                        break;
                    case StatusCode.Failure:
                        Debug.LogError(request.Error.message);
                        break;
                }
            }
            finally
            {
                _pool.Release(op);
                if (_tokenSource != default)
                {
                    _tokenSource.Dispose();
                    _tokenSource = default;
                }
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }
                IsProcessing = false;
            }
        }

        public void Cancel()
        {
            if (_tokenSource == default)
            {
                return;
            }
            if (!_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }
            _tokenSource.Dispose();
            _tokenSource = default;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            if (_tokenSource != default)
            {
                _tokenSource.Dispose();
                _tokenSource = default;
            }
            _pool.Dispose();
        }
    }
}