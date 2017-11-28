using Xunit;


namespace stratfaucet
{

    public class ThrottleTest
    {
        [Fact]
        public void WillNotSendWhenRequestingIPHasBeenUsed()
        {
           Assert.True(false);

        }

        [Fact]
        public void WillNotSendWhenReceivingAddressHasBeenUsed(){
          Assert.True(false);
        }
    }

}
