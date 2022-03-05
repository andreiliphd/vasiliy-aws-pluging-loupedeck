namespace Loupedeck.AWSPlugin
{
    using System;

    public class AWSApplication : ClientApplication
    {
        public AWSApplication()
        {

        }

        protected override String GetProcessName() => "";

        protected override String GetBundleName() => "";
    }
}