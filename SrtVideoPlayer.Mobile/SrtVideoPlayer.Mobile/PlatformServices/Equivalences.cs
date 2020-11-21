using SrtVideoPlayer.Shared.Models.Enums;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    static class Equivalences
    {
        public static readonly Dictionary<KeyboardType, Keyboard> Keyboards = new Dictionary<KeyboardType, Keyboard>()
        {
            { KeyboardType.Text, Keyboard.Text },
            { KeyboardType.Plain, Keyboard.Plain },
            { KeyboardType.Numeric, Keyboard.Numeric },
            { KeyboardType.Email, Keyboard.Email },
            { KeyboardType.Phone, Keyboard.Telephone },
            { KeyboardType.Url, Keyboard.Url }
        };
    }
}
