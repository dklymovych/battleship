using Client.MVVM.Model;
using System.Windows.Media;

namespace ClientTests
{
    public class MVVM_Model_User
    {
        [Fact]
        public void User_ConstructedWithAccessToken_HasCorrectAccessToken()
        {
            string accessToken = "example_token";

            var user = new User(accessToken);

            Assert.Equal(accessToken, user.access_token);
        }

        [Fact]
        public void User_Logout_SetsAccessTokenToEmptyString()
        {
            var user = new User("example_token");

            user.Logout();

            Assert.Equal(string.Empty, user.access_token);
        }

        [Fact]
        public void User_ToString_ReturnsAccessToken()
        {
            string accessToken = "example_token";
            var user = new User(accessToken);

            string result = user.ToString();

            Assert.Equal(accessToken, result);
        }
    }
}