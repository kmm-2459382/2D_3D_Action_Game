using UnityEngine;
using TheClimb.Item;
using TheClimb.Astral;
using TMPro;

namespace TheClimb.Core
{
    [RequireComponent(typeof(ObjectLabelController))]
    public class ObjectLabelBootstrap : MonoBehaviour    //  オブジェクトラベルBootstrap
    {
        [SerializeField]
        ObjectLabelConfigBase _labelConfig;
        [SerializeField]
        Transform _labelTargetTF;
        [SerializeField]
        AttractableListenerBase _itemController;
        [SerializeField]
        TextMeshPro _label;

        ObjectLabelContext _ctx;
        
        void Awake()
        {
            _ctx = new ObjectLabelContext(Camera.main.transform, _labelTargetTF, _itemController, _label);
        }

        void Start()
        {
            GetComponent<ObjectLabelController>().Initialize(_labelConfig, _ctx);
        }
    }
}