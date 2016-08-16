using System;

namespace SMDR_Service
{
    internal struct SMDRRecord
    {
        public DateTime CallStart;
        public TimeSpan ConnectedTime;
        public TimeSpan RingTime;
        public string Caller;
        public string CallDirection;
        public string DialedNumber;

        public override string ToString()
        {
            return (string)(object)this.CallStart + (object)"," + (string)(object)this.ConnectedTime.TotalSeconds + "," + (string)(object)this.RingTime.TotalSeconds + "," + this.Caller + "," + this.CallDirection + "," + this.DialedNumber;
        }
    }
}
