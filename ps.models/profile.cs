using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ps.models
{
    public class ProviderSetting
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
    public class Provider
    {
        public string ProviderName { get; set; }
        public string ProviderType { get; set; }
        public List<ProviderSetting> ProviderSettings { get; set; }
    }
    public class Profile
    {
        public string ProfileName { get; set; }
        public string ProfileKey { get; set; }
        public int ProfileID { get; set; }
        public Provider Provider { get; set; }
    }
}
