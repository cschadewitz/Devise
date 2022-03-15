using System;
using System.Collections.Generic;
using System.Text;
using Cottle;

namespace Devise.Utilities
{
    internal class DeviseEntity
    {
        public string ApiNamespace { get; set; }
        public string BusinessNamespace { get; set; }
        public string DataNamespace { get; set; }
        public string Name { get; set; }
        public Dictionary<DeviseTarget, IEnumerable<(DeviseOperation operation, bool isCustom)>> Targets { get; set; }
        public IEnumerable<(string name, string type)> Properties{ get; set; }


    }
}
