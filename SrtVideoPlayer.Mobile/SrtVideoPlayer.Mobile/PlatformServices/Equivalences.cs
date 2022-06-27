using SrtVideoPlayer.Shared.Models.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal static class Equivalences
    {
        public static readonly IReadOnlyDictionary<KeyboardType, Keyboard> Keyboards = new ReadOnlyDictionary<KeyboardType, Keyboard>(new Dictionary<KeyboardType, Keyboard>()
        {
            { KeyboardType.Text, Keyboard.Text },
            { KeyboardType.Plain, Keyboard.Plain },
            { KeyboardType.Numeric, Keyboard.Numeric },
            { KeyboardType.Email, Keyboard.Email },
            { KeyboardType.Phone, Keyboard.Telephone },
            { KeyboardType.Url, Keyboard.Url }
        });

        public static readonly IReadOnlyDictionary<string, DeviceOs> DeviceOss = new ReadOnlyDictionary<string, DeviceOs>(new Dictionary<string, DeviceOs>()
        {
            { Device.Android, DeviceOs.Android },
            { Device.iOS, DeviceOs.iOS }
        });
    }
}
