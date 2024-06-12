
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CMSuniVortex.Editor
{
    public sealed class TestConnection
    {
        [MenuItem("Tests/Connection")]
        public static async void Test()
        {
            var result = await GoogleSheetClient.GetSheet("19DrEi35I7H8f6bcUcORGIaUK8MmeLZ-ljkh7Fkbcxtw", "Translation");
            Debug.Log(result[0].Select(x => x.ToString()).Aggregate((a, b) => a + "," + b));
            Debug.Log(result[1].Select(x => x.ToString()).Aggregate((a, b) => a + "," + b));
        }
    }
}