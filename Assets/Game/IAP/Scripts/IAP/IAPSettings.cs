using UnityEngine;

namespace IAP
{
    public class IAPSettings : MonoBehaviour
    {
        private static IAPSettingsData _data;

        public static IAPSettingsData Data
        {
            get
            {
                if (_data == null)
                    _data = Resources.Load<IAPSettingsData>("IAPSettingsData");

                return _data;
            }
        }
    }
}