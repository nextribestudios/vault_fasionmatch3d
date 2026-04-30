using UnityEngine;

namespace Nextribe.Firebase
{
    [CreateAssetMenu(fileName = "RemoteConfigURLLinkDataSO", menuName = "ScriptableObjects/RemoteConfigURLLinkDataSO", order = 1)]
    public class RemoteConfigURLLinkDataSO : ScriptableObject
    {
        [SerializeField]
        private string[] RandomLocalURLLinks;        
        public string GetRandomURL()
        {
            string temp = "";
            temp = RandomLocalURLLinks[Random.Range(0, RandomLocalURLLinks.Length)];
            return temp;
        }
    }
}