using TMPro;
using UnityEngine;

namespace Monaverse.Modal.UI.Views.Elements
{
    public class TokenAttributeViewElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _key;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private TMP_Text _rarity;
        
        public void Set(string key, string value, string rarity)
        {
            _key.text = key;
            _value.text = value;
            _rarity.text = rarity;
        }
    }
}