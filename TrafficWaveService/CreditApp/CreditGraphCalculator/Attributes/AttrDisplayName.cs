using System;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Attributes
{
    public class AttrDisplayName : Attribute
    {
        private string value;

        public virtual string Value { get { return value; } }
        public AttrDisplayName(string value)
        {
            this.value = value;
        }
    }
}
